package com.score.system.entity.user;

import com.fasterxml.jackson.annotation.JsonFormat;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import lombok.Data;

import java.time.LocalDate;
import java.time.LocalDateTime;

@Data
public class StudentDTO {
    @NotBlank(message = "学生名字不能为空")
    private String name;
    private String userName;
    private String password;
    @NotBlank(message = "学号不能为空")
    private String studentNumber;
    private Integer classId;
    private Long state;
    @JsonFormat(pattern = "yyyy-MM-dd")
    private LocalDate enrollmentDate;
    @NotNull(message = "科目组合ID不能为空")
    @Min(value = 1, message = "选科ID错误")
    private Long subjectGroupId;
    @NotNull(message = "选修课1不能为空")
    @Min(value = 1, message = "选修课1 ID错误")
    private Long electiveCourse1Id;
    @NotNull(message = "选修课2不能为空")
    @Min(value = 1, message = "选修课2 ID错误")
    private Long electiveCourse2Id;
}
