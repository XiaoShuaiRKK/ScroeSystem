package com.score.system.entity.rank;

import lombok.Data;

@Data
public class GradeRankingDTO {
    private String studentNumber;
    private Long courseId;
    private Double score;
    private int gradeRank;
    private int totalStudents;
}
