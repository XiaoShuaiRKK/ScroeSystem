package com.score.system.entity;

import lombok.Data;

@Data
public class ResponseResult<T> {
    private int code;
    private String message;
    private T data;
    private long timestamp;
    private ResponseResult(int code, String message, T data) {
        this.code = code;
        this.message = message;
        this.data = data;
        this.timestamp = System.currentTimeMillis();
    }
    public static <T> ResponseResult<T> success(T data) {
        return new ResponseResult<T>(200, "success", data);
    }
    public static <T> ResponseResult<T> success(String message,T data) {
        return new ResponseResult<T>(200, message, data);
    }
    public static <T> ResponseResult<T> fail(String message,T data){
        return new ResponseResult<T>(400, message, data);
    }
    public static <T> ResponseResult<T> fail(String message) {
        return fail(message, null);
    }
    public static <T> ResponseResult<T> error(String message) {
        return new ResponseResult<>(500,message,null);
    }
}
