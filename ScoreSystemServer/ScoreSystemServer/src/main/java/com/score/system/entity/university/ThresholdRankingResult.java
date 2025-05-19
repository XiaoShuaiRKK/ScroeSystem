package com.score.system.entity.university;

import com.score.system.entity.rank.StudentRanking;
import lombok.Data;

import java.util.List;
import java.util.Map;

@Data
public class ThresholdRankingResult {
    Map<String,Integer> levelCounts;
    Map<String,Double> levelRates;
    Map<String, List<StudentRanking>> levelStudentList;
}
