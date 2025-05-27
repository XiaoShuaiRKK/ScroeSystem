package com.score.system.entity.school;

import com.baomidou.mybatisplus.annotation.IdType;
import com.baomidou.mybatisplus.annotation.TableField;
import com.baomidou.mybatisplus.annotation.TableId;
import com.baomidou.mybatisplus.annotation.TableName;
import com.score.system.entity.BaseEntity;
import lombok.Data;

@Data
@TableName("exam_class_subject_stat")
public class ExamClassSubjectStat extends BaseEntity {
    @TableId(type = IdType.AUTO)
    private Long id;
    @TableField("exam_id")
    private Long examId;
    @TableField("subject_id")
    private Long subjectId;
    @TableField("avg_score")
    private Double avgScore;
    @TableField("synergy_rate")
    private Double synergyRate;
    @TableField("contribution_rate")
    private Double contributionRate;
}
