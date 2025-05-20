package com.score.system.entity.school;

public class ExamConverter {
    public static ExamDTO toDTO(Exam exam) {
        if (exam == null) return null;
        ExamDTO examDTO = new ExamDTO();
        examDTO.setId(exam.getId());
        examDTO.setName(exam.getName());
        examDTO.setGrade(exam.getGrade());
        examDTO.setEndDate(exam.getEndDate());
        examDTO.setStartDate(exam.getStartDate());
        examDTO.setYear(exam.getYear());
        return examDTO;
    }

    public static Exam toEntity(ExamDTO examDTO) {
        if (examDTO == null) return null;
        Exam exam = new Exam();
        exam.setId(examDTO.getId());
        exam.setName(examDTO.getName());
        exam.setGrade(examDTO.getGrade());
        exam.setEndDate(examDTO.getEndDate());
        exam.setStartDate(examDTO.getStartDate());
        exam.setYear(examDTO.getYear());
        return exam;
    }
}
