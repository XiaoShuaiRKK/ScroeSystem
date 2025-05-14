package com.score.system.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.baomidou.mybatisplus.core.conditions.query.QueryWrapper;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.*;
import com.score.system.entity.user.Student;
import com.score.system.entity.user.StudentSubjectSelection;
import com.score.system.mapper.*;
import com.score.system.service.ScoreService;
import com.score.system.util.RedisUtil;
import org.apache.ibatis.annotations.Param;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.TimeUnit;
import java.util.stream.Collectors;

@Service
public class ScoreServiceImpl implements ScoreService {
    private final ScoreMapper scoreMapper;
    private final StudentMapper studentMapper;
    private final CourseMapper courseMapper;
    private final ExamMapper examMapper;
    private final StudentSubjectSelectionMapper studentSubjectSelectionMapper;
    private final RedisUtil redisUtil;
    private final ClassMapper classMapper;

    public ScoreServiceImpl(ScoreMapper scoreMapper, StudentMapper studentMapper, CourseMapper courseMapper, ExamMapper examMapper, StudentSubjectSelectionMapper studentSubjectSelectionMapper, RedisUtil redisUtil, ClassMapper classMapper) {
        this.scoreMapper = scoreMapper;
        this.studentMapper = studentMapper;
        this.courseMapper = courseMapper;
        this.examMapper = examMapper;
        this.studentSubjectSelectionMapper = studentSubjectSelectionMapper;
        this.redisUtil = redisUtil;
        this.classMapper = classMapper;
    }

    @Override
    @Transactional
    public ResponseResult<Boolean> addScore(Score score) {
        if(studentMapper.selectStudentByNumber(score.getStudentNumber()) == null){
            return ResponseResult.fail("学号： " + score.getStudentNumber() + " 不存在");
        }
        if(courseMapper.selectById(score.getCourseId()) == null){
            return ResponseResult.fail("科目： " + score.getCourseId() + " 不存在");
        }
        if(examMapper.selectById(score.getExamId()) == null){
            return ResponseResult.fail("考试不存在: " + score.getExamId());
        }
        LambdaQueryWrapper<StudentSubjectSelection> query = new LambdaQueryWrapper<>();
        query.eq(StudentSubjectSelection::getStudentNumber, score.getStudentNumber());
        StudentSubjectSelection selection = studentSubjectSelectionMapper.selectOne(query);
        if(selection == null){
            return ResponseResult.fail("学生还未设置分科: " + score.getStudentNumber());
        }
        // 查看学生是否选择了课程
        Long courseId = score.getCourseId();
        List<Long> mustCourseId = new ArrayList<>();
        mustCourseId.add(1L);
        mustCourseId.add(2L);
        mustCourseId.add(3L);
        mustCourseId.add(selection.getElectiveCourse1Id());
        mustCourseId.add(selection.getElectiveCourse2Id());
        if(selection.getSubjectGroupId() == 2){
            mustCourseId.add(5L);
        } else if (selection.getSubjectGroupId() == 3) {
            mustCourseId.add(4L);
        }
        boolean isSelected = false;
        for(Long l : mustCourseId){
            if (courseId.equals(l)) {
                isSelected = true;
                break;
            }
        }
        if(!isSelected){
            return ResponseResult.fail("学生:" + score.getStudentNumber() + "未选择该课程，无法录入成绩");
        }
        if(score.getScore() < 0){
            return ResponseResult.fail("学生:" + score.getStudentNumber() + "|courseId:" + score.getCourseId()
                    + "|score:" + score.getScore() + " 请输入正确的成绩范围");
        }
        if(courseId >= 1 && courseId <= 3){
            if(score.getScore() > 150){
                return ResponseResult.fail("请输入正确的成绩范围");
            }
        }else{
            if(score.getScore() > 100){
                return ResponseResult.fail("请输入正确的成绩范围");
            }
        }
        int result = scoreMapper.insert(score);
        if(result <= 0){
            return ResponseResult.error("成绩添加失败");
        }
        return ResponseResult.success("成绩添加成功", true);
    }

    @Override
    @Transactional(rollbackFor = Exception.class)
    public ResponseResult<Boolean> batchAddScores(List<Score> scores) {
        try {
            for(Score score : scores){
                ResponseResult<Boolean> result = addScore(score);
                if(result.getCode() == 400){
                    throw new IllegalArgumentException(result.getMessage());
                }else if (result.getCode() == 500){
                    throw new RuntimeException(result.getMessage());
                }
            }
        }catch (IllegalArgumentException e){
            return ResponseResult.fail(e.getMessage());
        }
        return ResponseResult.success("批量添加成功",true);
    }

    @Override
    public ResponseResult<List<ClassRankingDTO>> calculateClassRankings(String studentNumber, Long examId) {
        Student student = studentMapper.selectStudentByNumber(studentNumber);
        if(student == null){
            return ResponseResult.fail("学生不存在：" + studentNumber);
        }
        Exam exam = examMapper.selectById(examId);
        if(exam == null){
            return ResponseResult.fail("考试不存在: " + examId);
        }
        //获取学生的成绩
        List<Score> studentScore = scoreMapper.selectByExamAndStudentNumber(examId, studentNumber);
        if(studentScore.isEmpty()){
            return ResponseResult.fail("该学生" + studentNumber + "在该次考试中没有成绩记录");
        }
        List<ClassRankingDTO> resultList = new ArrayList<>();
        for(Score score : studentScore){
            Long courseId = score.getCourseId();
            Integer classId = student.getClassId();
            String redisKey = "classRank:" + examId + ":" + courseId + ":" + classId;
            //如果缓存不存在, 构建排名并缓存
            if(!redisUtil.exists(redisKey)){
                List<Score> classScores = scoreMapper.selectByExamCourseAndClassId(examId, courseId, classId);
                for(Score s : classScores){
                    redisUtil.getRedisTemplate().opsForZSet()
                            .add(redisKey, s.getStudentNumber(), s.getScore());
                }
                redisUtil.getRedisTemplate().expire(redisKey, 30 , TimeUnit.MINUTES);
            }
            //获取当前学生的排名(倒序，即高分在前)
            Long rank = redisUtil.getRedisTemplate().opsForZSet().reverseRank(redisKey, studentNumber);
            //获取总人数
            Long total = redisUtil.getRedisTemplate().opsForZSet().zCard(redisKey);
            //获取分数
            Double redisScore = redisUtil.getRedisTemplate().opsForZSet().score(redisKey,studentScore);
            ClassRankingDTO dto = new ClassRankingDTO();
            dto.setCourseId(courseId);
            dto.setScore(score.getScore());
            dto.setTotalInClass(total != null ? total : 0L);
            dto.setRank(rank != null ? rank.intValue() + 1 : null);
            resultList.add(dto);
        }
        return ResponseResult.success("班级排名获取成功", resultList);
    }

    @Override
    public ResponseResult<List<GradeRankingDTO>> calculateGradeRankings(String studentNumber, Long examId) {
        Student student = studentMapper.selectStudentByNumber(studentNumber);
        if (student == null) return ResponseResult.fail("学生不存在: " + studentNumber);

        ClassEntity clazz = classMapper.selectById(student.getClassId());
        if (clazz == null) return ResponseResult.fail("班级不存在: " + student.getClassId());

        List<Score> studentScores = scoreMapper.selectList(
                new QueryWrapper<Score>().eq("exam_id", examId).eq("student_number", studentNumber));

        List<GradeRankingDTO> result = new ArrayList<>();

        for (Score score : studentScores) {
            String redisKey = buildRedisKey(score.getExamId(), score.getCourseId(), clazz.getSubjectGroupId());

            // 如果缓存不存在则自动构建
            if (!redisUtil.exists(redisKey)) {
                buildGradeRankingToRedis(examId);
            }

            Long rank = redisUtil.zrevrank(redisKey, studentNumber);
            Long total = redisUtil.zcount(redisKey);

            if (rank != null && total != null) {
                int rankInt = rank.intValue();
                GradeRankingDTO dto = new GradeRankingDTO();
                dto.setStudentNumber(studentNumber);
                dto.setCourseId(score.getCourseId());
                dto.setScore(score.getScore());
                dto.setGradeRank(rankInt + 1);
                dto.setTotalStudents(total.intValue());
                result.add(dto);
            }
        }

        return ResponseResult.success(result);
    }

    @Override
    public ResponseResult<ClassRankingDTO> calculateTotalScoreClassRankings(String studentNumber, Long examId) {
        Student student = studentMapper.selectStudentByNumber(studentNumber);
        if (student == null) return ResponseResult.fail("学生不存在: " + studentNumber);
        int classId = student.getClassId();
        StudentSubjectSelection selection = studentSubjectSelectionMapper.selectOne(
                new LambdaQueryWrapper<StudentSubjectSelection>()
                        .eq(StudentSubjectSelection::getStudentNumber, studentNumber)
        );
        if(selection == null) return ResponseResult.fail("未找到该学生的选科信息 : " + studentNumber);
        //需要计算的课程ID
        List<Long> requiredCourseIds = new ArrayList<>();
        requiredCourseIds.add(1L);
        requiredCourseIds.add(2L);
        requiredCourseIds.add(3L);
        if(selection.getSubjectGroupId() == 2){
            requiredCourseIds.add(5L);
        }else if(selection.getSubjectGroupId() == 3){
            requiredCourseIds.add(4L);
        }
        requiredCourseIds.add(selection.getElectiveCourse1Id());
        requiredCourseIds.add(selection.getElectiveCourse2Id());
        //查出该班所有人的成绩
        List<Student> classStudents = studentMapper.selectList(
                new LambdaQueryWrapper<Student>()
                        .eq(Student::getClassId, classId)
        );
        List<String> studentNumbers = classStudents.stream().map(Student::getStudentNumber).toList();
        //查找所有分数
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
                        .in(Score::getStudentNumber, studentNumbers)
                        .in(Score::getCourseId, requiredCourseIds)
        );
        //分组求总分
        Map<String, Double> totalScoreMap = new HashMap<>();
        for(Score score : scores){
            if(score.getScore() == null) continue;
            totalScoreMap.merge(score.getStudentNumber(), score.getScore(), Double::sum);
        }
        String redisKey = "classTotalScore:" + examId + ":" + classId;
        if(!redisUtil.exists(redisKey)){
            redisUtil.delete(redisKey);
            for(Map.Entry<String, Double> entry : totalScoreMap.entrySet()){
                redisUtil.zadd(redisKey, entry.getValue(), entry.getKey());
            }
            redisUtil.expire(redisKey, 30, TimeUnit.MINUTES);
        }
        Long rank = redisUtil.zrevrank(redisKey, studentNumber);
        Long total = redisUtil.zcount(redisKey);
        Double score = redisUtil.zscore(redisKey, studentNumber);
        ClassRankingDTO dto = new ClassRankingDTO();
        dto.setCourseId(0L);
        dto.setScore(score);
        dto.setTotalInClass(total != null ? total : 0L);
        dto.setRank(rank != null ? rank.intValue() + 1 : null);
        return ResponseResult.success("总分班级排名获取成功", dto);
    }

    @Override
    public ResponseResult<GradeRankingDTO> calculateTotalScoreGradeRankings(String studentNumber, Long examId) {
        Student student = studentMapper.selectStudentByNumber(studentNumber);
        if (student == null) return ResponseResult.fail("学生不存在: " + studentNumber);
        ClassEntity classEntity = classMapper.selectById(student.getClassId());
        if(classEntity == null) return ResponseResult.fail("班级不存在: " + student.getClassId());
        Long subjectGroupId = classEntity.getSubjectGroupId();
        String redisKey = "gradeTotalScore:" + examId + ":" + subjectGroupId;
        //检查redis是否存在缓存
        if(!redisUtil.exists(redisKey)){
            List<Student> allStudent = studentMapper.selectList(null);
            Map<String, Long> studentGroupMap = allStudent.stream()
                    .collect(Collectors.toMap(Student::getStudentNumber, s -> {
                        ClassEntity c = classMapper.selectById(s.getClassId());
                        return c != null ? c.getSubjectGroupId() : null;
                    }));
            List<Score> scores = scoreMapper.selectList(
                    new LambdaQueryWrapper<Score>()
                            .eq(Score::getExamId, examId)
            );
            //计算总分并按 subjectGroupId 分类
            Map<String,Double> totalScoreMap = new HashMap<>();
            for(Score score : scores){
                String sNum = score.getStudentNumber();
                Long groupId = studentGroupMap.get(sNum);
                if(groupId != null && groupId.equals(subjectGroupId) && score.getScore() != null){
                    totalScoreMap.merge(sNum, score.getScore(), Double::sum);
                }
            }
            //缓存进redis
            redisUtil.delete(redisKey);
            for (Map.Entry<String, Double> entry : totalScoreMap.entrySet()){
                redisUtil.zadd(redisKey, entry.getValue(), entry.getKey());
            }
            redisUtil.expire(redisKey, 30, TimeUnit.MINUTES);
        }
        //获取当前学生信息
        Long rank = redisUtil.zrevrank(redisKey, studentNumber);
        Long total = redisUtil.zcount(redisKey);
        Double totalScore = redisUtil.zscore(redisKey, studentNumber);
        GradeRankingDTO dto = new GradeRankingDTO();
        dto.setStudentNumber(studentNumber);
        dto.setCourseId(0L);
        dto.setScore(totalScore != null ? totalScore : 0.0);
        dto.setGradeRank(rank != null ? rank.intValue() + 1 : 0);
        dto.setTotalStudents(total != null ? total.intValue() : 0);

        return ResponseResult.success("总分年级排名获取成功", dto);
    }


    private String buildRedisKey(Long examId,Long courseId,Long subjectGroupId){
        return String.format("rank:%d:%d:%d", examId, courseId, subjectGroupId);
    }

    private void buildGradeRankingToRedis(Long examId){
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
        );
        if(scores.isEmpty()) return;
        Map<Long, ClassEntity> classMap = classMapper.selectList(null)
                .stream().collect(Collectors.toMap(ClassEntity::getId, c -> c));
        Map<String, Student> studentMap = studentMapper.selectList(null)
                .stream().collect(Collectors.toMap(Student::getStudentNumber, s -> s));

        Map<String, List<Score>> grouped = new HashMap<>();

        for (Score score : scores) {
            Student student = studentMap.get(score.getStudentNumber());
            if (student == null || score.getScore() == null) continue;
            ClassEntity clazz = classMap.get(Long.valueOf(student.getClassId()));
            if (clazz == null) continue;

            String redisKey = buildRedisKey(score.getExamId(), score.getCourseId(), clazz.getSubjectGroupId());
            grouped.computeIfAbsent(redisKey, k -> new ArrayList<>()).add(score);
        }

        for (Map.Entry<String, List<Score>> entry : grouped.entrySet()) {
            String redisKey = entry.getKey();
            redisUtil.delete(redisKey); // 清理旧缓存
            for (Score score : entry.getValue()) {
                redisUtil.zadd(redisKey, score.getScore(), score.getStudentNumber());
            }
        }
    }
}
