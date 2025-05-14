package com.score.system.entity.school;

import com.baomidou.mybatisplus.annotation.IdType;
import com.baomidou.mybatisplus.annotation.TableField;
import com.baomidou.mybatisplus.annotation.TableId;
import com.baomidou.mybatisplus.annotation.TableName;
import com.score.system.entity.BaseEntity;
import lombok.Data;

@Data
@TableName("scores")
public class Score extends BaseEntity {
    @TableId(type = IdType.AUTO)
    private Long id;
    @TableField("student_number")
    private String studentNumber;
    @TableField("course_id")
    private Long courseId;
    @TableField("exam_id")
    private Long examId;
    @TableField("score")
    private Double score;
    @TableField("comment")
    private String comment;
}
