package com.score.system.service;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.CriticalConfig;
import com.score.system.entity.school.CriticalStudentLog;

import java.util.List;

public interface CriticalStudentService {
    ResponseResult<Boolean> addCriticalConfig(CriticalConfig criticalConfig);
    ResponseResult<Boolean> batchAddCriticalConfig(List<CriticalConfig> criticalConfigList);
    ResponseResult<Boolean> updateCriticalConfig(CriticalConfig criticalConfig);
    ResponseResult<Boolean> batchUpdateCriticalConfig(List<CriticalConfig> criticalConfigList);
    ResponseResult<Boolean> deleteCriticalConfig(CriticalConfig criticalConfig);
    ResponseResult<List<CriticalConfig>> getAllCriticalConfig();
    ResponseResult<List<CriticalConfig>> getCriticalConfigByGrade(int examId);
    ResponseResult<String> generateCriticalStudents(int grade, int year);
    ResponseResult<List<CriticalStudentLog>> getAllCriticalStudentLog(int grade,int year);
    ResponseResult<List<CriticalStudentLog>> getAllCriticalStudentByGradeAndYear(int grade,int year,int examId);
}
