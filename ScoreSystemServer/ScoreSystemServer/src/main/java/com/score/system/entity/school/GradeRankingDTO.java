package com.score.system.entity.school;

import lombok.Data;

@Data
public class GradeRankingDTO {
    private String studentNumber;
    private Long courseId;
    private Integer score;
    private int gradeRank;
    private int totalStudents;
}
