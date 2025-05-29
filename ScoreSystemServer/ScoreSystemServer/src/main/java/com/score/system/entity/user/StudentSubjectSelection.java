package com.score.system.entity.user;

import com.baomidou.mybatisplus.annotation.IdType;
import com.baomidou.mybatisplus.annotation.TableField;
import com.baomidou.mybatisplus.annotation.TableId;
import com.baomidou.mybatisplus.annotation.TableName;
import com.score.system.entity.BaseEntity;
import lombok.Data;

@Data
@TableName("student_subject_selection")
public class StudentSubjectSelection extends BaseEntity {
    @TableId(value = "student_number",type = IdType.INPUT)
    private String studentNumber;
    @TableField("subject_group_id")
    private Long subjectGroupId;
    @TableField("elective_course1_id")
    private Long electiveCourse1Id;
    @TableField("elective_course2_id")
    private Long electiveCourse2Id;
}
