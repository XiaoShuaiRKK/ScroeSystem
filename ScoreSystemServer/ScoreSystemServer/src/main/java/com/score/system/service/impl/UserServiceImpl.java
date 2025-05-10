package com.score.system.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.QueryWrapper;
import com.score.system.entity.request.LoginRequest;
import com.score.system.entity.request.RegisterRequest;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.result.LoginResult;
import com.score.system.entity.user.Teacher;
import com.score.system.entity.user.User;
import com.score.system.entity.user.UserLevel;
import com.score.system.entity.user.UserVO;
import com.score.system.mapper.TeacherMapper;
import com.score.system.mapper.UserLevelMapper;
import com.score.system.mapper.UserMapper;
import com.score.system.service.UserService;
import com.score.system.util.RedisUtil;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;
import java.util.UUID;
import java.util.concurrent.TimeUnit;

@Service
public class UserServiceImpl implements UserService {
    private final UserMapper userMapper;
    private final UserLevelMapper userLevelMapper;
    private final BCryptPasswordEncoder passwordEncoder = new BCryptPasswordEncoder();
    private final RedisUtil redisUtil;

    public UserServiceImpl(UserMapper userMapper, UserLevelMapper userLevelMapper, RedisUtil redisUtil) {
        this.userMapper = userMapper;
        this.userLevelMapper = userLevelMapper;
        this.redisUtil = redisUtil;
    }


    @Override
    @Transactional
    public ResponseResult<Boolean> register(RegisterRequest registerRequest) {
        QueryWrapper<User> query = new QueryWrapper<>();
        query.eq("username", registerRequest.getUsername());
        if(userMapper.selectOne(query) != null) return ResponseResult.fail("用户名已经存在");
        User user = new User();
        user.setUsername(registerRequest.getUsername());
        user.setName(registerRequest.getName());
        user.setPasswordHash(passwordEncoder.encode(registerRequest.getPassword()));
        user.setLevel(1);
        user.setRole(4);
        userMapper.insert(user);
        return ResponseResult.success("注册成功",true);
    }

    @Override
    public ResponseResult<LoginResult> login(LoginRequest loginRequest) {
        String username = loginRequest.getUsername();
        //1. 查redis 用户缓存
        String redisKey = "login:user:" + username;
        UserVO cachedUser = null;
        //1.查redis
        if(redisUtil.exists(redisKey)){
            cachedUser = (UserVO)redisUtil.get(redisKey, UserVO.class);
            String tokenKey = "login:token:username:" + username;
            String token = (String) redisUtil.get(tokenKey,String.class);
            return ResponseResult.success("登录成功", new LoginResult(token,cachedUser));
        }
        //2.查询数据库
        cachedUser = userMapper.selectByUsername(username);
        if(cachedUser == null){
            return ResponseResult.fail("用户名或密码错误");
        }
        if(!passwordEncoder.matches(loginRequest.getPassword(), cachedUser.getPasswordHash())){
            return ResponseResult.fail("用户名或密码错误");
        }
        //3.生成token
        String token = UUID.randomUUID().toString();
        redisUtil.set("login:token:" + token,username,TimeUnit.DAYS,15); // 可用于认证时查 user
        redisUtil.set("login:token:username:" + username,token,TimeUnit.DAYS,15); // 可用于通过用户名快速找 token
        redisUtil.set("login:level:" + token,cachedUser.getLevel(),TimeUnit.DAYS,15);
        redisUtil.set(redisKey,cachedUser,TimeUnit.DAYS,15);
        return ResponseResult.success("登录成功",new LoginResult(token,cachedUser));
    }

    @Override
    public ResponseResult<List<UserLevel>> getUserLevels() {
        return ResponseResult.success(userLevelMapper.selectList(null));
    }
}
