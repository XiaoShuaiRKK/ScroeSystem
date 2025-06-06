package com.score.system.service;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.user.StudentClassHistory;
import com.score.system.entity.user.StudentDTO;
import com.score.system.entity.user.StudentScoreVO;
import com.score.system.entity.user.StudentVO;

import java.util.List;

public interface TeacherService {
    ResponseResult<List<StudentVO>> selectStudentsByClass(Long classId);
    ResponseResult<StudentVO> selectStudentByTeacherId(String studentNumber);
    ResponseResult<List<StudentScoreVO>> selectStudentScoreByTeacherId(Long classId,Long examId);
    ResponseResult<StudentScoreVO> selectStudentScoreByStudentId(String studentNumber,Long examId);
    ResponseResult<List<StudentClassHistory>> selectStudentByStudentNumber(String studentNumber);
    ResponseResult<List<StudentVO>> selectStudentNumberByName(String studentName);
}
