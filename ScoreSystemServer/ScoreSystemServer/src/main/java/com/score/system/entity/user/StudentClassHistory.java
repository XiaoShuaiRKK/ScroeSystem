package com.score.system.entity.user;


import com.baomidou.mybatisplus.annotation.TableField;
import com.baomidou.mybatisplus.annotation.TableName;
import com.score.system.entity.BaseEntity;
import lombok.Data;

@Data
@TableName("student_class_history")
public class StudentClassHistory extends BaseEntity {
    @TableField("student_number")
    private String studentNumber;

    @TableField("class_id")
    private Long classId;

    @TableField("grade")
    private Integer grade;
    @TableField("year")
    private Integer year;
}
