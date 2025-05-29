package com.score.system.controller;

import com.score.system.controller.interceptor.LevelRequired;
import com.score.system.entity.request.LoginRequest;
import com.score.system.entity.request.RegisterRequest;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.result.LoginResult;
import com.score.system.entity.user.*;
import com.score.system.service.StaffService;
import com.score.system.service.UserService;
import jakarta.validation.Valid;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@Validated
@RequestMapping("/user")
public class UserController {
    private final UserService userService;
    private final StaffService staffService;

    public UserController(UserService userService, StaffService staffService) {
        this.userService = userService;
        this.staffService = staffService;
    }

    @GetMapping("/levels")
    public ResponseResult<List<UserLevel>> getUserLevels() {
        return userService.getUserLevels();
    }

    @GetMapping("/roles")
    public ResponseResult<List<UserRole>> getUserRoles() {
        return userService.getUserRoles();
    }

    @PostMapping("/register")
    public ResponseResult<Boolean> createUser(@Valid @RequestBody RegisterRequest registerRequest){
        return userService.register(registerRequest);
    }

    @PostMapping("/login")
    public ResponseResult<LoginResult> login(@Valid @RequestBody LoginRequest loginRequest){
        return userService.login(loginRequest);
    }


    @PostMapping("/add/student")
    public ResponseResult<Boolean> addStudent(@RequestBody StudentDTO studentDTO){
        return staffService.addStudent(studentDTO);
    }

    @PostMapping("/batch/add/student")
    public ResponseResult<Boolean> batchAddStudent(@Valid @RequestBody List<StudentDTO> studentDTOList){
        return staffService.batchAddStudent(studentDTOList);
    }

    @LevelRequired(1)
    @PostMapping("/add/teacher")
    public ResponseResult<Boolean> addTeacher(@Valid @RequestBody TeacherDTO teacherDTO){
        return staffService.addTeacher(teacherDTO);
    }

    @PostMapping("/batch/add/teacher")
    public ResponseResult<Boolean> batchAddTeacher(@Valid @RequestBody List<TeacherDTO> teacherDTOList){
        return staffService.batchAddTeacher(teacherDTOList);
    }

    @GetMapping("/get/teachers")
    public ResponseResult<List<Teacher>> getTeachers() {
        return staffService.getAllTeachers();
    }

    @PostMapping("/delete/teacher")
    public ResponseResult<Boolean> deletedTeacher(@RequestBody TeacherDTO teacherDTO){
        return staffService.deleteTeacher(teacherDTO);
    }

    @PostMapping("/delete/student")
    public ResponseResult<Boolean> deleteStudent(@RequestBody StudentDTO studentDTO){
        return staffService.deleteStudent(studentDTO);
    }

    @PostMapping("/update/student")
    public ResponseResult<Boolean> updateStudent(@Valid @RequestBody StudentDTO studentDTO){
        return staffService.updateStudent(studentDTO);
    }

    @PostMapping("/update/batch/student")
    public ResponseResult<Boolean> batchUpdateStudent(@Valid @RequestBody List<StudentDTO> studentDTOList){
        return staffService.batchUpdateStudent(studentDTOList);
    }

    @PostMapping("/update/teacher")
    public ResponseResult<Boolean> updateTeacher(@Valid @RequestBody TeacherDTO teacherDTO){
        return staffService.updateTeacher(teacherDTO);
    }
}
