package com.score.system.service.impl;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.user.*;
import com.score.system.mapper.StudentMapper;
import com.score.system.mapper.TeacherMapper;
import com.score.system.mapper.UserMapper;
import com.score.system.service.StaffService;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

@Service
public class StaffServiceImpl implements StaffService {
    private final TeacherMapper teacherMapper;
    private final StudentMapper studentMapper;
    private final UserMapper userMapper;
    private final BCryptPasswordEncoder passwordEncoder = new BCryptPasswordEncoder();

    public StaffServiceImpl(TeacherMapper teacherMapper, StudentMapper studentMapper, UserMapper userMapper) {
        this.teacherMapper = teacherMapper;
        this.studentMapper = studentMapper;
        this.userMapper = userMapper;
    }

    @Override
    @Transactional
    public ResponseResult<Boolean> addStudent(StudentDTO studentDTO) {
        User user = new User();
        user.setName(studentDTO.getName());
        user.setUsername(studentDTO.getUsername());
        user.setPasswordHash(passwordEncoder.encode(studentDTO.getPassword()));
        user.setLevel(3);// 普通用户
        user.setRole(3);// 学生
        userMapper.insert(user);
        Student student = StudentConverter.toEntity(studentDTO, user.getId());
        studentMapper.insert(student);
        return ResponseResult.success("添加成功",true);
    }

    @Override
    public ResponseResult<Boolean> batchAddStudent(List<StudentDTO> studentDTOList) {
       for (StudentDTO studentDTO : studentDTOList) {
           addStudent(studentDTO);
       }
       return ResponseResult.success("添加成功",true);
    }

    @Override
    @Transactional
    public ResponseResult<Boolean> addTeacher(TeacherDTO teacherDTO) {
        User user = new User();
        user.setName(teacherDTO.getName());
        user.setUsername(teacherDTO.getUsername());
        user.setPasswordHash(passwordEncoder.encode(teacherDTO.getPassword()));
        user.setLevel(2);
        user.setRole(2);
        userMapper.insert(user);
        Teacher teacher = TeacherConverter.toEntity(teacherDTO, user.getId());
        teacherMapper.insert(teacher);
        return ResponseResult.success("添加成功",true);
    }

    @Override
    public ResponseResult<Boolean> batchAddTeacher(List<TeacherDTO> teacherDTOList) {
        for (TeacherDTO teacherDTO : teacherDTOList) {
            addTeacher(teacherDTO);
        }
        return ResponseResult.success("添加成功",true);

    }
}
