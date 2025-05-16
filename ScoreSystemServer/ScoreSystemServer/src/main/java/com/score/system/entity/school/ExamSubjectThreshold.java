package com.score.system.entity.school;

import com.baomidou.mybatisplus.annotation.IdType;
import com.baomidou.mybatisplus.annotation.TableField;
import com.baomidou.mybatisplus.annotation.TableId;
import com.baomidou.mybatisplus.annotation.TableName;
import com.score.system.entity.BaseEntity;
import lombok.Data;

@Data
@TableName("exam_subject_threshold")
public class ExamSubjectThreshold extends BaseEntity {
    @TableId(type = IdType.AUTO)
    private Long id;
    @TableField("exam_id")
    private Long examId;
    @TableField("course_id")
    private Long courseId;
    @TableField("threshold_score")
    private Double thresholdScore;
}
