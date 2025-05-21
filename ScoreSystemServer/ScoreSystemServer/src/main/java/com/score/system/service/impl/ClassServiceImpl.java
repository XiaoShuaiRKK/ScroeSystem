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

    @Override
    public ResponseResult<List<ClassDTO>> getAllClasses() {
        List<ClassEntity> classEntities = classMapper.selectList(null);
        if (classEntities == null) return ResponseResult.success(new ArrayList<>());
        List<ClassDTO> classDTOList = new ArrayList<>();
        for (ClassEntity classEntity : classEntities){
            String name = teacherMapper.selectById(classEntity.getHeadTeacherId()).getName();
            ClassDTO classDTO = ClassConverter.toDTO(classEntity,name);
            classDTOList.add(classDTO);
        }
        return ResponseResult.success(classDTOList);
    }

    @Override
    public ResponseResult<Boolean> updateClass(ClassDTO classDTO) {
        // 1. 校验班主任是否存在
        Long teacherId = classDTO.getHeadTeacherId();
        if (teacherId != null && teacherMapper.selectById(teacherId) == null) {
            return ResponseResult.fail("老师ID " + teacherId + " 不存在");
        }

        // 2. 校验班级是否存在
        ClassEntity existingClass = classMapper.selectById(classDTO.getId());
        if (existingClass == null) {
            return ResponseResult.fail("班级ID " + classDTO.getId() + " 不存在");
        }

        // 3. 校验班级名称是否重复（排除当前班级自身）
        ClassEntity sameNameClass = classMapper.selectByClassName(classDTO.getName());
        if (sameNameClass != null && !sameNameClass.getId().equals(classDTO.getId())) {
            return ResponseResult.fail("班级名称 " + classDTO.getName() + " 已存在");
        }

        // 4. 转换并更新
        ClassEntity updated = ClassConverter.toEntity(classDTO);
        int result = classMapper.updateById(updated);

        return result > 0
                ? ResponseResult.success("更新成功", true)
                : ResponseResult.fail("更新失败");
    }
}
