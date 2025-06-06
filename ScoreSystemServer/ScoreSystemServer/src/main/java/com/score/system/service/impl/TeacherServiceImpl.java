package com.score.system.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.ClassEntity;
import com.score.system.entity.school.Score;
import com.score.system.entity.user.*;
import com.score.system.mapper.*;
import com.score.system.service.TeacherService;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

@Service
public class TeacherServiceImpl implements TeacherService {
    private final StudentMapper studentMapper;
    private final TeacherMapper teacherMapper;
    private final ClassMapper classMapper;
    private final ScoreMapper scoreMapper;
    private final StudentSubjectSelectionMapper studentSubjectSelectionMapper;
    private final StudentClassHistoryMapper studentClassHistoryMapper;

    public TeacherServiceImpl(StudentMapper studentMapper, TeacherMapper teacherMapper, ClassMapper classMapper, ScoreMapper scoreMapper, StudentSubjectSelectionMapper studentSubjectSelectionMapper, StudentClassHistoryMapper studentClassHistoryMapper) {
        this.studentMapper = studentMapper;
        this.teacherMapper = teacherMapper;
        this.classMapper = classMapper;
        this.scoreMapper = scoreMapper;
        this.studentSubjectSelectionMapper = studentSubjectSelectionMapper;
        this.studentClassHistoryMapper = studentClassHistoryMapper;
    }

    @Override
    public ResponseResult<List<StudentVO>> selectStudentsByClass(Long classId) {
        ClassEntity classEntity = classMapper.selectById(classId);
        if (classEntity == null) {
            return ResponseResult.fail("班级不存在");
        }

        List<Student> students = studentMapper.selectList(
                new LambdaQueryWrapper<Student>()
                        .eq(Student::getClassId, classEntity.getId())
        );
        if (students.isEmpty()) {
            return ResponseResult.success("该班级暂无学生", new ArrayList<>());
        }

        List<String> studentNumbers = students.stream()
                .map(Student::getStudentNumber)
                .collect(Collectors.toList());

        List<StudentSubjectSelection> selections = studentSubjectSelectionMapper.selectList(
                new LambdaQueryWrapper<StudentSubjectSelection>()
                        .in(StudentSubjectSelection::getStudentNumber, studentNumbers)
        );

        Map<String, StudentSubjectSelection> selectionMap = selections.stream()
                .collect(Collectors.toMap(StudentSubjectSelection::getStudentNumber, s -> s));

        List<StudentVO> studentVOList = students.stream()
                .map(s -> StudentConverter.toVO(s, selectionMap.get(s.getStudentNumber())))
                .collect(Collectors.toList());

        return ResponseResult.success("查询成功", studentVOList);
    }

    @Override
    public ResponseResult<StudentVO> selectStudentByTeacherId(String studentNumber) {
        LambdaQueryWrapper<Student> studentLambdaQueryWrapper = new LambdaQueryWrapper<>();
        studentLambdaQueryWrapper.eq(Student::getStudentNumber, studentNumber);
        Student student = studentMapper.selectOne(studentLambdaQueryWrapper);
        if(student == null){
            return ResponseResult.fail("该学号不存在", null);
        }
        StudentSubjectSelection selection = studentSubjectSelectionMapper.selectOne(
                new LambdaQueryWrapper<StudentSubjectSelection>()
                        .eq(StudentSubjectSelection::getStudentNumber, student.getStudentNumber())
        );
        StudentVO studentVO = StudentConverter.toVO(student, selection);
        return ResponseResult.success(studentVO);
    }

    @Override
    public ResponseResult<List<StudentScoreVO>> selectStudentScoreByTeacherId(Long classId, Long examId) {
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
            scoreLambdaQueryWrapper.eq(Score::getStudentNumber, student.getStudentNumber()).eq(Score::getExamId, examId);
            List<Score> scores = scoreMapper.selectList(scoreLambdaQueryWrapper);
            studentScoreVOS.add(StudentConverter.toScoreVO(student, classEntity.getName(), scores));
        }
        return ResponseResult.success(studentScoreVOS);
    }

    @Override
    public ResponseResult<StudentScoreVO> selectStudentScoreByStudentId(String studentNumber, Long examId) {
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

    @Override
    public ResponseResult<List<StudentClassHistory>> selectStudentByStudentNumber(String studentNumber) {
        if(studentNumber == null || studentNumber.trim().isEmpty()){
            return ResponseResult.fail("学号不能为空");
        }
        LambdaQueryWrapper<StudentClassHistory> query = new LambdaQueryWrapper<StudentClassHistory>().eq(StudentClassHistory::getStudentNumber, studentNumber);
        return ResponseResult.success(studentClassHistoryMapper.selectList(query));
    }

    @Override
    public ResponseResult<List<StudentVO>> selectStudentNumberByName(String studentName) {
        if(studentName == null || studentName.trim().isEmpty()){
            return ResponseResult.fail("名字不能为空");
        }
        LambdaQueryWrapper<Student> eq = new LambdaQueryWrapper<Student>().eq(Student::getName, studentName);
        List<Student> students = studentMapper.selectList(eq);
        if(students.isEmpty()){
            return ResponseResult.fail("未找到学生");
        }
        List<String> studentNumbers = students.stream()
                .map(Student::getStudentNumber)
                .toList();
        List<StudentSubjectSelection> selections = studentSubjectSelectionMapper.selectList(
                new LambdaQueryWrapper<StudentSubjectSelection>()
                        .in(StudentSubjectSelection::getStudentNumber, studentNumbers)
        );
        Map<String,StudentSubjectSelection> selectionMap = selections.stream()
                .collect(Collectors.toMap(StudentSubjectSelection::getStudentNumber, s -> s));
        List<StudentVO> studentVOList = students.stream()
                .map(s -> StudentConverter.toVO(s,selectionMap.get(s.getStudentNumber())))
                .toList();
        return ResponseResult.success(studentVOList);
    }
}
