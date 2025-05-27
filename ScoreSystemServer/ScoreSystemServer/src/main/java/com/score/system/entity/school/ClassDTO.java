package com.score.system.entity.school;

import jakarta.validation.constraints.*;
import lombok.Data;

@Data
public class ClassDTO {
    private Long id;
    @NotBlank(message = "班级名称不能为空")
    @Pattern(
            regexp = "^高[一二三]\\d{1,2}班$",
            message = "班级名称格式错误，应为例如：高一12班"
    )
    private String name;
    @Min(value = 1, message = "年级必须在1到6之间")
    @Max(value = 6, message = "年级必须在1到6之间")
    private int grade;
    @NotNull(message = "选科不能为空")
    @Min(value = 1,message = "选科ID不正确,必须大于0")
    private Long subjectGroupId;
    private Long headTeacherId;
    private String teacherName;
}
