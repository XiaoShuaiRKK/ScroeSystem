package com.score.system.entity.rank;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@AllArgsConstructor
@NoArgsConstructor
@Data
public class Ranking {
    private Long courseId;
    private Double score;
    private Integer rank;
    private Integer total;
    private String scope; //排名范围 (班级、年级)
}
