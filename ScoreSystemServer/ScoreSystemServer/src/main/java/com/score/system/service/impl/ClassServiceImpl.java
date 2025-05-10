package com.score.system.service.impl;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.ClassConverter;
import com.score.system.entity.school.ClassDTO;
import com.score.system.entity.school.ClassEntity;
import com.score.system.mapper.ClassMapper;
import com.score.system.mapper.TeacherMapper;
import com.score.system.service.ClassService;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;

@Service
public class ClassServiceImpl implements ClassService {
    private final ClassMapper classMapper;
    private final TeacherMapper teacherMapper;

    public ClassServiceImpl(ClassMapper classMapper, TeacherMapper teacherMapper) {
        this.classMapper = classMapper;
        this.teacherMapper = teacherMapper;
    }

    @Override
    public ResponseResult<Boolean> addClass(ClassDTO classDTO) {
        Long teacherId = classDTO.getHeadTeacherId();
        if(teacherMapper.selectById(teacherId) == null){
            return ResponseResult.fail("老师ID " + teacherId + "不存在");
        }
        if(classMapper.selectByClassName(classDTO.getName()) != null){
            return ResponseResult.fail("班级名称 " + classDTO.getName() + "已存在");
        }
        ClassEntity classEntity = ClassConverter.toEntity(classDTO);
        int result = classMapper.insert(classEntity);
        return result > 0
                ? ResponseResult.success("导入成功",true)
                : ResponseResult.fail("导入失败");
    }

    @Override
    public ResponseResult<Boolean> batchAddClass(List<ClassDTO> classDTOList) {
        List<ClassEntity> classEntities = new ArrayList<>();
        for (ClassDTO classDTO : classDTOList){
            if(teacherMapper.selectById(classDTO.getHeadTeacherId()) == null){
                return ResponseResult.fail("老师ID " + classDTO.getHeadTeacherId() + "不存在");
            }
            if(classMapper.selectByClassName(classDTO.getName()) != null){
                return ResponseResult.fail("班级名称 " + classDTO.getName() + "已存在");
            }
            classEntities.add(ClassConverter.toEntity(classDTO));
        }
        int result = classMapper.batchInsertClass(classEntities);
        return result > 0
                ? ResponseResult.success("导入成功",true)
                : ResponseResult.fail("导入失败");
    }
}
