package com.score.system.service;

import com.score.system.entity.request.LoginRequest;
import com.score.system.entity.request.RegisterRequest;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.result.LoginResult;
import com.score.system.entity.user.*;

import java.util.List;

public interface UserService {
    ResponseResult<Boolean> register(RegisterRequest registerRequest);
    ResponseResult<LoginResult> login(LoginRequest loginRequest);
    ResponseResult<List<UserLevel>> getUserLevels();
    ResponseResult<List<UserRole>> getUserRoles();
}