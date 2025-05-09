package com.score.system.entity.request;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Pattern;
import jakarta.validation.constraints.Size;
import lombok.Data;

@Data
public class LoginRequest {
    @NotBlank(message = "用户名不能为空")
    @Size(min = 3, max = 20, message = "用户名或者密码不正确")
    @Pattern(regexp = "^[a-zA-Z0-9_]+$", message = "用户名或者密码不正确")
    private String username;
    @NotBlank(message = "密码不能为空")
    @Size(min = 6, max = 32, message = "用户名或者密码不正确")
    private String password;
}
