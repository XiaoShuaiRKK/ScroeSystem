package com.score.system.controller;

import com.score.system.entity.request.LoginRequest;
import com.score.system.entity.request.RegisterRequest;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.result.LoginResult;
import com.score.system.entity.user.StudentDTO;
import com.score.system.entity.user.TeacherDTO;
import com.score.system.entity.user.User;
import com.score.system.entity.user.UserLevel;
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

    @PostMapping("/batch/add/teacher")
    public ResponseResult<Boolean> batchAddStudent(@Valid @RequestBody List<StudentDTO> studentDTOList){
        return staffService.batchAddStudent(studentDTOList);
    }

    @PostMapping("/add/teacher")
    public ResponseResult<Boolean> addTeacher(@Valid @RequestBody TeacherDTO teacherDTO){
        return staffService.addTeacher(teacherDTO);
    }

    @PostMapping("/batch/add/teacher")
    public ResponseResult<Boolean> batchAddTeacher(@Valid @RequestBody List<TeacherDTO> teacherDTOList){
        return staffService.batchAddTeacher(teacherDTOList);
    }
}
