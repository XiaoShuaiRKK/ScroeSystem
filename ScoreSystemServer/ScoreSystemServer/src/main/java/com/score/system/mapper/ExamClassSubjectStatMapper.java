package com.score.system.mapper;

import com.score.system.entity.school.ExamClassSubjectStat;
import org.apache.ibatis.annotations.Mapper;
import org.apache.ibatis.annotations.Param;

import java.util.List;

@Mapper
public interface ExamClassSubjectStatMapper extends MyBaseMapper<ExamClassSubjectStat> {
    int updateBatchById(@Param("list") List<ExamClassSubjectStat> list);
}
