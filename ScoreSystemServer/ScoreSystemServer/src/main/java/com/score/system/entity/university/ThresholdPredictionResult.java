package com.score.system.entity.university;

import lombok.Data;

@Data
public class ThresholdPredictionResult {
    String level;
    int totalExams;
    int qualifiedExams;
    double probability;
}
