package com.score.system.controller;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.user.StudentDTO;
import com.score.system.entity.user.StudentScoreVO;
import com.score.system.entity.user.StudentVO;
import com.score.system.service.TeacherService;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/teacher")
@Validated
public class TeacherController {
    private final TeacherService teacherService;

    public TeacherController(TeacherService teacherService) {
        this.teacherService = teacherService;
    }

    @GetMapping("/get/student/byClass")
    public ResponseResult<List<StudentVO>> selectClassStudent(@RequestParam("class_id")Long classId) {
        return teacherService.selectStudentsByClass(classId);
    }

    @GetMapping("/get/student")
    public ResponseResult<StudentVO> selectStudentByNumber(@RequestParam("student_number")String studentNumber) {
        return teacherService.selectStudentByTeacherId(studentNumber);
    }

    @GetMapping("/getClass/studentScore")
    public ResponseResult<List<StudentScoreVO>> selectTeacherClassStudentScore(@RequestParam("teacher_id") Long teacherId,
                                                                               @RequestParam("class_id") Long classId,
                                                                               @RequestParam("exam_id")Long examId){
        return teacherService.selectStudentScoreByTeacherId(teacherId, classId,examId);
    }

    @GetMapping("/studentScore")
    public ResponseResult<StudentScoreVO> selectStudentScore(@RequestParam("teacher_id")Long teacherId,
                                                             @RequestParam("student_number")String studentNumber,
                                                             @RequestParam("exam_id")Long examId){
        return teacherService.selectStudentScoreByStudentId(teacherId, studentNumber, examId);
    }
}
