package com.score.system.service;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.ClassRankingDTO;
import com.score.system.entity.school.GradeRankingDTO;
import com.score.system.entity.school.Score;

import java.util.List;
import java.util.Map;

public interface ScoreService {
    ResponseResult<Boolean> addScore(Score score);
    ResponseResult<Boolean> batchAddScores(List<Score> scores);
    ResponseResult<List<ClassRankingDTO>> calculateClassRankings(String studentNumber, Long examId);
    ResponseResult<List<GradeRankingDTO>> calculateGradeRankings(String studentNumber, Long examId);
    ResponseResult<ClassRankingDTO> calculateTotalScoreClassRankings(String studentNumber, Long examId);
    ResponseResult<GradeRankingDTO> calculateTotalScoreGradeRankings(String studentNumber, Long examId);
}
