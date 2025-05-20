package com.score.system.entity.trend;

import lombok.Data;

@Data
public class ScoreTrendPoint {
    private Long examId;
    private Double score;
    private Integer rank;
}
