package com.score.system.controller;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.Course;
import com.score.system.entity.school.TeacherCourse;
import com.score.system.service.CourseService;
import jakarta.validation.Valid;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/course")
public class CourseController {
    private final CourseService courseService;

    public CourseController(CourseService courseService) {
        this.courseService = courseService;
    }

    /**
     * 获取所有课程
     */
    @GetMapping("/list")
    public ResponseResult<List<Course>> getCourses() {
        return courseService.getCourses();
    }

    /**
     * 添加单个课程
     */
    @PostMapping("/add")
    public ResponseResult<Boolean> addCourse(@RequestBody @Valid Course course) {
        return courseService.addCourse(course);
    }

    /**
     * 批量添加课程
     */
    @PostMapping("/batchAdd")
    public ResponseResult<Boolean> batchAddCourses(@RequestBody @Valid List<Course> courses) {
        return courseService.batchAddCourse(courses);
    }

    @PostMapping("/assign")
    public ResponseResult<Boolean> assignTeacherToCourse(@RequestBody @Valid TeacherCourse teacherCourse) {
        return courseService.assignTeacherToCourse(teacherCourse);
    }
}
