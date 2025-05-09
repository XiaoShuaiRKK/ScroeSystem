package com.score.system.entity.user;

import jakarta.validation.constraints.NotBlank;
import lombok.Data;

import java.time.LocalDateTime;

@Data
public class StudentDTO {
    @NotBlank(message = "学生名字不能为空")
    private String name;
    private String username;
    private String password;
    @NotBlank(message = "学号不能为空")
    private String studentNumber;
    private Integer classId;
    private Long state;
    private LocalDateTime enrollmentDate;
}
