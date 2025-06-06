package com.score.system.entity.user;

import lombok.Data;

import java.time.LocalDate;
import java.time.LocalDateTime;

@Data
public class StudentVO {
    private String name;
    private String studentNumber;
    private Integer classId;
    private Long state;
    private LocalDate enrollmentDate;
    private Long subjectGroupId;
    private Long electiveCourse1Id;
    private Long electiveCourse2Id;
}
