package com.score.system.entity.user;

import com.score.system.entity.school.Score;

import java.time.LocalDateTime;
import java.util.List;

public class StudentConverter {
    public static StudentDTO toDTO(Student student) {
        if (student == null) return null;
        StudentDTO studentDTO = new StudentDTO();
        studentDTO.setStudentNumber(student.getStudentNumber());
        studentDTO.setName(student.getName());
        studentDTO.setState(student.getState());
        studentDTO.setEnrollmentDate(student.getEnrollmentDate());
        studentDTO.setClassId(student.getClassId());
        return studentDTO;
    }

    public static Student toEntity(StudentDTO studentDTO,Long userId) {
        if (studentDTO == null) return null;
        Student student = new Student();
        student.setStudentNumber(studentDTO.getStudentNumber());
        student.setName(studentDTO.getName());
        student.setState(studentDTO.getState());
        student.setEnrollmentDate(studentDTO.getEnrollmentDate());
        student.setClassId(studentDTO.getClassId());
        student.setUserId(userId);
        student.setCreatedAt(LocalDateTime.now());
        student.setUpdatedAt(LocalDateTime.now());
        return student;
    }

    public static StudentScoreVO toScoreVO(Student student,String className, List<Score> scores){
        StudentScoreVO studentScoreVO = new StudentScoreVO();
        studentScoreVO.setStudentNumber(student.getStudentNumber());
        studentScoreVO.setName(student.getName());
        studentScoreVO.setClassName(className);
        studentScoreVO.setScores(scores);
        return studentScoreVO;
    }
}
