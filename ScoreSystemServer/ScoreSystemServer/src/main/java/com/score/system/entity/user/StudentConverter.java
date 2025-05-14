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

    public static StudentVO toVO(Student student,StudentSubjectSelection selection) {
        if (student == null) return null;
        if (selection == null) return null;
        StudentVO studentVO = new StudentVO();
        studentVO.setStudentNumber(student.getStudentNumber());
        studentVO.setName(student.getName());
        studentVO.setState(student.getState());
        studentVO.setEnrollmentDate(student.getEnrollmentDate());
        studentVO.setClassId(student.getClassId());
        studentVO.setSubjectGroupId(selection.getSubjectGroupId());
        studentVO.setElectiveCourse1Id(selection.getElectiveCourse1Id());
        studentVO.setElectiveCourse2Id(selection.getElectiveCourse2Id());
        return studentVO;
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
