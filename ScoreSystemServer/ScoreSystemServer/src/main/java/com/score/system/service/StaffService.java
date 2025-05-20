package com.score.system.service;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.user.StudentDTO;
import com.score.system.entity.user.Teacher;
import com.score.system.entity.user.TeacherDTO;

import java.util.List;

public interface StaffService {
    ResponseResult<Boolean> addStudent(StudentDTO studentDTO);
    ResponseResult<Boolean> batchAddStudent(List<StudentDTO> studentDTOList);
    ResponseResult<Boolean> addTeacher(TeacherDTO teacherDTO);
    ResponseResult<Boolean> batchAddTeacher(List<TeacherDTO> teacherDTOList);
    ResponseResult<List<Teacher>> getAllTeachers();
}
