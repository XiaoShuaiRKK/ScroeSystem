package com.score.system.service;

import com.score.system.entity.request.LoginRequest;
import com.score.system.entity.request.RegisterRequest;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.result.LoginResult;
import com.score.system.entity.user.StudentDTO;
import com.score.system.entity.user.TeacherDTO;
import com.score.system.entity.user.User;
import com.score.system.entity.user.UserLevel;

import java.util.List;

public interface UserService {
    ResponseResult<Boolean> register(RegisterRequest registerRequest);
    ResponseResult<LoginResult> login(LoginRequest loginRequest);
    ResponseResult<List<UserLevel>> getUserLevels();
}