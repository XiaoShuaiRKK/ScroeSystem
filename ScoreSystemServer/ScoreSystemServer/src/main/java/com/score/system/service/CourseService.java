package com.score.system.service;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.Course;
import com.score.system.entity.school.ExamDTO;
import com.score.system.entity.school.ExamSubjectThreshold;
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
    ResponseResult<Boolean> batchAssignTeacherToCourse(List<TeacherCourse> teacherCourses);
    ResponseResult<Boolean> addExam(ExamDTO examDTO);
    ResponseResult<List<ExamDTO>> getExams();
    ResponseResult<Boolean> batchAddExam(List<ExamDTO> examDTOList);
    ResponseResult<Boolean> deleteExam(ExamDTO examDTO);
    ResponseResult<Boolean> updateExam(ExamDTO examDTO);
    ResponseResult<Boolean> addExamSubjectThreshold(ExamSubjectThreshold examSubjectThreshold);
    ResponseResult<Boolean> batchAddExamSubjectThreshold(List<ExamSubjectThreshold> examSubjectThresholds);
    ResponseResult<List<ExamSubjectThreshold>> getExamSubjectThresholds(int examId);
}
