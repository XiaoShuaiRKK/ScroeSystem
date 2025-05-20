package com.score.system.entity.school;

import com.baomidou.mybatisplus.annotation.IdType;
import com.baomidou.mybatisplus.annotation.TableField;
import com.baomidou.mybatisplus.annotation.TableId;
import com.baomidou.mybatisplus.annotation.TableName;
import com.score.system.entity.BaseEntity;
import jakarta.validation.constraints.*;
import lombok.Data;

@Data
@TableName("critical_config")
public class CriticalConfig extends BaseEntity {
    @TableId(type = IdType.AUTO)
    private Long id;
    @NotNull(message = "年级不能为空")
    @Min(value = 1, message = "年级必须大于等于1")
    private Integer grade;

    @NotNull(message = "学年不能为空")
    @Min(value = 2000, message = "学年格式不正确")
    private Integer year;

    @NotNull(message = "目标高校层次不能为空")
    @Min(value = 1, message = "目标高校层次必须为合法值（如：985、211等）")
    @Max(value = 4, message = "目标高校层次必须为合法值（如：985、211等）")
    @TableField("university_level")
    private Integer universityLevel;

    @NotNull(message = "目标人数不能为空")
    @Min(value = 1, message = "目标人数必须大于0")
    @TableField("target_count")
    private Integer targetCount;

    @NotNull(message = "临界比例不能为空")
    @DecimalMin(value = "0.01", inclusive = true, message = "临界比例必须大于0")
    @DecimalMax(value = "1.0", inclusive = true, message = "临界比例不能大于1")
    @TableField("critical_ratio")
    private Double criticalRatio;

    @NotNull(message = "科目分组ID不能为空")
    @Min(value = 1, message = "科目分组ID必须大于0")
    @TableField("subject_group_id")
    private Long subjectGroupId;
    private boolean deleted;
}
