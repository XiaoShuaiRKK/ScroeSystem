package com.score.system.entity.rank;

import lombok.Data;

@Data
public class ClassRankingDTO {
    private Long courseId;
    private Double score;
    private Integer rank;
    private Long totalInClass;
}
