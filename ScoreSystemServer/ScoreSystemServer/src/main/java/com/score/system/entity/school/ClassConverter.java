package com.score.system.entity.school;

public class ClassConverter {
    public static ClassEntity toEntity(ClassDTO classDTO){
        if (classDTO == null) return null;
        ClassEntity classEntity = new ClassEntity();
        classEntity.setId(classDTO.getId());
        classEntity.setGrade(classDTO.getGrade());
        classEntity.setName(classDTO.getName());
        classEntity.setHeadTeacherId(classDTO.getHeadTeacherId());
        classEntity.setSubjectGroupId(classDTO.getSubjectGroupId());
        return classEntity;
    }

    public static ClassDTO toDTO(ClassEntity classEntity, String teacherName){
        if (classEntity == null) return null;
        ClassDTO classDTO = new ClassDTO();
        classDTO.setId(classEntity.getId());
        classDTO.setGrade(classEntity.getGrade());
        classDTO.setName(classEntity.getName());
        classDTO.setHeadTeacherId(classEntity.getHeadTeacherId());
        classDTO.setSubjectGroupId(classEntity.getSubjectGroupId());
        classDTO.setTeacherName(teacherName);
        return classDTO;
    }
}
