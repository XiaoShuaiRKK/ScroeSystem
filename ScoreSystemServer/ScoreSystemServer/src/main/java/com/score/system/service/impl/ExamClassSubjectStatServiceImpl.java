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

        // 查询该年级该年份所有学生历史，得到所有班级ID
        List<StudentClassHistory> histories = historyMapper.selectList(
                new LambdaQueryWrapper<StudentClassHistory>()
                        .eq(StudentClassHistory::getGrade, grade)
                        .eq(StudentClassHistory::getYear, exam.getYear())
        );
        List<Long> cIds = histories.stream().map(StudentClassHistory::getClassId).toList();
        List<ClassEntity> allClasses = classMapper.selectList(
                new LambdaQueryWrapper<ClassEntity>().in(ClassEntity::getId, cIds)
        );
        if (allClasses.isEmpty()) return ResponseResult.fail("没有班级信息");

        // 按学科组分班级
        Map<Long, List<ClassEntity>> subjectGroupToClasses = allClasses.stream()
                .collect(Collectors.groupingBy(ClassEntity::getSubjectGroupId));

        // 获取考试中所有科目ID，额外加100L表示3+1+2总分
        List<Long> courseIds = scoreMapper.selectDistinctCoursesByExamId(examId);
        courseIds.add(100L);

        List<Integer> allClassIds = allClasses.stream().map(ClassEntity::getId).toList();

        // 获取所有该年级该考试相关学生
        List<Student> allStudents = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().in(Student::getClassId, allClassIds)
        );
        if (allStudents.isEmpty()) return ResponseResult.fail("没有学生数据");

        Map<String, Student> studentMap = allStudents.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, s -> s));
        Map<Integer, ClassEntity> classIdMap = allClasses.stream()
                .collect(Collectors.toMap(ClassEntity::getId, c -> c));

        // 查询所有成绩
        List<Score> allScores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>().eq(Score::getExamId, examId)
        );
        if (allScores.isEmpty()) return ResponseResult.fail("没有成绩数据");

        // 组装学生成绩映射：学号 -> (课程ID -> 成绩)
        Map<String, Map<Long, Double>> studentScores = new HashMap<>();
        for (Score score : allScores) {
            studentScores
                    .computeIfAbsent(score.getStudentNumber(), k -> new HashMap<>())
                    .put(score.getCourseId(), score.getScore() != null ? score.getScore() : 0.0);
        }

        // 计算3+1+2总分 (课程ID=100L)
        for (Student student : allStudents) {
            String sn = student.getStudentNumber();
            Map<Long, Double> scores = studentScores.getOrDefault(sn, Map.of());

            List<Long> core = List.of(1L, 2L, 3L); // 核心三科
            double three = core.stream().mapToDouble(cid -> scores.getOrDefault(cid, 0.0)).sum();

            // 选科科目成绩排序取前两科（排除核心三科）
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

        // 查询已存在统计，避免重复插入
        List<ExamClassSubjectStat> existingStats = examClassSubjectStatMapper.selectList(
                new LambdaQueryWrapper<ExamClassSubjectStat>().eq(ExamClassSubjectStat::getExamId, examId)
        );
        Set<String> existingKeys = existingStats.stream()
                .map(stat -> stat.getClassId() + "-" + stat.getCourseId() + "-" + stat.getUniversityLevel())
                .collect(Collectors.toSet());

        List<ExamClassSubjectStat> toInsert = new ArrayList<>();
        List<ExamClassSubjectStat> toUpdate = new ArrayList<>();

        // 遍历各学科组班级进行统计
        for (Map.Entry<Long, List<ClassEntity>> entry : subjectGroupToClasses.entrySet()) {
            Long subjectGroupId = entry.getKey();
            List<ClassEntity> classList = entry.getValue();

            // 当前学科组对应年级的目标人数配置（九八五等）
            List<CriticalConfig> criticalConfigs = criticalConfigMapper.selectList(
                    new LambdaQueryWrapper<CriticalConfig>()
                            .eq(CriticalConfig::getGrade, grade)
                            .eq(CriticalConfig::getSubjectGroupId, subjectGroupId)
                            .eq(CriticalConfig::isDeleted, false)
            );
            Map<Integer, Integer> universityTargetMap = criticalConfigs.stream()
                    .collect(Collectors.toMap(CriticalConfig::getUniversityLevel, CriticalConfig::getTargetCount));

            // 当前学科组的学生
            List<Student> students = allStudents.stream()
                    .filter(s -> {
                        ClassEntity cls = classIdMap.get(s.getClassId());
                        return cls != null && cls.getSubjectGroupId().equals(subjectGroupId);
                    }).toList();

            Map<String, Student> sgStudentMap = students.stream()
                    .collect(Collectors.toMap(Student::getStudentNumber, s -> s));
            List<String> studentNumbers = new ArrayList<>(sgStudentMap.keySet());

            // 学生3+1+2总分映射
            Map<String, Double> totalScoreMap = new HashMap<>();
            for (String sn : studentNumbers) {
                double total = studentScores.getOrDefault(sn, Map.of()).getOrDefault(100L, 0.0);
                totalScoreMap.put(sn, total);
            }

            // 3+1+2总分降序排序（排名）
            List<Map.Entry<String, Double>> totalSorted = totalScoreMap.entrySet().stream()
                    .sorted(Map.Entry.<String, Double>comparingByValue().reversed())
                    .toList();

            for (Long courseId : courseIds) {
                // 单科成绩映射
                Map<String, Double> courseScoreMap = new HashMap<>();
                for (String sn : studentNumbers) {
                    double score = studentScores.getOrDefault(sn, Map.of()).getOrDefault(courseId, 0.0);
                    courseScoreMap.put(sn, score);
                }

                for (ClassEntity cls : classList) {
                    // 班级学生学号
                    List<String> classStudentNumbers = students.stream()
                            .filter(s -> s.getClassId().equals(cls.getId()))
                            .map(Student::getStudentNumber)
                            .toList();

                    // 该班该科平均成绩
                    double avgScore = classStudentNumbers.stream()
                            .mapToDouble(sn -> courseScoreMap.getOrDefault(sn, 0.0)).average().orElse(0.0);

                    // 遍历各大学等级配置
                    for (Map.Entry<Integer, Integer> target : universityTargetMap.entrySet()) {
                        Integer level = target.getKey();
                        Integer targetCount = target.getValue();

                        // 班级中达到3+1+2总分目标线人数（协同人数）
                        long synergyCount = totalSorted.stream()
                                .limit(targetCount)
                                .filter(e -> classStudentNumbers.contains(e.getKey()))
                                .count();
                        double synergyRate = targetCount > 0 ? synergyCount * 1.0 / targetCount : 0.0;

                        // 3+1+2目标分数线（贡献线）
                        double cutoffScore = totalSorted.size() >= targetCount ? totalSorted.get(targetCount - 1).getValue() : -1;

                        // 班级中达到3+1+2贡献线的学生集合
                        Set<String> contributionStudentSet = totalSorted.stream()
                                .filter(e -> classStudentNumbers.contains(e.getKey()))
                                .filter(e -> e.getValue() >= cutoffScore)
                                .map(Map.Entry::getKey)
                                .collect(Collectors.toSet());

                        int contributionCount = 0;
                        double contributionRate = 0.0;

                        if (courseId == 100L) {
                            // 3+1+2贡献率 = 班级达到贡献线人数 / 配置目标人数
                            contributionCount = contributionStudentSet.size();
                            contributionRate = targetCount > 0 ? (double) contributionCount / targetCount : 0.0;

                        } else {
                            // 其他单科贡献率 = 贡献学生集合中，单科达到贡献线的人数占比
                            if (!contributionStudentSet.isEmpty()) {
                                List<Map.Entry<String, Double>> subjectSorted = classStudentNumbers.stream()
                                        .map(sn -> Map.entry(sn, courseScoreMap.getOrDefault(sn, 0.0)))
                                        .sorted(Map.Entry.<String, Double>comparingByValue().reversed())
                                        .toList();

                                double subjectCutoff = subjectSorted.size() >= contributionStudentSet.size()
                                        ? subjectSorted.get(contributionStudentSet.size() - 1).getValue()
                                        : -1;

                                contributionCount = (int) subjectSorted.stream()
                                        .filter(e -> e.getValue() >= subjectCutoff)
                                        .filter(e -> contributionStudentSet.contains(e.getKey()))
                                        .count();

                                contributionRate = contributionStudentSet.size() > 0
                                        ? contributionCount * 1.0 / contributionStudentSet.size()
                                        : 0.0;
                            }
                        }

                        // 构造统计实体
                        ExamClassSubjectStat stat = new ExamClassSubjectStat();
                        stat.setExamId((long) examId);
                        stat.setCourseId(courseId);
                        stat.setClassId(cls.getId());
                        stat.setUniversityLevel(level);
                        stat.setAvgScore(avgScore);
                        stat.setSynergyCount((int) synergyCount);
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

            // 清理缓存
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
