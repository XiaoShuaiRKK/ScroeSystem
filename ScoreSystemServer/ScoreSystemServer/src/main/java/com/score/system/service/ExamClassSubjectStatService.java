package com.score.system.service;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.ExamClassSubjectStat;

import java.util.List;

public interface ExamClassSubjectStatService {
    ResponseResult<Boolean> generateExamStat(int examId);
    ResponseResult<List<ExamClassSubjectStat>> getExamClassSubjectStat(int examId,int subjectGroupId);
}
