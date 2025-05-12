package com.score.system.service;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.Score;

import java.util.List;

public interface ScoreService {
    ResponseResult<Boolean> addScore(Score score);
    ResponseResult<Boolean> batchAddScores(List<Score> scores);
}
