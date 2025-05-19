package com.score.system.entity.university;

import com.baomidou.mybatisplus.annotation.IdType;
import com.baomidou.mybatisplus.annotation.TableField;
import com.baomidou.mybatisplus.annotation.TableId;
import com.baomidou.mybatisplus.annotation.TableName;
import com.score.system.entity.BaseEntity;
import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import lombok.Data;

@Data
@TableName("university")
public class University extends BaseEntity {
    @TableId(type = IdType.AUTO)
    private Long id;
    @NotBlank(message = "大学名称不能为空")
    @TableField("name")
    private String name;
    @Min(value = 1,message = "大学等级格式不正确")
    @Max(value = 4,message = "大学等级格式不正确")
    @TableField("university_level")
    private Long universityLevel;
    @TableField("science_score_line")
    @Min(value = 0,message = "请输入正确的理科达标线")
    private Double scienceScoreLine;
    @Min(value = 0,message = "请输入正确的文科达标线")
    @TableField("art_score_line")
    private Double artScoreLine;
    @TableField("year")
    private Integer year;
}
