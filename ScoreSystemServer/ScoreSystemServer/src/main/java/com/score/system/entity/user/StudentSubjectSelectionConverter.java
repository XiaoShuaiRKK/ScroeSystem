package com.score.system.entity.user;

public class StudentSubjectSelectionConverter {
    public static StudentSubjectSelectionDTO toDTO(StudentSubjectSelection studentSubjectSelection) {
        if(studentSubjectSelection == null) return null;
        StudentSubjectSelectionDTO studentSubjectSelectionDTO = new StudentSubjectSelectionDTO();
        studentSubjectSelectionDTO.setStudentNumber(studentSubjectSelection.getStudentNumber());
        studentSubjectSelectionDTO.setSubjectGroupId(studentSubjectSelection.getSubjectGroupId());
        studentSubjectSelectionDTO.setElectiveCourse1Id(studentSubjectSelection.getElectiveCourse1Id());
        studentSubjectSelectionDTO.setElectiveCourse2Id(studentSubjectSelection.getElectiveCourse2Id());
        return studentSubjectSelectionDTO;
    }

    public static StudentSubjectSelection toEntity(StudentSubjectSelectionDTO studentSubjectSelectionDTO) {
        if(studentSubjectSelectionDTO == null) return null;
        StudentSubjectSelection studentSubjectSelection = new StudentSubjectSelection();
        studentSubjectSelection.setStudentNumber(studentSubjectSelectionDTO.getStudentNumber());
        studentSubjectSelection.setSubjectGroupId(studentSubjectSelectionDTO.getSubjectGroupId());
        studentSubjectSelection.setElectiveCourse1Id(studentSubjectSelectionDTO.getElectiveCourse1Id());
        studentSubjectSelection.setElectiveCourse2Id(studentSubjectSelectionDTO.getElectiveCourse2Id());
        return studentSubjectSelection;
    }
}
