package com.score.system.entity.school;

import com.baomidou.mybatisplus.annotation.IdType;
import com.baomidou.mybatisplus.annotation.TableField;
import com.baomidou.mybatisplus.annotation.TableId;
import com.baomidou.mybatisplus.annotation.TableName;
import com.score.system.entity.BaseEntity;
import lombok.Data;

@Data
@TableName("classes")
public class ClassEntity extends BaseEntity {
    @TableId(type = IdType.AUTO)
    private Integer id;
    @TableField("name")
    private String name;
    @TableField("grade")
    private int grade;
    @TableField("head_teacher_id")
    private Long headTeacherId;
    @TableField("subject_group_id")
    private Long subjectGroupId;
}
