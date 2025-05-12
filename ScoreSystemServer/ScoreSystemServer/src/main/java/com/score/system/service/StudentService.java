package com.score.system.service;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.user.StudentSubjectSelectionDTO;

import java.util.List;

public interface StudentService {
    ResponseResult<Boolean> saveStudentSelection(StudentSubjectSelectionDTO dto);
    ResponseResult<Boolean> batchSaveStudentSelection(List<StudentSubjectSelectionDTO> dtoList);
}
