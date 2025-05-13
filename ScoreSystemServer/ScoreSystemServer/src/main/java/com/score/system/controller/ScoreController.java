package com.score.system.controller;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.ClassRankingDTO;
import com.score.system.entity.school.GradeRankingDTO;
import com.score.system.entity.school.Score;
import com.score.system.service.ScoreService;
import jakarta.validation.Valid;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/score")
@Validated
public class ScoreController {
    private final ScoreService scoreService;

    public ScoreController(@Valid @RequestBody ScoreService scoreService) {
        this.scoreService = scoreService;
    }

    @PostMapping("/add")
    public ResponseResult<Boolean> addScore(Score score) {
        return scoreService.addScore(score);
    }

    @PostMapping("/batchAdd")
    public ResponseResult<Boolean> batchAddScore(@Valid @RequestBody List<Score> scores) {
        return scoreService.batchAddScores(scores);
    }

    @PostMapping("/rankings/class")
    public ResponseResult<List<ClassRankingDTO>> calculateClassRankings(@RequestParam("exam_id") Long examId,
                                                                        @RequestParam("student_number")String studentNumber) {
        return scoreService.calculateClassRankings(studentNumber, examId);
    }

    @PostMapping("/rankings/grade")
    public ResponseResult<List<GradeRankingDTO>> calculateGradeRankings(@RequestParam("exam_id") Long examId,
                                                                        @RequestParam("student_number")String studentNumber){
        return scoreService.calculateGradeRankings(studentNumber, examId);
    }
}
