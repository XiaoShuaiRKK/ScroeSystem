package com.score.system.entity.rank;

import lombok.Data;

import java.util.List;

@Data
public class StudentRanking {
    private String studentNumber;
    private String studentName;
    private List<Ranking> ranks;
}
