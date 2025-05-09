package com.score.system.entity.user;

import jakarta.validation.constraints.NotBlank;
import lombok.Data;

@Data
public class TeacherDTO {
    @NotBlank(message = "老师名字不能为空")
    private String name;
    private String username;
    @NotBlank(message = "老师编号不能为空")
    private String teacherNumber;
    private String password;
    private Long state;
}
