package com.score.system.entity.user;

import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotNull;
import lombok.Data;

@Data
public class StudentSubjectSelectionDTO {
    @NotNull(message = "学生编号(学号)不能为空")
    private String studentNumber;

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
