package com.score.system.service.impl;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.Course;
import com.score.system.entity.school.TeacherCourse;
import com.score.system.mapper.ClassMapper;
import com.score.system.mapper.CourseMapper;
import com.score.system.mapper.TeacherCourseMapper;
import com.score.system.mapper.TeacherMapper;
import com.score.system.service.CourseService;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.HashSet;
import java.util.List;
import java.util.Set;

@Service
public class CourseServiceImpl implements CourseService {
    private final CourseMapper courseMapper;
    private final TeacherMapper teacherMapper;
    private final TeacherCourseMapper teacherCourseMapper;
    private final ClassMapper classMapper;

    public CourseServiceImpl(CourseMapper courseMapper, TeacherMapper teacherMapper, TeacherCourseMapper teacherCourseMapper, ClassMapper classMapper) {
        this.courseMapper = courseMapper;
        this.teacherMapper = teacherMapper;
        this.teacherCourseMapper = teacherCourseMapper;
        this.classMapper = classMapper;
    }

    @Override
    public ResponseResult<List<Course>> getCourses() {
        return ResponseResult.success(courseMapper.selectList(null));
    }

    @Override
    public ResponseResult<Boolean> addCourse(Course course) {
        Course existing = courseMapper.selectByNameAndGrade(course.getName(), course.getGrade());
        if(existing != null){
            return ResponseResult.fail("该科目已存在,不能重复添加");
        }
        int result = courseMapper.insert(course);
        return result > 0 ? ResponseResult.success(true) : ResponseResult.fail("添加课程失败");
    }

    @Override
    @Transactional
    public ResponseResult<Boolean> batchAddCourse(List<Course> courses) {
        Set<String> courseKeySet = new HashSet<>();
        for (Course course : courses){
            String key = course.getName() + "-" + course.getGrade();
            if (!courseKeySet.add(key)) {
                return ResponseResult.fail("批量中存在重复课程: " + course.getName() + " 年级：" + course.getGrade());
            }

            Course existing = courseMapper.selectByNameAndGrade(course.getName(), course.getGrade());
            if (existing != null) {
                return ResponseResult.fail("课程已存在: " + course.getName() + " 年级：" + course.getGrade());
            }
        }
        int result = courseMapper.batchInsertCourse(courses);
        return result > 0 ? ResponseResult.success(true) : ResponseResult.fail("批量添加课程失败");
    }

    @Override
    public ResponseResult<Boolean> assignTeacherToCourse(TeacherCourse teacherCourse) {
        // 检查老师是否存在
        if (teacherMapper.selectById(teacherCourse.getTeacherId()) == null) {
            return ResponseResult.error("教师不存在");
        }

        // 检查课程是否存在
        if (courseMapper.selectById(teacherCourse.getCourseId()) == null) {
            return ResponseResult.error("课程不存在");
        }

        // 检查班级是否存在
        if (classMapper.selectById(teacherCourse.getClassId()) == null) {
            return ResponseResult.error("班级不存在");
        }
        int result = teacherCourseMapper.insert(teacherCourse);
        return result > 0 ? ResponseResult.success(true) : ResponseResult.fail("设置失败");
    }
}
