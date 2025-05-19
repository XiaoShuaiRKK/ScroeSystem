package com.score.system.entity.university;

import lombok.Data;

import java.util.List;

@Data
public class GradeThresholdPredictionResult {
    String studentNumber;
    String studentName;
    List<ThresholdPredictionResult> predictionResults;
}
