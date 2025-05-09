package com.score.system.mapper;

import com.baomidou.mybatisplus.core.mapper.BaseMapper;
import com.score.system.entity.user.User;
import com.score.system.entity.user.UserVO;
import org.apache.ibatis.annotations.Mapper;
import org.apache.ibatis.annotations.Param;
import org.apache.ibatis.annotations.Select;

import java.util.List;

@Mapper
public interface UserMapper extends BaseMapper<User> {
    @Select("""
        SELECT
          u.id,
          u.name,
          u.username,
          u.password_hash AS passwordHash,
          ul.name AS level,
          r.name AS role
        FROM user u
        LEFT JOIN user_level ul ON u.level = ul.id
        LEFT JOIN role r ON u.role = r.id
        WHERE u.username = #{username}
    """)
    UserVO selectByUsername(String username);

    User findByUsername(@Param("username") String username);

    Boolean existsByUsername(@Param("username") String username);

    void batchInsert(@Param("list") List<User> users);
}
