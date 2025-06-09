package com.score.system.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.*;
import com.score.system.entity.user.Student;
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
    private final RedisUtil redisUtil;

    public ExamClassSubjectStatServiceImpl(ExamMapper examMapper, ExamClassSubjectStatMapper examClassSubjectStatMapper, ScoreMapper scoreMapper, ClassMapper classMapper, StudentMapper studentMapper, CriticalConfigMapper configMapper, RedisUtil redisUtil) {
        this.examMapper = examMapper;
        this.examClassSubjectStatMapper = examClassSubjectStatMapper;
        this.scoreMapper = scoreMapper;
        this.classMapper = classMapper;
        this.studentMapper = studentMapper;
        this.criticalConfigMapper = configMapper;
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

        // 获取所有班级（含分科信息）
        List<ClassEntity> allClasses = classMapper.selectList(
                new LambdaQueryWrapper<ClassEntity>().eq(ClassEntity::getGrade, grade)
        );
        if (allClasses.isEmpty()) return ResponseResult.fail("没有班级信息");

        Map<Long, List<ClassEntity>> subjectGroupToClasses = allClasses.stream()
                .collect(Collectors.groupingBy(ClassEntity::getSubjectGroupId));

        // 所有课程 + "3+1+2"总分（100L）
        List<Long> courseIds = scoreMapper.selectDistinctCoursesByExamId(examId);
        courseIds.add(100L);

        // 所有学生
        List<Integer> allClassIds = allClasses.stream().map(ClassEntity::getId).toList();
        List<Student> allStudents = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().in(Student::getClassId, allClassIds)
        );
        if (allStudents.isEmpty()) return ResponseResult.fail("没有学生数据");

        Map<String, Student> studentMap = allStudents.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, s -> s));

        // 班级 ID -> 班级信息
        Map<Integer, ClassEntity> classIdMap = allClasses.stream()
                .collect(Collectors.toMap(ClassEntity::getId, c -> c));

        // 成绩：studentNumber -> courseId -> score
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

        // 构建 3+1+2 总分（courseId = 100）
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

        // 查询已有记录主键（一次性）
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
            List<Integer> classIds = classList.stream().map(ClassEntity::getId).toList();

            // 获取目标设置
            List<CriticalConfig> criticalConfigs = criticalConfigMapper.selectList(
                    new LambdaQueryWrapper<CriticalConfig>()
                            .eq(CriticalConfig::getGrade, grade)
                            .eq(CriticalConfig::getSubjectGroupId, subjectGroupId)
                            .eq(CriticalConfig::isDeleted, false)
            );
            Map<Integer, Integer> universityTargetMap = criticalConfigs.stream()
                    .collect(Collectors.toMap(CriticalConfig::getUniversityLevel, CriticalConfig::getTargetCount));

            // 当前分科学生
            List<Student> students = allStudents.stream()
                    .filter(s -> {
                        ClassEntity cls = classIdMap.get(s.getClassId());
                        return cls != null && cls.getSubjectGroupId().equals(subjectGroupId);
                    }).toList();

            Map<String, Student> sgStudentMap = students.stream()
                    .collect(Collectors.toMap(Student::getStudentNumber, s -> s));
            List<String> studentNumbers = new ArrayList<>(sgStudentMap.keySet());

            for (Long courseId : courseIds) {
                Map<String, Double> courseScoreMap = new HashMap<>();
                for (String sn : studentNumbers) {
                    double score = studentScores.getOrDefault(sn, Map.of()).getOrDefault(courseId, 0.0);
                    courseScoreMap.put(sn, score);
                }

                // 排序，取前 N 名
                List<Map.Entry<String, Double>> sorted = courseScoreMap.entrySet().stream()
                        .sorted(Map.Entry.<String, Double>comparingByValue().reversed())
                        .toList();

                for (ClassEntity cls : classList) {
                    List<String> classStudentNumbers = students.stream()
                            .filter(s -> s.getClassId().equals(cls.getId()))
                            .map(Student::getStudentNumber)
                            .toList();

                    double avgScore = classStudentNumbers.stream()
                            .mapToDouble(sn -> courseScoreMap.getOrDefault(sn, 0.0)).average().orElse(0.0);

                    for (Map.Entry<Integer, Integer> target : universityTargetMap.entrySet()) {
                        Integer level = target.getKey();
                        Integer targetCount = target.getValue();

                        long synergyCount = sorted.stream()
                                .limit(targetCount)
                                .filter(e -> classStudentNumbers.contains(e.getKey()))
                                .count();
                        double synergyRate = targetCount > 0 ? synergyCount * 1.0 / targetCount : 0.0;

                        ExamClassSubjectStat stat = new ExamClassSubjectStat();
                        stat.setExamId((long) examId);
                        stat.setCourseId(courseId);
                        stat.setClassId(cls.getId());
                        stat.setUniversityLevel(level);
                        stat.setAvgScore(avgScore);
                        stat.setSynergyCount((int) synergyCount);
                        stat.setSynergyRate(synergyRate);
                        stat.setContributionCount(0);
                        stat.setContributionRate(0.0);

                        String key = cls.getId() + "-" + courseId + "-" + level;
                        if (existingKeys.contains(key)) {
                            toUpdate.add(stat);
                        } else {
                            toInsert.add(stat);
                        }
                    }
                }
            }

            // 删除该分科缓存
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

        List<ClassEntity> classEntities = classMapper.selectList(
                new LambdaQueryWrapper<ClassEntity>()
                        .eq(ClassEntity::getGrade, exam.getGrade())
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
