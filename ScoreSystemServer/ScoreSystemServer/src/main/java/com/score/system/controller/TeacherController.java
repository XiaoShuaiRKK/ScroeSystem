package com.score.system.controller;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.user.StudentScoreVO;
import com.score.system.service.TeacherService;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/teacher")
@Validated
public class TeacherController {
    private final TeacherService teacherService;

    public TeacherController(TeacherService teacherService) {
        this.teacherService = teacherService;
    }

    @PostMapping("/getClass/studentScore")
    public ResponseResult<List<StudentScoreVO>> selectTeacherClassStudentScore(@RequestParam("teacher_id") Long teacherId,
                                                                               @RequestParam("class_id") Long classId,
                                                                               @RequestParam("exam_id")Long examId){
        return teacherService.selectStudentScoreByTeacherId(teacherId, classId,examId);
    }

    @PostMapping("/studentScore")
    public ResponseResult<StudentScoreVO> selectStudentScore(@RequestParam("teacher_id")Long teacherId,
                                                             @RequestParam("student_number")String studentNumber,
                                                             @RequestParam("exam_id")Long examId){
        return teacherService.selectStudentScoreByStudentId(teacherId, studentNumber, examId);
    }
}
