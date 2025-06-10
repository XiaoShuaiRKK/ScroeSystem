package com.score.system.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.*;
import com.score.system.entity.user.Student;
import com.score.system.entity.user.StudentClassHistory;
import com.score.system.mapper.*;
import com.score.system.service.ExamClassSubjectStatService;
import com.score.system.util.RedisUtil;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.*;
import java.util.concurrent.TimeUnit;
import java.util.stream.Collectors;

@Service
public class ExamClassSubjectStatServiceImpl implements ExamClassSubjectStatService {
    private final ExamMapper examMapper;
    private final ExamClassSubjectStatMapper examClassSubjectStatMapper;
    private final ScoreMapper scoreMapper;
    private final ClassMapper classMapper;
    private final StudentMapper studentMapper;
    private final CriticalConfigMapper criticalConfigMapper;
    private final StudentClassHistoryMapper historyMapper;
    private final RedisUtil redisUtil;

    public ExamClassSubjectStatServiceImpl(ExamMapper examMapper, ExamClassSubjectStatMapper examClassSubjectStatMapper, ScoreMapper scoreMapper, ClassMapper classMapper, StudentMapper studentMapper, CriticalConfigMapper configMapper, StudentClassHistoryMapper historyMapper, RedisUtil redisUtil) {
        this.examMapper = examMapper;
        this.examClassSubjectStatMapper = examClassSubjectStatMapper;
        this.scoreMapper = scoreMapper;
        this.classMapper = classMapper;
        this.studentMapper = studentMapper;
        this.criticalConfigMapper = configMapper;
        this.historyMapper = historyMapper;
        this.redisUtil = redisUtil;
    }

    @Override
    @Transactional
    public ResponseResult<Boolean> generateExamStat(int examId) {
        Exam exam = examMapper.selectById(examId);
        if (exam == null) {
            return ResponseResult.fail("考试不存在");
        }

        Integer grade = exam.getGrade();
        List<StudentClassHistory> histories = historyMapper.selectList(
                new LambdaQueryWrapper<StudentClassHistory>()
                        .eq(StudentClassHistory::getGrade, exam.getGrade())
                        .eq(StudentClassHistory::getYear, exam.getYear())
        );
        List<Long> cIds = histories.stream().map(StudentClassHistory::getClassId).toList();
        List<ClassEntity> allClasses = classMapper.selectList(
                new LambdaQueryWrapper<ClassEntity>().in(ClassEntity::getId, cIds)
        );
        if (allClasses.isEmpty()) return ResponseResult.fail("没有班级信息");

        Map<Long, List<ClassEntity>> subjectGroupToClasses = allClasses.stream()
                .collect(Collectors.groupingBy(ClassEntity::getSubjectGroupId));

        List<Long> courseIds = scoreMapper.selectDistinctCoursesByExamId(examId);
        courseIds.add(100L); // 100L 表示 3+1+2 总分

        List<Integer> allClassIds = allClasses.stream().map(ClassEntity::getId).toList();
        List<Student> allStudents = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().in(Student::getClassId, allClassIds)
        );
        if (allStudents.isEmpty()) return ResponseResult.fail("没有学生数据");

        Map<String, Student> studentMap = allStudents.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, s -> s));

        Map<Integer, ClassEntity> classIdMap = allClasses.stream()
                .collect(Collectors.toMap(ClassEntity::getId, c -> c));

        List<Score> allScores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>().eq(Score::getExamId, examId)
        );
        if (allScores.isEmpty()) return ResponseResult.fail("没有成绩数据");

        Map<String, Map<Long, Double>> studentScores = new HashMap<>();
        for (Score score : allScores) {
            studentScores
                    .computeIfAbsent(score.getStudentNumber(), k -> new HashMap<>())
                    .put(score.getCourseId(), score.getScore() != null ? score.getScore() : 0.0);
        }

        // 计算每个学生的 3+1+2 总分，放入 courseId=100L
        for (Student student : allStudents) {
            String sn = student.getStudentNumber();
            Map<Long, Double> scores = studentScores.getOrDefault(sn, Map.of());

            List<Long> core = List.of(1L, 2L, 3L); // 语数英
            double three = core.stream().mapToDouble(cid -> scores.getOrDefault(cid, 0.0)).sum();

            List<Double> optional = scores.entrySet().stream()
                    .filter(e -> !core.contains(e.getKey()))
                    .map(Map.Entry::getValue)
                    .sorted(Comparator.reverseOrder())
                    .toList();

            double one = optional.size() > 0 ? optional.get(0) : 0.0;
            double two = optional.size() > 1 ? optional.get(1) : 0.0;

            studentScores
                    .computeIfAbsent(sn, k -> new HashMap<>())
                    .put(100L, three + one + two);
        }

        List<ExamClassSubjectStat> existingStats = examClassSubjectStatMapper.selectList(
                new LambdaQueryWrapper<ExamClassSubjectStat>().eq(ExamClassSubjectStat::getExamId, examId)
        );
        Set<String> existingKeys = existingStats.stream()
                .map(stat -> stat.getClassId() + "-" + stat.getCourseId() + "-" + stat.getUniversityLevel())
                .collect(Collectors.toSet());

        List<ExamClassSubjectStat> toInsert = new ArrayList<>();
        List<ExamClassSubjectStat> toUpdate = new ArrayList<>();

        for (Map.Entry<Long, List<ClassEntity>> entry : subjectGroupToClasses.entrySet()) {
            Long subjectGroupId = entry.getKey();
            List<ClassEntity> classList = entry.getValue();

            List<CriticalConfig> criticalConfigs = criticalConfigMapper.selectList(
                    new LambdaQueryWrapper<CriticalConfig>()
                            .eq(CriticalConfig::getGrade, grade)
                            .eq(CriticalConfig::getSubjectGroupId, subjectGroupId)
                            .eq(CriticalConfig::isDeleted, false)
            );
            Map<Integer, Integer> universityTargetMap = criticalConfigs.stream()
                    .collect(Collectors.toMap(CriticalConfig::getUniversityLevel, CriticalConfig::getTargetCount));

            List<Student> students = allStudents.stream()
                    .filter(s -> {
                        ClassEntity cls = classIdMap.get(s.getClassId());
                        return cls != null && cls.getSubjectGroupId().equals(subjectGroupId);
                    }).toList();

            Map<String, Student> sgStudentMap = students.stream()
                    .collect(Collectors.toMap(Student::getStudentNumber, s -> s));
            List<String> studentNumbers = new ArrayList<>(sgStudentMap.keySet());

            // 1. 准备全体年级学生每科成绩排序（用于协同人数）
            Map<Long, List<Map.Entry<String, Double>>> gradeCourseRankMap = new HashMap<>();
            for (Long cid : courseIds) {
                List<Map.Entry<String, Double>> rankList = students.stream()
                        .map(s -> Map.entry(s.getStudentNumber(),
                                studentScores.getOrDefault(s.getStudentNumber(), Map.of()).getOrDefault(cid, 0.0)))
                        .sorted(Map.Entry.<String, Double>comparingByValue().reversed())
                        .toList();
                gradeCourseRankMap.put(cid, rankList);
            }

            // 2. 准备班级内学生成绩排序（用于贡献率）
            Map<Integer, List<String>> classIdToStudentNumbers = new HashMap<>();
            for (ClassEntity cls : classList) {
                List<String> clsStudentNumbers = students.stream()
                        .filter(s -> s.getClassId().equals(cls.getId()))
                        .map(Student::getStudentNumber)
                        .toList();
                classIdToStudentNumbers.put(cls.getId(), clsStudentNumbers);
            }

            // 3. 计算每个班每等级“3+1+2”总分在目标线内的学生集合（贡献率分母）
            // 构造 "3+1+2" 总分 排名（courseId = 100L）
            List<Map.Entry<String, Double>> totalScoreRankList = students.stream()
                    .map(s -> Map.entry(s.getStudentNumber(),
                            studentScores.getOrDefault(s.getStudentNumber(), Map.of()).getOrDefault(100L, 0.0)))
                    .sorted(Map.Entry.<String, Double>comparingByValue().reversed())
                    .toList();

            Map<String, Set<String>> classLevelTotalScoreTopMap = new HashMap<>();
            for (Map.Entry<Integer, Integer> target : universityTargetMap.entrySet()) {
                Integer level = target.getKey();
                Integer targetCount = target.getValue();

                double cutoffScore = targetCount <= totalScoreRankList.size()
                        ? totalScoreRankList.get(targetCount - 1).getValue()
                        : totalScoreRankList.get(totalScoreRankList.size() - 1).getValue();

                Set<String> topTotalStudents = totalScoreRankList.stream()
                        .filter(e -> e.getValue() >= cutoffScore)
                        .map(Map.Entry::getKey)
                        .collect(Collectors.toSet());

                for (ClassEntity cls : classList) {
                    Set<String> clsSet = new HashSet<>(classIdToStudentNumbers.getOrDefault(cls.getId(), Collections.emptyList()));
                    Set<String> classTopTotal = topTotalStudents.stream()
                            .filter(clsSet::contains)
                            .collect(Collectors.toSet());
                    classLevelTotalScoreTopMap.put(cls.getId() + "-" + level, classTopTotal);
                }
            }

            // 4. 计算每个班每科每等级的统计数据
            for (Long courseId : courseIds) {
                for (ClassEntity cls : classList) {
                    List<String> classStudentNumbers = classIdToStudentNumbers.getOrDefault(cls.getId(), Collections.emptyList());

                    double avgScore = classStudentNumbers.stream()
                            .mapToDouble(sn -> studentScores.getOrDefault(sn, Map.of()).getOrDefault(courseId, 0.0))
                            .average().orElse(0.0);

                    for (Map.Entry<Integer, Integer> target : universityTargetMap.entrySet()) {
                        Integer level = target.getKey();
                        Integer targetCount = target.getValue();

                        // 计算协同人数
                        List<Map.Entry<String, Double>> gradeRankList = gradeCourseRankMap.get(courseId);
                        int synergyCount = 0;
                        double synergyRate = 0.0;
                        if (gradeRankList != null && !gradeRankList.isEmpty() && targetCount > 0) {
                            double cutoffScore = targetCount <= gradeRankList.size()
                                    ? gradeRankList.get(targetCount - 1).getValue()
                                    : gradeRankList.get(gradeRankList.size() - 1).getValue();

                            Set<String> topStudents = gradeRankList.stream()
                                    .filter(e -> e.getValue() >= cutoffScore)
                                    .map(Map.Entry::getKey)
                                    .collect(Collectors.toSet());

                            Set<String> classStudentSet = new HashSet<>(classStudentNumbers);
                            synergyCount = (int) topStudents.stream().filter(classStudentSet::contains).count();

                            synergyRate = synergyCount == 0 ? 0.0 : (double) synergyCount / targetCount;
                        }

                        // 计算贡献人数和贡献率
                        int contributionCount = 0;
                        double contributionRate = 0.0;
                        if (!classStudentNumbers.isEmpty()) {
                            List<Map.Entry<String, Double>> classSorted = classStudentNumbers.stream()
                                    .map(sn -> Map.entry(sn, studentScores.getOrDefault(sn, Map.of()).getOrDefault(courseId, 0.0)))
                                    .sorted(Map.Entry.<String, Double>comparingByValue().reversed())
                                    .toList();

                            if (synergyCount > 0 && classSorted.size() >= synergyCount) {
                                double subjectCutoff = classSorted.get(synergyCount - 1).getValue();

                                Set<String> subjectTopStudents = classSorted.stream()
                                        .filter(e -> e.getValue() >= subjectCutoff)
                                        .map(Map.Entry::getKey)
                                        .collect(Collectors.toSet());

                                contributionCount = subjectTopStudents.size();

                                // 使用正确的分母，班级该等级下3+1+2总分目标线人数
                                Set<String> totalTopSet = classLevelTotalScoreTopMap.getOrDefault(cls.getId() + "-" + level, Set.of());
                                contributionRate = totalTopSet.isEmpty() ? 0.0 : (double) contributionCount / totalTopSet.size();
                            } else {
                                contributionCount = 0;
                                contributionRate = 0.0;
                            }
                        }

                        ExamClassSubjectStat stat = new ExamClassSubjectStat();
                        stat.setExamId((long) examId);
                        stat.setCourseId(courseId);
                        stat.setClassId(cls.getId());
                        stat.setUniversityLevel(level);
                        stat.setAvgScore(avgScore);
                        stat.setSynergyCount(synergyCount);
                        stat.setSynergyRate(synergyRate);
                        stat.setContributionCount(contributionCount);
                        stat.setContributionRate(contributionRate);

                        String key = cls.getId() + "-" + courseId + "-" + level;
                        if (existingKeys.contains(key)) {
                            toUpdate.add(stat);
                        } else {
                            toInsert.add(stat);
                        }
                    }
                }
            }

            String redisKey = "exam:stat:" + examId + ":" + subjectGroupId;
            redisUtil.delete(redisKey);
        }

        if (!toInsert.isEmpty()) {
            examClassSubjectStatMapper.insertBatchSomeColumn(toInsert);
        }
        if (!toUpdate.isEmpty()) {
            examClassSubjectStatMapper.updateBatchById(toUpdate);
        }

        return ResponseResult.success(true);
    }






    @Override
    public ResponseResult<List<ExamClassSubjectStat>> getExamClassSubjectStat(int examId, int subjectGroupId) {
        if (examId == 0 || subjectGroupId == 0) {
            return ResponseResult.fail("请输入正确的考试和分科");
        }

        // 1. 构建 Redis 缓存 Key
        String cacheKey = "exam:stat:" + examId + ":" + subjectGroupId;

        // 2. 尝试从 Redis 获取
        if (redisUtil.exists(cacheKey)) {
            List<ExamClassSubjectStat> cachedList = redisUtil.get(cacheKey, List.class);
            return ResponseResult.success(cachedList);
        }

        // 3. 查询数据库
        Exam exam = examMapper.selectById(examId);
        if (exam == null) {
            return ResponseResult.fail("未查找到对应考试,请查看是否已被删除");
        }

        List<StudentClassHistory> histories = historyMapper.selectList(
                new LambdaQueryWrapper<StudentClassHistory>()
                        .eq(StudentClassHistory::getGrade,exam.getGrade())
                        .eq(StudentClassHistory::getYear,exam.getYear())
        );
        List<Long> cIds = histories.stream().map(StudentClassHistory::getClassId).toList();
        List<ClassEntity> classEntities = classMapper.selectList(
                new LambdaQueryWrapper<ClassEntity>()
                        .in(ClassEntity::getId,cIds)
                        .eq(ClassEntity::getSubjectGroupId, subjectGroupId)
        );
        if (classEntities.isEmpty()) {
            return ResponseResult.fail("未查找到对应班级信息");
        }

        // classId -> className 映射
        Map<Integer, String> classNameMap = classEntities.stream()
                .collect(Collectors.toMap(ClassEntity::getId, ClassEntity::getName));

        List<Integer> classIds = classEntities.stream()
                .map(ClassEntity::getId)
                .toList();

        List<ExamClassSubjectStat> result = examClassSubjectStatMapper.selectList(
                new LambdaQueryWrapper<ExamClassSubjectStat>()
                        .eq(ExamClassSubjectStat::getExamId, examId)
                        .in(ExamClassSubjectStat::getClassId, classIds)
        );

        // 设置 className
        for (ExamClassSubjectStat stat : result) {
            stat.setClassName(classNameMap.get(stat.getClassId()));
        }

        // 4. 写入缓存，设置有效时间（例如 10 分钟）
        redisUtil.set(cacheKey, result, TimeUnit.MINUTES, 10);

        return ResponseResult.success(result);
    }
}
