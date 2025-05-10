package com.score.system.controller.interceptor;

import com.score.system.util.RedisUtil;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import org.springframework.stereotype.Component;
import org.springframework.web.method.HandlerMethod;
import org.springframework.web.servlet.HandlerInterceptor;

@Component
public class LevelInterceptor implements HandlerInterceptor {
    private final RedisUtil redisUtil;

    public LevelInterceptor(RedisUtil redisUtil) {
        this.redisUtil = redisUtil;
    }

    @Override
    public boolean preHandle(HttpServletRequest request, HttpServletResponse response, Object handler) throws Exception {
        if(!(handler instanceof HandlerMethod)) return true;
        HandlerMethod handlerMethod = (HandlerMethod)handler;
        LevelRequired levelRequired = handlerMethod.getMethodAnnotation(LevelRequired.class);
        if(levelRequired == null) return true;
        String authorizationHeader = request.getHeader("Authorization");
        // 如果请求头为空或者不以 Bearer 开头，返回未授权错误
        if (authorizationHeader == null || !authorizationHeader.startsWith("Bearer ")) {
            response.setStatus(HttpServletResponse.SC_UNAUTHORIZED);
            return false;
        }
        String token = authorizationHeader.substring(7);
        String levelStr = (String) redisUtil.get("login:level:" + token,String.class);
        if(levelStr == null){
            response.setStatus(HttpServletResponse.SC_UNAUTHORIZED);
            return false;
        }
        int userLevel = Integer.parseInt(levelStr);
        int requiredLevel = levelRequired.value();
        if(userLevel > requiredLevel){
            response.setStatus(HttpServletResponse.SC_UNAUTHORIZED);
            return false;
        }
        return true;
    }
}
