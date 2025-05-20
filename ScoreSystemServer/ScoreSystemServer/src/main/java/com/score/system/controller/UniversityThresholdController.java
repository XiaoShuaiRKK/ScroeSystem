package com.score.system.controller;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.rank.StudentRanking;
import com.score.system.entity.university.GradeThresholdPredictionResult;
import com.score.system.entity.university.ThresholdPredictionResult;
import com.score.system.entity.university.ThresholdRankingResult;
import com.score.system.service.UniversityThresholdService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/university/threshold")
@RequiredArgsConstructor
public class UniversityThresholdController {
    private final UniversityThresholdService universityThresholdService;

    /**
     * 获取指定年级的各大学层次上线人数、比率和学生列表（按考试）
     *
     * @param grade 年级ID
     * @param examId  考试ID
     * @return 各层次大学的上线统计
     */
    @GetMapping("/grade/ranking")
    public ResponseResult<ThresholdRankingResult> getGradeThresholdRanking(
            @RequestParam("grade") Integer grade,
            @RequestParam("exam_id") Long examId
    ) {
        return universityThresholdService.getGradeThresholdRanking(grade, examId);
    }

    /**
     * 获取某学生基于所有历史成绩的各大学层次上线概率
     *
     * @param studentNumber 学号
     * @return 各大学层次的达标考试次数与上线概率
     */
    @GetMapping("/student/predict")
    public ResponseResult<List<ThresholdPredictionResult>> predictStudentThresholdProbability(
            @RequestParam("student_number") String studentNumber
    ) {
        return universityThresholdService.predictStudentThresholdProbability(studentNumber);
    }

    /**
     * 获取某班级在某次考试中，达到指定大学层次分数线的学生名单（含排名）
     *
     * @param classId         班级ID
     * @param universityLevel 大学层次（1: 985, 2: 双一流, 3: 优投, 4: 本科）
     * @param examId          考试ID
     * @return 达标学生的排名列表
     */
    @GetMapping("/class/qualified")
    public ResponseResult<List<StudentRanking>> getClassLevelStudentList(
            @RequestParam("class_id") Integer classId,
            @RequestParam("university_level") Long universityLevel,
            @RequestParam("exam_id") Long examId
    ) {
        return universityThresholdService.getClassLevelStudentList(classId, universityLevel, examId);
    }

    @GetMapping("/predict/grade")
    public ResponseResult<List<GradeThresholdPredictionResult>> predictGradeThresholdProbability(@RequestParam("grade") Integer grade,
                                                                                                 @RequestParam("university_level")Long universityLevel){
        return  universityThresholdService.predictGradeThresholdProbability(grade,universityLevel);
    }
}
