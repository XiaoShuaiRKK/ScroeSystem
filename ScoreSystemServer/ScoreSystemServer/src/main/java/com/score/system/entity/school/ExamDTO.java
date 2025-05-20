package com.score.system.entity.school;


import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import lombok.Data;

import java.time.LocalDate;
import java.time.LocalDateTime;

@Data
public class ExamDTO {
    private Long id;
    @NotBlank(message = "考试名称不能为空")
    private String name;
    @Min(value = 1, message = "年级必须在1到6之间")
    @Max(value = 6, message = "年级必须在1到6之间")
    private int grade;
    private int year;
    private LocalDate startDate;
    private LocalDate endDate;
}
