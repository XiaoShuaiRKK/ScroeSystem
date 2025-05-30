package com.score.system.entity.school;

import com.baomidou.mybatisplus.annotation.IdType;
import com.baomidou.mybatisplus.annotation.TableId;
import com.baomidou.mybatisplus.annotation.TableName;
import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import lombok.Data;

@Data
@TableName("courses")
public class Course {
    @TableId(type = IdType.AUTO)
    private Long id;
    @NotBlank(message = "科目名称不能为空")
    private String name;
}
