package com.score.system.entity.school;

import com.baomidou.mybatisplus.annotation.IdType;
import com.baomidou.mybatisplus.annotation.TableField;
import com.baomidou.mybatisplus.annotation.TableId;
import com.baomidou.mybatisplus.annotation.TableName;
import com.score.system.entity.BaseEntity;
import lombok.Data;

@Data
@TableName("critical_student_log")
public class CriticalStudentLog extends BaseEntity {
    @TableId(type = IdType.AUTO)
    private Long id;
    @TableField("exam_id")
    private Long examId;
    @TableField("student_number")
    private String studentNumber;
    @TableField("student_name")
    private String studentName;
    @TableField("university_level")
    private Integer universityLevel;
    @TableField("score_rank")
    private Integer scoreRank;
    @TableField("target_rank")
    private Integer targetRank;
    private Integer gap;
    private Double score;
    @TableField("subject_group_id")
    private Long subjectGroupId;
}
