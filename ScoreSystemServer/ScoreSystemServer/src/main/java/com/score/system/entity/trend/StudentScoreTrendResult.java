package com.score.system.entity.trend;

import lombok.Data;

import java.util.List;
import java.util.Map;

@Data
public class StudentScoreTrendResult {
    private String studentName;
    private String studentNumber;
    private Map<String,List<ScoreTrendPoint>> trend;
}
