package com.score.system.entity.school;

public class ClassConverter {
    public static ClassEntity toEntity(ClassDTO classDTO){
        if (classDTO == null) return null;
        ClassEntity classEntity = new ClassEntity();
        classEntity.setGrade(classDTO.getGrade());
        classEntity.setName(classDTO.getName());
        classEntity.setHeadTeacherId(classDTO.getHeadTeacherId());
        classEntity.setSubjectGroupId(classDTO.getSubjectGroupId());
        return classEntity;
    }

    public static ClassDTO toDTO(ClassEntity classEntity){
        if (classEntity == null) return null;
        ClassDTO classDTO = new ClassDTO();
        classDTO.setGrade(classEntity.getGrade());
        classDTO.setName(classEntity.getName());
        classDTO.setHeadTeacherId(classEntity.getHeadTeacherId());
        classDTO.setSubjectGroupId(classEntity.getSubjectGroupId());
        return classDTO;
    }
}
