package com.score.system.service;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.user.Student;
import com.score.system.entity.user.StudentDTO;
import com.score.system.entity.user.StudentScoreVO;

import java.util.List;

public interface TeacherService {
    ResponseResult<List<StudentDTO>> selectStudentsByClass(Long teacherId);
    ResponseResult<StudentDTO> selectStudentByTeacherId(Long teacherId,String studentNumber);
    ResponseResult<List<StudentScoreVO>> selectStudentScoreByTeacherId(Long teacherId,Long classId,Long examId);
    ResponseResult<StudentScoreVO> selectStudentScoreByStudentId(Long studentId,String studentNumber,Long examId);
}
