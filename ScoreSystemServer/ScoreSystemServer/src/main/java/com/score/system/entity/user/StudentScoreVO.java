package com.score.system.entity.user;

import com.score.system.entity.school.Score;
import lombok.Data;

import java.util.List;

@Data
public class StudentScoreVO {
    private String name;
    private String studentNumber;
    private String className;
    private List<Score> scores;
}
