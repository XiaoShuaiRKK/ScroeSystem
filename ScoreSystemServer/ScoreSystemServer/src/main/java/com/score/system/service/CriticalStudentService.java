package com.score.system.service;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.CriticalConfig;
import com.score.system.entity.school.CriticalStudentLog;

import java.util.List;

public interface CriticalStudentService {
    ResponseResult<Boolean> addCriticalConfig(CriticalConfig criticalConfig);
    ResponseResult<Boolean> batchAddCriticalConfig(List<CriticalConfig> criticalConfigList);
    ResponseResult<List<CriticalConfig>> getAllCriticalConfig();
    ResponseResult<String> generateCriticalStudents(int grade, int year);
    ResponseResult<List<CriticalStudentLog>> getAllCriticalStudentLog(int grade,int year);
}
