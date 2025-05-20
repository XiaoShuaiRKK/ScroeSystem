package com.score.system.controller;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.trend.StudentScoreTrendResult;
import com.score.system.service.StudentScoreService;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;
import java.util.Map;

@RestController
@RequestMapping("/student/trend")
@Validated
public class StudentScoreTrendController {
    private final StudentScoreService studentScoreService;

    public StudentScoreTrendController(StudentScoreService studentScoreService) {
        this.studentScoreService = studentScoreService;
    }

    @GetMapping("/student/score-trend")
    public ResponseResult<StudentScoreTrendResult> getStudentScoreTrend(
            @RequestParam("student_number") String studentNumber,
            @RequestParam("grade") Integer grade
    ) {
        return studentScoreService.getStudentScoreTrend(studentNumber,grade);
    }

    @GetMapping("/student/312/score-trend")
    public ResponseResult<Map<Long,Double>> getStudentExamTotalScores(@RequestParam("student_number")String studentNumber,
                                                                      @RequestParam("grade")Integer grade){
        return studentScoreService.getStudentExam312Scores(studentNumber,grade);
    }
}
