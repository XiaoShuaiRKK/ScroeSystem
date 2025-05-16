package com.score.system.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
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
    private final ExamSubjectThresholdMapper examSubjectThresholdMapper;

    public CourseServiceImpl(CourseMapper courseMapper, TeacherMapper teacherMapper, TeacherCourseMapper teacherCourseMapper, ClassMapper classMapper, ExamMapper examMapper, ExamSubjectThresholdMapper examSubjectThresholdMapper) {
        this.courseMapper = courseMapper;
        this.teacherMapper = teacherMapper;
        this.teacherCourseMapper = teacherCourseMapper;
        this.classMapper = classMapper;
        this.examMapper = examMapper;
        this.examSubjectThresholdMapper = examSubjectThresholdMapper;
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
            return ResponseResult.fail("教师不存在: ID=" + teacherCourse.getTeacherId());
        }

        // 检查课程是否存在
        if (courseMapper.selectById(teacherCourse.getCourseId()) == null) {
            return ResponseResult.fail("课程不存在: ID=" + teacherCourse.getCourseId());
        }

        // 检查班级是否存在
        if (classMapper.selectById(teacherCourse.getClassId()) == null) {
            return ResponseResult.fail("班级不存在: ID=" + teacherCourse.getClassId());
        }

        TeacherCourse teacherCourseOne = teacherCourseMapper.getTeacherCourseOne(teacherCourse.getTeacherId(), teacherCourse.getCourseId(), teacherCourse.getClassId());
        if(teacherCourseOne != null){
            return ResponseResult.fail("老师科目已设置 " + teacherCourse.getClassId());
        }
        if(teacherCourseMapper.selectClassCourse(teacherCourse.getCourseId(),teacherCourse.getClassId()) != null){
            return ResponseResult.fail("班级科目已被设置  " + teacherCourse.getClassId());
        }
        int result = teacherCourseMapper.insert(teacherCourse);
        if(result <= 0){
            return ResponseResult.fail("设置失败: 教师ID=" + teacherCourse.getTeacherId()
                    + ", 课程ID=" + teacherCourse.getCourseId()
                    + ", 班级ID=" + teacherCourse.getClassId());
        }
        return ResponseResult.success("设置成功",true);
    }

    @Override
    @Transactional(rollbackFor = Exception.class)
    public ResponseResult<Boolean> batchAssignTeacherToCourse(List<TeacherCourse> teacherCourses) {
        for (TeacherCourse teacherCourse : teacherCourses){
            ResponseResult<Boolean> result = assignTeacherToCourse(teacherCourse);
            if(result.getCode() != 200){
                throw new IllegalArgumentException(result.getMessage());
            }
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
    public ResponseResult<Boolean> addExamSubjectThreshold(ExamSubjectThreshold examSubjectThreshold) {
        if (examSubjectThreshold.getExamId() == null || examSubjectThreshold.getCourseId() == null || examSubjectThreshold.getThresholdScore() == null) {
            return ResponseResult.fail("参数不能为空");
        }
        // 校验考试是否存在
        if (examMapper.selectById(examSubjectThreshold.getExamId()) == null) {
            return ResponseResult.fail("考试ID不存在：" + examSubjectThreshold.getExamId());
        }
        // 可选：检查是否已存在
        LambdaQueryWrapper<ExamSubjectThreshold> query = new LambdaQueryWrapper<>();
        query.eq(ExamSubjectThreshold::getExamId, examSubjectThreshold.getExamId())
                .eq(ExamSubjectThreshold::getCourseId, examSubjectThreshold.getCourseId());
        if (examSubjectThresholdMapper.selectOne(query) != null) {
            return ResponseResult.fail("该考试科目的达标线已设置");
        }

        int result = examSubjectThresholdMapper.insert(examSubjectThreshold);
        return result > 0 ? ResponseResult.success(true) : ResponseResult.fail("添加失败");
    }

    @Override
    public ResponseResult<Boolean> batchAddExamSubjectThreshold(List<ExamSubjectThreshold> examSubjectThresholds) {
        for (ExamSubjectThreshold threshold : examSubjectThresholds){
            ResponseResult<Boolean> result = addExamSubjectThreshold(threshold);
            if(result.getCode() != 200){
                throw new IllegalArgumentException(result.getMessage());
            }
        }
        return ResponseResult.success("批量设置成功",true);
    }

    @Override
    public ResponseResult<List<ExamSubjectThreshold>> getExamSubjectThresholds(int examId) {
        List<ExamSubjectThreshold> list = examSubjectThresholdMapper.selectByExamId(examId);
        return ResponseResult.success(list);
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
