package com.score.system.entity.user;

import com.baomidou.mybatisplus.annotation.IdType;
import com.baomidou.mybatisplus.annotation.TableField;
import com.baomidou.mybatisplus.annotation.TableId;
import com.baomidou.mybatisplus.annotation.TableName;
import com.score.system.entity.BaseEntity;
import lombok.Data;

import java.time.LocalDateTime;

@Data
@TableName("student")
public class Student extends BaseEntity {
    @TableId(type = IdType.AUTO)
    private Long id;

    @TableField("name")
    private String name;

    @TableField("user_id")
    private Long userId;

    @TableField("student_number")
    private String studentNumber;

    @TableField("class_id")
    private Integer classId;

    @TableField("enrollment_date")
    private LocalDateTime enrollmentDate;

    @TableField("state")
    private Long state;
}
