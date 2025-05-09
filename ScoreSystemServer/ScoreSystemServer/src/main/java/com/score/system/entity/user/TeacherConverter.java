package com.score.system.entity.user;

import java.time.LocalDateTime;

public class TeacherConverter {
    public static TeacherDTO toDTO(Teacher teacher) {
        if (teacher == null) return null;
        TeacherDTO dto = new TeacherDTO();
        dto.setTeacherNumber(teacher.getTeacherNumber());
        dto.setName(teacher.getName());
        dto.setState(teacher.getState());
        dto.setName(teacher.getName());
        return dto;
    }

    public static Teacher toEntity(TeacherDTO dto,Long userId) {
        if (dto == null) return null;
        Teacher teacher = new Teacher();
        teacher.setTeacherNumber(dto.getTeacherNumber());
        teacher.setName(dto.getName());
        teacher.setState(dto.getState());
        teacher.setUserId(userId);
        teacher.setCreatedAt(LocalDateTime.now());
        teacher.setUpdatedAt(LocalDateTime.now());
        return teacher;
    }
}
