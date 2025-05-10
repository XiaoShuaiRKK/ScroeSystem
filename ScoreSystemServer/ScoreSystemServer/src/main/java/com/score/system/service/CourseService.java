package com.score.system.service;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.Course;
import com.score.system.entity.school.TeacherCourse;

import java.util.List;

public interface CourseService {
    ResponseResult<List<Course>> getCourses();
    ResponseResult<Boolean> addCourse(Course course);
    ResponseResult<Boolean> batchAddCourse(List<Course> courses);

    /**
     * 设置教师对应的班级和科目
     * @param teacherCourse
     * @return
     */
    ResponseResult<Boolean> assignTeacherToCourse(TeacherCourse teacherCourse);
}
