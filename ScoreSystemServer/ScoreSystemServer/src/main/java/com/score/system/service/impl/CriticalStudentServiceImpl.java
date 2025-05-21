package com.score.system.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.CriticalConfig;
import com.score.system.entity.school.CriticalStudentLog;
import com.score.system.entity.school.Exam;
import com.score.system.entity.school.Score;
import com.score.system.entity.user.Student;
import com.score.system.mapper.*;
import com.score.system.service.CriticalStudentService;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

@Service
public class CriticalStudentServiceImpl implements CriticalStudentService {
    private final CriticalConfigMapper criticalConfigMapper;
    private final ScoreMapper scoreMapper;
    private final ExamMapper examMapper;
    private final StudentMapper studentMapper;
    private final CourseMapper courseMapper;
    private final CriticalStudentLogMapper criticalStudentLogMapper;
    private final CriticalConfigMapper configMapper;

    public CriticalStudentServiceImpl(CriticalConfigMapper criticalConfigMapper,
                                      ScoreMapper scoreMapper,
                                      ExamMapper examMapper,
                                      StudentMapper studentMapper,
                                      CourseMapper courseMapper,
                                      CriticalStudentLogMapper criticalStudentLogMapper, CriticalConfigMapper configMapper) {
        this.criticalConfigMapper = criticalConfigMapper;
        this.scoreMapper = scoreMapper;
        this.examMapper = examMapper;
        this.studentMapper = studentMapper;
        this.courseMapper = courseMapper;
        this.criticalStudentLogMapper = criticalStudentLogMapper;
        this.configMapper = configMapper;
    }

    @Override
    public ResponseResult<Boolean> addCriticalConfig(CriticalConfig criticalConfig) {
        // 1. 校验是否已存在相同配置
        boolean exists = configMapper.exists(
                new LambdaQueryWrapper<CriticalConfig>()
                        .eq(CriticalConfig::getGrade, criticalConfig.getGrade())
                        .eq(CriticalConfig::getYear, criticalConfig.getYear())
                        .eq(CriticalConfig::getSubjectGroupId, criticalConfig.getSubjectGroupId())
                        .eq(CriticalConfig::getUniversityLevel, criticalConfig.getUniversityLevel())
                        .eq(CriticalConfig::isDeleted, false)
        );

        if (exists) {
            return ResponseResult.fail("已存在相同配置，禁止重复添加");
        }

        // 2. 插入配置
        criticalConfig.setDeleted(false);

        configMapper.insert(criticalConfig);
        return ResponseResult.success("添加成功",true);
    }

    @Override
    @Transactional(rollbackFor = Exception.class)
    public ResponseResult<Boolean> batchAddCriticalConfig(List<CriticalConfig> criticalConfigList) {
        for (CriticalConfig criticalConfig : criticalConfigList) {
            ResponseResult<Boolean> result = addCriticalConfig(criticalConfig);
            if(result.getCode() != 200){
                throw new IllegalArgumentException(result.getMessage());
            }
        }
        return ResponseResult.success("批量设置成功",true);
    }

    @Override
    public ResponseResult<List<CriticalConfig>> getAllCriticalConfig() {
        return ResponseResult.success(configMapper.selectList(null));
    }

    public ResponseResult<String> generateCriticalStudents(int grade, int year) {
        // 获取当前配置
        List<CriticalConfig> configs = criticalConfigMapper.selectList(
                new LambdaQueryWrapper<CriticalConfig>()
                        .eq(CriticalConfig::getGrade, grade)
                        .eq(CriticalConfig::getYear, year)
                        .eq(CriticalConfig::isDeleted, false)
        );

        if (configs.isEmpty()) {
            return ResponseResult.fail("未配置临界学生统计设置");
        }

        // 获取所有考试
        List<Exam> exams = examMapper.selectList(new LambdaQueryWrapper<Exam>()
                .eq(Exam::getGrade, grade)
                .eq(Exam::getYear, year));

        if (exams.isEmpty()) {
            return ResponseResult.fail("该年级学年没有考试数据");
        }

        // 处理每次考试
        for (Exam exam : exams) {
            Long examId = exam.getId();
            List<Score> scores = scoreMapper.selectList(new LambdaQueryWrapper<Score>().eq(Score::getExamId, examId));

            Map<String, List<Score>> studentGrouped = scores.stream().collect(Collectors.groupingBy(Score::getStudentNumber));
            Map<String, Double> studentTotalMap = new HashMap<>();

            for (Map.Entry<String, List<Score>> entry : studentGrouped.entrySet()) {
                double total = entry.getValue().stream().mapToDouble(s -> s.getScore() == null ? 0.0 : s.getScore()).sum();
                studentTotalMap.put(entry.getKey(), total);
            }

            List<Map.Entry<String, Double>> sorted = studentTotalMap.entrySet().stream()
                    .sorted(Map.Entry.<String, Double>comparingByValue().reversed())
                    .toList();

            for (CriticalConfig config : configs) {
                int targetCount = config.getTargetCount();
                int criticalCount = (int) Math.ceil(targetCount * config.getCriticalRatio());

                int start = targetCount;
                int end = Math.min(start + criticalCount, sorted.size());

                for (int i = start; i < end; i++) {
                    Map.Entry<String, Double> entry = sorted.get(i);
                    String studentNumber = entry.getKey();
                    Double score = entry.getValue();
                    Student student = studentMapper.selectOne(new LambdaQueryWrapper<Student>().eq(Student::getStudentNumber, studentNumber));

                    CriticalStudentLog log = new CriticalStudentLog();
                    log.setExamId(examId);
                    log.setStudentNumber(studentNumber);
                    log.setStudentName(student != null ? student.getName() : null);
                    log.setUniversityLevel(config.getUniversityLevel());
                    log.setScoreRank(i + 1);
                    log.setTargetRank(targetCount);
                    log.setGap(i + 1 - targetCount);
                    log.setScore(score);
                    log.setSubjectGroupId(config.getSubjectGroupId() != null ? config.getSubjectGroupId().longValue() : null);

                    criticalStudentLogMapper.insert(log);
                }
            }
        }

        return ResponseResult.success("生成完成");
    }

    @Override
    public ResponseResult<List<CriticalStudentLog>> getAllCriticalStudentLog(int grade, int year) {
        // 1. 获取指定年级和年份的所有考试
        List<Exam> exams = examMapper.selectList(new LambdaQueryWrapper<Exam>()
                .eq(Exam::getGrade, grade)
                .eq(Exam::getYear, year));

        if (exams.isEmpty()) {
            return ResponseResult.fail("当前年级学年下没有考试数据");
        }

        // 2. 获取考试ID列表
        List<Long> examIds = exams.stream()
                .map(Exam::getId)
                .collect(Collectors.toList());

        // 3. 查询临界学生日志
        List<CriticalStudentLog> logs = criticalStudentLogMapper.selectList(
                new LambdaQueryWrapper<CriticalStudentLog>()
                        .in(CriticalStudentLog::getExamId, examIds)
        );

        return ResponseResult.success(logs);
    }
}
