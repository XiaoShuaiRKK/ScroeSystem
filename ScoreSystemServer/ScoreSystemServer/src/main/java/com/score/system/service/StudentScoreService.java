package com.score.system.service;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.trend.StudentScoreTrendResult;

import java.util.Map;

public interface StudentScoreService {
    ResponseResult<StudentScoreTrendResult> getStudentScoreTrend(String studentNumber, Integer grade);
    ResponseResult<Map<Long, Double>> getStudentExam312Scores(String studentNumber, Integer grade);
}
