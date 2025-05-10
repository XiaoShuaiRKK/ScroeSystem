package com.score.system.mapper;

import com.baomidou.mybatisplus.core.mapper.BaseMapper;
import com.score.system.entity.school.ClassDTO;
import com.score.system.entity.school.ClassEntity;
import org.apache.ibatis.annotations.Mapper;
import org.apache.ibatis.annotations.Param;
import org.apache.ibatis.annotations.Select;

import java.util.List;

@Mapper
public interface ClassMapper extends BaseMapper<ClassEntity> {
    @Select("select * from classes where name = #{className}")
    ClassEntity selectByClassName(String className);
    int batchInsertClass(@Param("classList")List<ClassEntity> classEntities);
}
