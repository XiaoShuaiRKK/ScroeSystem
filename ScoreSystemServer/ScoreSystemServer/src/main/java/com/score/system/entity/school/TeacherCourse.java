package com.score.system.entity.school;

import com.baomidou.mybatisplus.annotation.TableField;
import com.baomidou.mybatisplus.annotation.TableName;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotNull;
import lombok.Data;

@Data
@TableName("teacher_course")
public class TeacherCourse {
    @TableField("teacher_id")
    @NotNull(message = "教师ID不能为空")
    @Min(value = 1, message = "教师ID无效")
    private Long teacherId;
    @TableField("course_id")
    @NotNull(message = "课程ID不能为空")
    @Min(value = 1, message = "课程ID无效")
    private Long courseId;
    @TableField("class_id")
    @NotNull(message = "班级ID不能为空")
    @Min(value = 1, message = "班级ID无效")
    private Long classId;
}
