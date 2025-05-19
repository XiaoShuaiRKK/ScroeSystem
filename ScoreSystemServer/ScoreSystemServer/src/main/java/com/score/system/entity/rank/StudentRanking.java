package com.score.system.entity.rank;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.List;

@AllArgsConstructor
@NoArgsConstructor
@Data
public class StudentRanking {
    private String studentNumber;
    private String studentName;
    private List<Ranking> ranks;
}
