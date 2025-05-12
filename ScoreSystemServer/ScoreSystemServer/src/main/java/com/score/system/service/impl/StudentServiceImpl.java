package com.score.system.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.user.Student;
import com.score.system.entity.user.StudentSubjectSelection;
import com.score.system.entity.user.StudentSubjectSelectionConverter;
import com.score.system.entity.user.StudentSubjectSelectionDTO;
import com.score.system.mapper.StudentMapper;
import com.score.system.mapper.StudentSubjectSelectionMapper;
import com.score.system.service.StudentService;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

@Service
public class StudentServiceImpl implements StudentService {
    private final StudentSubjectSelectionMapper studentSubjectSelectionMapper;
    private final StudentMapper studentMapper;


    public StudentServiceImpl(StudentSubjectSelectionMapper studentSubjectSelectionMapper, StudentMapper studentMapper) {
        this.studentSubjectSelectionMapper = studentSubjectSelectionMapper;
        this.studentMapper = studentMapper;
    }

    @Override
    @Transactional
    public ResponseResult<Boolean> saveStudentSelection(StudentSubjectSelectionDTO dto) {
        // 检验学号
        LambdaQueryWrapper<Student> queryWrapper = new LambdaQueryWrapper<>();
        queryWrapper.eq(Student::getStudentNumber, dto.getStudentNumber());
        if(studentMapper.selectOne(queryWrapper) == null) {
            throw new IllegalArgumentException("学生学号不存在: " + dto.getStudentNumber());
        }
        StudentSubjectSelection studentSubjectSelection = StudentSubjectSelectionConverter.toEntity(dto);
        //若有记录则更新 否则插入
        LambdaQueryWrapper<StudentSubjectSelection> wrapper = new LambdaQueryWrapper<>();
        wrapper.eq(StudentSubjectSelection::getStudentNumber, studentSubjectSelection.getStudentNumber());
        if(studentSubjectSelectionMapper.selectOne(wrapper) != null){
            studentSubjectSelectionMapper.update(studentSubjectSelection, wrapper);
        }else{
            studentSubjectSelectionMapper.insert(studentSubjectSelection);
        }
        return ResponseResult.success("选科设置成功", true);
    }

    @Override
    @Transactional(rollbackFor = Exception.class)
    public ResponseResult<Boolean> batchSaveStudentSelection(List<StudentSubjectSelectionDTO> dtoList) {
        for(StudentSubjectSelectionDTO dto : dtoList){
            saveStudentSelection(dto);
        }
        return ResponseResult.success("批量设置成功",true);
    }


}
