package com.score.system.mapper;

import com.baomidou.mybatisplus.core.mapper.BaseMapper;
import com.score.system.entity.school.Course;
import org.apache.ibatis.annotations.Mapper;
import org.apache.ibatis.annotations.Param;
import org.apache.ibatis.annotations.Select;

import java.util.List;

@Mapper
public interface CourseMapper extends BaseMapper<Course> {
    @Select("select * from courses where name = #{name} and grade = #{grade} limit 1")
    Course selectByNameAndGrade(String name,int grade);
    int batchInsertCourse(@Param("courses")List<Course> courses);
}
