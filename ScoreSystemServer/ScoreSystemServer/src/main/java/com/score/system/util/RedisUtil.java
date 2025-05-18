package com.score.system.util;

import com.fasterxml.jackson.databind.ObjectMapper;
import org.springframework.data.redis.core.RedisTemplate;
import org.springframework.stereotype.Component;

import java.util.concurrent.TimeUnit;

@Component
public class RedisUtil {
    private final RedisTemplate<String, Object> redisTemplate;
    private final ObjectMapper objectMapper;

    public RedisUtil(RedisTemplate<String, Object> redisTemplate, ObjectMapper objectMapper) {
        this.redisTemplate = redisTemplate;
        this.objectMapper = objectMapper;
    }

    public void set(String key, Object value, TimeUnit timeUnit, long timeoutSeconds) {
        redisTemplate.opsForValue().set(key, value, timeoutSeconds, timeUnit);
    }

    public <T> T get(String key,Class<?> classType) {
        return (T) objectMapper.convertValue(redisTemplate.opsForValue().get(key),classType);
    }

    public void delete(String key) {
        redisTemplate.delete(key);
    }

    public boolean exists(String key) {
        return Boolean.TRUE.equals(redisTemplate.hasKey(key));
    }

    public void zadd(String key, double score, String member) {
        redisTemplate.opsForZSet().add(key, member, score);
    }

    public Long zrevrank(String key, Object member) {
        return redisTemplate.opsForZSet().reverseRank(key, member);
    }

    public Long zcount(String key) {
        return redisTemplate.opsForZSet().zCard(key);
    }

    public Double zscore(String key, Object member) {
        try {
            return redisTemplate.opsForZSet().score(key, member);
        } catch (Exception e) {
            e.printStackTrace();
            return null;
        }
    }

    public boolean expire(String key, long time, TimeUnit unit) {
        try {
            return redisTemplate.expire(key, time, unit);
        } catch (Exception e) {
            e.printStackTrace();
            return false;
        }
    }


    public RedisTemplate<String, Object> getRedisTemplate() {
        return redisTemplate;
    }
}
