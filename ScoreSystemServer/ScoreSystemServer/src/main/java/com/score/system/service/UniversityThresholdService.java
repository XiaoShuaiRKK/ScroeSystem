package com.score.system.service;


import com.score.system.entity.ResponseResult;
import com.score.system.entity.rank.StudentRanking;
import com.score.system.entity.university.GradeThresholdPredictionResult;
import com.score.system.entity.university.ThresholdPredictionResult;
import com.score.system.entity.university.ThresholdRankingResult;

import java.util.List;

public interface UniversityThresholdService {
    ResponseResult<ThresholdRankingResult> getGradeThresholdRanking(Integer grade, Long examId);
    ResponseResult<List<ThresholdPredictionResult>> predictStudentThresholdProbability(String studentNumber);
    ResponseResult<List<StudentRanking>> getClassLevelStudentList(Integer classId, Long universityLevel, Long examId);
    ResponseResult<List<GradeThresholdPredictionResult>> predictGradeThresholdProbability(Integer gradeId, Long universityLevel);
}
