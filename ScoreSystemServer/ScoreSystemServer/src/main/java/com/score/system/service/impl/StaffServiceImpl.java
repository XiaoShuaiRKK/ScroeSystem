package com.score.system.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.baomidou.mybatisplus.core.conditions.query.QueryWrapper;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.ClassEntity;
import com.score.system.entity.user.*;
import com.score.system.mapper.*;
import com.score.system.service.StaffService;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

@Service
public class StaffServiceImpl implements StaffService {
    private final TeacherMapper teacherMapper;
    private final StudentMapper studentMapper;
    private final ClassMapper classMapper;
    private final UserMapper userMapper;
    private final StudentSubjectSelectionMapper studentSubjectSelectionMapper;
    private final StudentClassHistoryMapper studentClassHistoryMapper;
    private final BCryptPasswordEncoder passwordEncoder = new BCryptPasswordEncoder();

    public StaffServiceImpl(TeacherMapper teacherMapper, StudentMapper studentMapper, ClassMapper classMapper, UserMapper userMapper, StudentSubjectSelectionMapper studentSubjectSelectionMapper, StudentClassHistoryMapper studentClassHistoryMapper) {
        this.teacherMapper = teacherMapper;
        this.studentMapper = studentMapper;
        this.classMapper = classMapper;
        this.userMapper = userMapper;
        this.studentSubjectSelectionMapper = studentSubjectSelectionMapper;
        this.studentClassHistoryMapper = studentClassHistoryMapper;
    }

    @Override
    @Transactional(rollbackFor = Exception.class)
    public ResponseResult<Boolean> addStudent(StudentDTO studentDTO) {
        if (classMapper.selectById(studentDTO.getClassId()) == null) {
            return ResponseResult.fail("班级不存在", false);
        }

        if (studentDTO.getEnrollmentDate() != null && studentDTO.getEnrollmentDate().isAfter(LocalDate.now())) {
            return ResponseResult.fail("入学时间不能晚于当前时间", false);
        }

        // 检查学号是否重复
        LambdaQueryWrapper<Student> queryWrapper = new LambdaQueryWrapper<>();
        queryWrapper.eq(Student::getStudentNumber, studentDTO.getStudentNumber());
        if (studentMapper.selectOne(queryWrapper) != null) {
            return ResponseResult.fail("学号已存在", false);
        }

        // 创建用户
        User user = new User();
        user.setName(studentDTO.getName());
        user.setUsername(studentDTO.getUserName());
        user.setPasswordHash(passwordEncoder.encode(studentDTO.getPassword()));
        user.setLevel(3); // 普通用户
        user.setRole(3);  // 学生

        LambdaQueryWrapper<User> userQuery = new LambdaQueryWrapper<>();
        userQuery.eq(User::getUsername, user.getUsername());
        if (userMapper.selectOne(userQuery) != null) {
            return ResponseResult.fail(user.getName() + " 用户名已经存在");
        }

        userMapper.insert(user);

        // 插入学生记录
        Student student = StudentConverter.toEntity(studentDTO, user.getId());
        studentMapper.insert(student);

        // 插入选科记录
        StudentSubjectSelection selection = new StudentSubjectSelection();
        selection.setStudentNumber(student.getStudentNumber());
        selection.setSubjectGroupId(studentDTO.getSubjectGroupId());
        selection.setElectiveCourse1Id(studentDTO.getElectiveCourse1Id());
        selection.setElectiveCourse2Id(studentDTO.getElectiveCourse2Id());
        studentSubjectSelectionMapper.insert(selection);

        // 获取班级信息以获取年级
        ClassEntity classEntity = classMapper.selectById(studentDTO.getClassId());

        // 插入学生历史班级记录
        StudentClassHistory history = new StudentClassHistory();
        history.setStudentNumber(student.getStudentNumber());
        history.setClassId(studentDTO.getClassId().longValue());
        history.setGrade(classEntity.getGrade());
        history.setYear(studentDTO.getYear()); // 注意：year 由前端传入
        studentClassHistoryMapper.insert(history);

        return ResponseResult.success("添加成功", true);
    }


    @Override
    @Transactional
    public ResponseResult<Boolean> batchAddStudent(List<StudentDTO> studentDTOList) {
        try {
            for (StudentDTO studentDTO : studentDTOList) {
                ResponseResult<Boolean> result = addStudent(studentDTO);
                if(result.getCode() != 200){
                    throw new IllegalArgumentException("批量添加失败: " + result.getMessage());
                }
            }
        }catch (IllegalArgumentException e){
            return ResponseResult.fail("批量添加学生失败: " + e.getMessage());
        }catch (Exception ex){
            throw ex;
        }
       return ResponseResult.success("批量添加成功",true);
    }

    @Override
    @Transactional(rollbackFor = Exception.class)
    public ResponseResult<Boolean> addTeacher(TeacherDTO teacherDTO) {
        User user = new User();
        user.setName(teacherDTO.getName());
        user.setUsername(teacherDTO.getUsername());
        user.setPasswordHash(passwordEncoder.encode(teacherDTO.getPassword()));
        user.setLevel(2);
        user.setRole(2);
        LambdaQueryWrapper<User> query = new LambdaQueryWrapper<>();
        query.eq(User::getUsername, user.getUsername());
        if(userMapper.selectOne(query) != null) return ResponseResult.fail(user.getName() + " 用户名已经存在");
        userMapper.insert(user);
        Teacher teacher = TeacherConverter.toEntity(teacherDTO, user.getId());
        LambdaQueryWrapper<Teacher> queryWrapper = new LambdaQueryWrapper<>();
        queryWrapper.eq(Teacher::getTeacherNumber, teacherDTO.getTeacherNumber());
        if(teacherMapper.selectOne(queryWrapper) != null) return ResponseResult.fail(teacher.getTeacherNumber() + " 教师编号已经存在");
        teacherMapper.insert(teacher);
        return ResponseResult.success("添加成功",true);
    }

    @Override
    @Transactional
    public ResponseResult<Boolean> batchAddTeacher(List<TeacherDTO> teacherDTOList) {
        try {
            for (TeacherDTO teacherDTO : teacherDTOList) {
                ResponseResult<Boolean> result = addTeacher(teacherDTO);
                if(result.getCode() != 200){
                    throw new IllegalArgumentException("批量添加失败: " + result.getMessage());
                }
            }
        }catch (IllegalArgumentException e){
            return ResponseResult.fail("批量添加老师失败: " + e.getMessage(),true);
        }
        catch (Exception ex){
            throw ex;
        }
        return ResponseResult.success("添加成功",true);

    }

    @Override
    public ResponseResult<List<Teacher>> getAllTeachers() {
        return ResponseResult.success(teacherMapper.selectList(null));
    }
}
