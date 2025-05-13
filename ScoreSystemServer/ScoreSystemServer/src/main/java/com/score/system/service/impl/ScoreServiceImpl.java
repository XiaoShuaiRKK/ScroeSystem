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
        if (student == null) throw new RuntimeException("学生不存在");

        ClassEntity clazz = classMapper.selectById(student.getClassId());
        if (clazz == null) throw new RuntimeException("班级不存在");

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
