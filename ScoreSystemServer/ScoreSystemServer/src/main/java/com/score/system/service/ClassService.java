package com.score.system.service;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.ClassDTO;

import java.util.List;

public interface ClassService {
    ResponseResult<Boolean> addClass(ClassDTO classDTO);
    ResponseResult<Boolean> batchAddClass(List<ClassDTO> classDTOList);
    ResponseResult<List<ClassDTO>> getAllClasses();
    ResponseResult<Boolean> updateClass(ClassDTO classDTO);
}
