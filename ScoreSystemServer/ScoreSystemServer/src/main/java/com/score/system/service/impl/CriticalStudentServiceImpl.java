package com.score.system.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.*;
import com.score.system.entity.user.Student;
import com.score.system.mapper.*;
import com.score.system.service.CriticalStudentService;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Objects;
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
    private final ClassMapper classMapper;

    public CriticalStudentServiceImpl(CriticalConfigMapper criticalConfigMapper,
                                      ScoreMapper scoreMapper,
                                      ExamMapper examMapper,
                                      StudentMapper studentMapper,
                                      CourseMapper courseMapper,
                                      CriticalStudentLogMapper criticalStudentLogMapper, CriticalConfigMapper configMapper, ClassMapper classMapper) {
        this.criticalConfigMapper = criticalConfigMapper;
        this.scoreMapper = scoreMapper;
        this.examMapper = examMapper;
        this.studentMapper = studentMapper;
        this.courseMapper = courseMapper;
        this.criticalStudentLogMapper = criticalStudentLogMapper;
        this.configMapper = configMapper;
        this.classMapper = classMapper;
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

        // 2. 查询符合条件的班级 ID
        List<ClassEntity> classList = classMapper.selectList(
                new LambdaQueryWrapper<ClassEntity>()
                        .eq(ClassEntity::getGrade, criticalConfig.getGrade())
                        .eq(ClassEntity::getSubjectGroupId, criticalConfig.getSubjectGroupId())
        );

        if (classList.isEmpty()) {
            return ResponseResult.fail("未找到符合条件的班级（年级 + 科目组）");
        }

        List<Integer> classIds = classList.stream()
                .map(c -> c.getId().intValue())
                .toList();

        // 3. 查询这些班级中的学生数量
        long groupStudentCount = studentMapper.selectCount(
                new LambdaQueryWrapper<Student>()
                        .in(Student::getClassId, classIds)
        );

        if (groupStudentCount == 0) {
            return ResponseResult.fail("该年级对应科目组下没有学生，无法计算比例");
        }

        // 4. 处理上浮 / 下浮人数规则
        if (criticalConfig.getUniversityLevel() == 1) {
            criticalConfig.setFloatUpCount(0); // 985 不需要上浮人数
        }
        if (criticalConfig.getUniversityLevel() == 4) {
            criticalConfig.setFloatDownCount(0); // 专科不需要下浮人数
        }

        int floatUp = criticalConfig.getFloatUpCount() != null ? criticalConfig.getFloatUpCount() : 0;
        int floatDown = criticalConfig.getFloatDownCount() != null ? criticalConfig.getFloatDownCount() : 0;
        int target = criticalConfig.getTargetCount() != null ? criticalConfig.getTargetCount() : 0;

        if (floatUp + floatDown > target) {
            return ResponseResult.fail("上浮人数 + 下浮人数不能大于目标人数（当前上浮：" + floatUp + "，下浮：" + floatDown + "，目标：" + target + "）");
        }

        // 5. 校验是否超出学生总数
        Long totalConfiguredCount = configMapper.selectList(
                        new LambdaQueryWrapper<CriticalConfig>()
                                .eq(CriticalConfig::getGrade, criticalConfig.getGrade())
                                .eq(CriticalConfig::getYear, criticalConfig.getYear())
                                .eq(CriticalConfig::getSubjectGroupId, criticalConfig.getSubjectGroupId())
                                .eq(CriticalConfig::isDeleted, false)
                ).stream()
                .mapToLong(c -> c.getTargetCount() != null ? c.getTargetCount() : 0)
                .sum();

        if (totalConfiguredCount + target > groupStudentCount) {
            return ResponseResult.fail("该年级年份科目组下总目标人数超出当前学生人数（当前已有：" +
                    totalConfiguredCount + "，新配置：" + target + "，总人数：" + groupStudentCount + "）");
        }

        // 6. 自动计算比例
        double ratio = target / (double) groupStudentCount;
        criticalConfig.setCriticalRatio(ratio);

        // 7. 插入
        criticalConfig.setDeleted(false);
        configMapper.insert(criticalConfig);

        return ResponseResult.success("添加成功", true);
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
    public ResponseResult<Boolean> updateCriticalConfig(CriticalConfig criticalConfig) {
        // 1. 查询原始配置是否存在
        CriticalConfig original = configMapper.selectById(criticalConfig.getId());
        if (original == null || original.isDeleted()) {
            return ResponseResult.fail("配置不存在或已被删除");
        }

        // 2. 查重：是否存在相同 grade + year + subjectGroupId 的其它配置
        long duplicateCount = configMapper.selectCount(
                new LambdaQueryWrapper<CriticalConfig>()
                        .eq(CriticalConfig::getGrade, criticalConfig.getGrade())
                        .eq(CriticalConfig::getYear, criticalConfig.getYear())
                        .eq(CriticalConfig::getSubjectGroupId, criticalConfig.getSubjectGroupId())
                        .eq(CriticalConfig::getUniversityLevel, criticalConfig.getUniversityLevel())
                        .eq(CriticalConfig::isDeleted, false)
                        .ne(CriticalConfig::getId, criticalConfig.getId()) // 排除当前这条记录
        );
        if (duplicateCount > 0) {
            return ResponseResult.fail("已存在相同年级、学年和选科组合的配置");
        }

        // 3. 查询对应班级和学生数量
        List<ClassEntity> classList = classMapper.selectList(
                new LambdaQueryWrapper<ClassEntity>()
                        .eq(ClassEntity::getGrade, criticalConfig.getGrade())
                        .eq(ClassEntity::getSubjectGroupId, criticalConfig.getSubjectGroupId())
        );
        if (classList.isEmpty()) return ResponseResult.fail("未找到对应班级");

        List<Integer> classIds = classList.stream().map(c -> c.getId().intValue()).toList();

        long groupStudentCount = studentMapper.selectCount(
                new LambdaQueryWrapper<Student>().in(Student::getClassId, classIds)
        );
        if (groupStudentCount == 0) return ResponseResult.fail("班级中无学生");

        // 4. 校验上下浮限制
        if (criticalConfig.getUniversityLevel() == 1) criticalConfig.setFloatUpCount(0);
        if (criticalConfig.getUniversityLevel() == 4) criticalConfig.setFloatDownCount(0);

        int floatUp = criticalConfig.getFloatUpCount() != null ? criticalConfig.getFloatUpCount() : 0;
        int floatDown = criticalConfig.getFloatDownCount() != null ? criticalConfig.getFloatDownCount() : 0;
        int target = criticalConfig.getTargetCount() != null ? criticalConfig.getTargetCount() : 0;

        if (floatUp + floatDown > target) {
            return ResponseResult.fail("上浮 + 下浮人数不能大于目标人数");
        }

        // 5. 校验总配置人数不超限（扣除原配置后再加上新配置）
        Long totalConfiguredCount = configMapper.selectList(
                        new LambdaQueryWrapper<CriticalConfig>()
                                .eq(CriticalConfig::getGrade, criticalConfig.getGrade())
                                .eq(CriticalConfig::getYear, criticalConfig.getYear())
                                .eq(CriticalConfig::getSubjectGroupId, criticalConfig.getSubjectGroupId())
                                .eq(CriticalConfig::isDeleted, false)
                ).stream()
                .mapToLong(c -> Objects.equals(c.getId(), criticalConfig.getId()) ? 0 : (c.getTargetCount() != null ? c.getTargetCount() : 0))
                .sum();

        if (totalConfiguredCount + target > groupStudentCount) {
            return ResponseResult.fail("总目标人数超出学生总数");
        }

        // 6. 设置比例
        double ratio = target / (double) groupStudentCount;
        criticalConfig.setCriticalRatio(ratio);

        // 7. 更新数据
        configMapper.updateById(criticalConfig);
        return ResponseResult.success("更新成功", true);
    }

    @Override
    @Transactional(rollbackFor = Exception.class)
    public ResponseResult<Boolean> batchUpdateCriticalConfig(List<CriticalConfig> criticalConfigList) {
        for (CriticalConfig config : criticalConfigList) {
            ResponseResult<Boolean> result = updateCriticalConfig(config);
            if (result.getCode() != 200) {
                throw new IllegalArgumentException("更新失败：[" + config.getUniversityLevel() + "]层次 - " + result.getMessage());
            }
        }
        return ResponseResult.success("全部更新成功", true);
    }

    @Override
    public ResponseResult<List<CriticalConfig>> getAllCriticalConfig() {
        return ResponseResult.success(configMapper.selectList(null));
    }

    @Override
    public ResponseResult<List<CriticalConfig>> getCriticalConfigByGrade(int examId) {
        Exam exam = examMapper.selectOne(
                new LambdaQueryWrapper<Exam>().eq(Exam::getGrade, examId)
        );
        if(exam == null){
            return ResponseResult.fail("未查找到对应信息");
        }
        // 查询指定年级的临界生配置，排除被逻辑删除的记录（is_deleted = 0）
        List<CriticalConfig> configList = criticalConfigMapper.selectList(
                new LambdaQueryWrapper<CriticalConfig>()
                        .eq(CriticalConfig::getGrade, exam.getGrade())
                        .eq(CriticalConfig::isDeleted, false)
        );

        if (configList.isEmpty()) {
            return ResponseResult.fail("该年级暂无临界生配置");
        }

        return ResponseResult.success("查询成功", configList);
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

    @Override
    public ResponseResult<List<CriticalStudentLog>> getAllCriticalStudentByGradeAndYear(int grade, int year,int examId) {
        return ResponseResult.fail("null");
    }
}
