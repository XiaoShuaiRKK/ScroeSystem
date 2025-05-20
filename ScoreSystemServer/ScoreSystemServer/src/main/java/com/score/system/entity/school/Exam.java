package com.score.system.entity.school;

import com.baomidou.mybatisplus.annotation.IdType;
import com.baomidou.mybatisplus.annotation.TableField;
import com.baomidou.mybatisplus.annotation.TableId;
import com.baomidou.mybatisplus.annotation.TableName;
import com.score.system.entity.BaseEntity;
import lombok.Data;

import java.time.LocalDate;
import java.time.LocalDateTime;

@Data
@TableName("exams")
public class Exam extends BaseEntity {
    @TableId(type = IdType.AUTO)
    private Long id;
    private String name;
    private int grade;
    private int year;
    @TableField("start_date")
    private LocalDate startDate;
    @TableField("end_date")
    private LocalDate endDate;
}
