package com.score.system.mapper;

import com.baomidou.mybatisplus.core.mapper.BaseMapper;
import com.score.system.entity.school.TeacherCourse;
import org.apache.ibatis.annotations.Mapper;
import org.apache.ibatis.annotations.Select;

@Mapper
public interface TeacherCourseMapper extends BaseMapper<TeacherCourse> {
    @Select("select * from teacher_course where teacher_id = #{teacherId} and course_id = #{courseId} and class_id = #{classId}")
    TeacherCourse getTeacherCourseOne(Long teacherId,Long courseId,Long classId);
    @Select("select * from teacher_course where course_id = #{courseId} and class_id = #{classId}")
    TeacherCourse selectClassCourse(Long courseId,Long classId);
}
