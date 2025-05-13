package com.score.system.entity.school;

import lombok.Data;

@Data
public class ClassRankingDTO {
    private Long courseId;
    private Integer score;
    private Integer rank;
    private Long totalInClass;
}
