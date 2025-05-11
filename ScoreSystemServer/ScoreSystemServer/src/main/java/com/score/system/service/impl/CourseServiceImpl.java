package com.score.system.service.impl;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.*;
import com.score.system.mapper.*;
import com.score.system.service.CourseService;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

@Service
public class CourseServiceImpl implements CourseService {
    private final CourseMapper courseMapper;
    private final TeacherMapper teacherMapper;
    private final TeacherCourseMapper teacherCourseMapper;
    private final ClassMapper classMapper;
    private final ExamMapper examMapper;

    public CourseServiceImpl(CourseMapper courseMapper, TeacherMapper teacherMapper, TeacherCourseMapper teacherCourseMapper, ClassMapper classMapper, ExamMapper examMapper) {
        this.courseMapper = courseMapper;
        this.teacherMapper = teacherMapper;
        this.teacherCourseMapper = teacherCourseMapper;
        this.classMapper = classMapper;
        this.examMapper = examMapper;
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

    @Override
    public ResponseResult<Boolean> addExam(ExamDTO examDTO) {
        if(examMapper.selectByExamName(examDTO.getName()) != null){
            return ResponseResult.fail("该考试已存在, 不能重复添加");
        }
        if (examDTO.getStartDate() == null || examDTO.getEndDate() == null) {
            return ResponseResult.fail("考试时间不能为空");
        }
        LocalDate today = LocalDate.now();
        if(examDTO.getStartDate().isBefore(today)){
            return ResponseResult.fail("考试开始时间不能早于今天");
        }
        if (examDTO.getEndDate().isBefore(examDTO.getStartDate())) {
            return ResponseResult.fail("考试结束时间不能早于开始时间");
        }
        Exam exam = ExamConverter.toEntity(examDTO);
        int result = examMapper.insert(exam);
        return result > 0 ? ResponseResult.success(true) : ResponseResult.fail("添加考试失败");
    }

    @Override
    @Transactional
    public ResponseResult<Boolean> batchAddExam(List<ExamDTO> examDTOList) {
        Set<String> examNames = new HashSet<>();

        LocalDate today = LocalDate.now();

        for (ExamDTO exam : examDTOList) {
            // 名称重复校验
            if (!examNames.add(exam.getName())) {
                return ResponseResult.fail("批量中存在重复考试名称：" + exam.getName());
            }

            // 时间合法性校验
            if (exam.getStartDate() == null || exam.getEndDate() == null) {
                return ResponseResult.fail("考试 " + exam.getName() + " 的时间不能为空");
            }
            if (exam.getStartDate().isBefore(today)) {
                return ResponseResult.fail("考试 " + exam.getName() + " 的开始时间不能早于今天");
            }
            if (exam.getEndDate().isBefore(exam.getStartDate())) {
                return ResponseResult.fail("考试 " + exam.getName() + " 的结束时间不能早于开始时间");
            }

            // 数据库中名称重复校验（可选）
            if (examMapper.selectByExamName(exam.getName()) != null) {
                return ResponseResult.fail("考试名称已存在：" + exam.getName());
            }
        }

        int result = examMapper.batchInsertExams(examDTOList);
        return result > 0 ? ResponseResult.success(true) : ResponseResult.fail("批量添加考试失败");
    }


    @Override
    public ResponseResult<List<ExamDTO>> getExams() {
        List<ExamDTO> examDTOList = new ArrayList<>();
        List<Exam> examList = examMapper.selectList(null);
        for (Exam exam : examList) {
            ExamDTO examDTO = ExamConverter.toDTO(exam);
            examDTOList.add(examDTO);
        }
        return ResponseResult.success(examDTOList);
    }
}
