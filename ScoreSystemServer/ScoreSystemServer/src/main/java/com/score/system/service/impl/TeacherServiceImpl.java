package com.score.system.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.ClassEntity;
import com.score.system.entity.school.Score;
import com.score.system.entity.user.Student;
import com.score.system.entity.user.StudentConverter;
import com.score.system.entity.user.StudentDTO;
import com.score.system.entity.user.StudentScoreVO;
import com.score.system.mapper.ClassMapper;
import com.score.system.mapper.ScoreMapper;
import com.score.system.mapper.StudentMapper;
import com.score.system.mapper.TeacherMapper;
import com.score.system.service.TeacherService;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;

@Service
public class TeacherServiceImpl implements TeacherService {
    private final StudentMapper studentMapper;
    private final TeacherMapper teacherMapper;
    private final ClassMapper classMapper;
    private final ScoreMapper scoreMapper;

    public TeacherServiceImpl(StudentMapper studentMapper, TeacherMapper teacherMapper, ClassMapper classMapper, ScoreMapper scoreMapper) {
        this.studentMapper = studentMapper;
        this.teacherMapper = teacherMapper;
        this.classMapper = classMapper;
        this.scoreMapper = scoreMapper;
    }

    @Override
    public ResponseResult<List<StudentDTO>> selectStudentsByClass(Long teacherId) {
        LambdaQueryWrapper<ClassEntity> classEntityLambdaQueryWrapper = new LambdaQueryWrapper<>();
        classEntityLambdaQueryWrapper.eq(ClassEntity::getHeadTeacherId, teacherId);
        ClassEntity classEntity = classMapper.selectOne(classEntityLambdaQueryWrapper);
        if(classEntity == null) {
            return ResponseResult.fail("该老师不是班主任", null);
        }
        LambdaQueryWrapper<Student> studentLambdaQueryWrapper = new LambdaQueryWrapper<>();
        studentLambdaQueryWrapper.eq(Student::getClassId, classEntity.getId());
        List<Student> students = studentMapper.selectList(studentLambdaQueryWrapper);
        List<StudentDTO> studentDTOS = students.stream().map(student -> StudentConverter.toDTO(student)).toList();
        return ResponseResult.success(studentDTOS);
    }

    @Override
    public ResponseResult<StudentDTO> selectStudentByTeacherId(Long teacherId, String studentNumber) {
        LambdaQueryWrapper<Student> studentLambdaQueryWrapper = new LambdaQueryWrapper<>();
        studentLambdaQueryWrapper.eq(Student::getStudentNumber, studentNumber);
        Student student = studentMapper.selectOne(studentLambdaQueryWrapper);
        StudentDTO studentDTO = StudentConverter.toDTO(student);
        return studentDTO == null ? ResponseResult.fail("该学号不存在", null) : ResponseResult.success(studentDTO);
    }

    @Override
    public ResponseResult<List<StudentScoreVO>> selectStudentScoreByTeacherId(Long teacherId,Long classId, Long examId) {
        ClassEntity classEntity = classMapper.selectById(classId);
        if(classEntity == null) {
            return ResponseResult.fail("班级不存在 class id: " + classId);
        }
        LambdaQueryWrapper<Student> studentLambdaQueryWrapper = new LambdaQueryWrapper<>();
        studentLambdaQueryWrapper.eq(Student::getClassId, classId);
        List<Student> students = studentMapper.selectList(studentLambdaQueryWrapper);
        List<StudentScoreVO> studentScoreVOS = new ArrayList<>();
        for(Student student : students) {
            LambdaQueryWrapper<Score> scoreLambdaQueryWrapper = new LambdaQueryWrapper<>();
            scoreLambdaQueryWrapper.eq(Score::getStudentNumber, student.getStudentNumber());
            List<Score> scores = scoreMapper.selectList(scoreLambdaQueryWrapper);
            studentScoreVOS.add(StudentConverter.toScoreVO(student, classEntity.getName(), scores));
        }
        return ResponseResult.success(studentScoreVOS);
    }

    @Override
    public ResponseResult<StudentScoreVO> selectStudentScoreByStudentId(Long studentId, String studentNumber, Long examId) {
        LambdaQueryWrapper<Student> studentLambdaQueryWrapper = new LambdaQueryWrapper<>();
        studentLambdaQueryWrapper.eq(Student::getStudentNumber, studentNumber);
        Student student = studentMapper.selectOne(studentLambdaQueryWrapper);
        ClassEntity classEntity = classMapper.selectById(student.getClassId());
        LambdaQueryWrapper<Score> scoreLambdaQueryWrapper = new LambdaQueryWrapper<>();
        scoreLambdaQueryWrapper.eq(Score::getStudentNumber, student.getStudentNumber());
        List<Score> scores = scoreMapper.selectList(scoreLambdaQueryWrapper);
        StudentScoreVO studentScoreVO = StudentConverter.toScoreVO(student, classEntity.getName(), scores);
        return ResponseResult.success(studentScoreVO);
    }
}
