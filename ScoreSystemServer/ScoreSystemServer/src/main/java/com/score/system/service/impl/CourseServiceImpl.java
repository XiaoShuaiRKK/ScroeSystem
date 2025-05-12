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
        Course existing = courseMapper.selectByName(course.getName());
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
            if (!courseKeySet.add(course.getName())) {
                return ResponseResult.fail("批量中存在重复课程: " + course.getName());
            }

            Course existing = courseMapper.selectByName(course.getName());
            if (existing != null) {
                return ResponseResult.fail("课程已存在: " + course.getName());
            }
        }
        int result = courseMapper.batchInsertCourse(courses);
        return result > 0 ? ResponseResult.success(true) : ResponseResult.fail("批量添加课程失败");
    }

    @Override
    public ResponseResult<Boolean> assignTeacherToCourse(TeacherCourse teacherCourse) {
        // 检查老师是否存在
        if (teacherMapper.selectById(teacherCourse.getTeacherId()) == null) {
            throw new IllegalArgumentException("教师不存在: ID=" + teacherCourse.getTeacherId());
        }

        // 检查课程是否存在
        if (courseMapper.selectById(teacherCourse.getCourseId()) == null) {
            throw new IllegalArgumentException("课程不存在: ID=" + teacherCourse.getCourseId());
        }

        // 检查班级是否存在
        if (classMapper.selectById(teacherCourse.getClassId()) == null) {
            throw new IllegalArgumentException("班级不存在: ID=" + teacherCourse.getClassId());
        }

        TeacherCourse teacherCourseOne = teacherCourseMapper.getTeacherCourseOne(teacherCourse.getTeacherId(), teacherCourse.getCourseId(), teacherCourse.getClassId());
        if(teacherCourseOne != null){
            throw new IllegalArgumentException("老师科目已设置 " + teacherCourse.getClassId());
        }
        if(teacherCourseMapper.selectClassCourse(teacherCourse.getCourseId(),teacherCourse.getClassId()) != null){
            throw new IllegalArgumentException("班级科目已被设置  " + teacherCourse.getClassId());
        }
        int result = teacherCourseMapper.insert(teacherCourse);
        if(result <= 0){
            throw new RuntimeException("设置失败: 教师ID=" + teacherCourse.getTeacherId()
                    + ", 课程ID=" + teacherCourse.getCourseId()
                    + ", 班级ID=" + teacherCourse.getClassId());
        }
        return ResponseResult.success("设置成功",true);
    }

    @Override
    @Transactional(rollbackFor = Exception.class)
    public ResponseResult<Boolean> batchAssignTeacherToCourse(List<TeacherCourse> teacherCourses) {
        for (TeacherCourse teacherCourse : teacherCourses){
            assignTeacherToCourse(teacherCourse);
        }
        return ResponseResult.success("批量设置成功",true);
    }

    @Override
    @Transactional
    public ResponseResult<Boolean> addExam(ExamDTO examDTO) {
        if(examMapper.selectByExamName(examDTO.getName(), examDTO.getGrade()) != null){
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
        List<Exam> examList = new ArrayList<>();
        Set<String> examNames = new HashSet<>();

        LocalDate today = LocalDate.now();

        for (ExamDTO exam : examDTOList) {
            String key = exam.getName() + "-" + exam.getGrade();
            // 名称重复校验
            if (!examNames.add(key)) {
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
            if (examMapper.selectByExamName(exam.getName(), exam.getGrade()) != null) {
                return ResponseResult.fail("考试名称已存在：" + exam.getName());
            }
            Exam entity = ExamConverter.toEntity(exam);
            entity.setCreatedAt(LocalDateTime.now());
            entity.setUpdatedAt(LocalDateTime.now());
            examList.add(entity);
        }

        int result = examMapper.batchInsertExams(examList);
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
