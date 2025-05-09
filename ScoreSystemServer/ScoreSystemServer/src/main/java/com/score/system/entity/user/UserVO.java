package com.score.system.entity.user;

import com.baomidou.mybatisplus.annotation.IdType;
import com.baomidou.mybatisplus.annotation.TableField;
import com.baomidou.mybatisplus.annotation.TableId;
import lombok.Data;

@Data
public class UserVO {
    private Long id;
    private String name;
    private String username;
    private String passwordHash;
    private String level;
    private String role;
}
