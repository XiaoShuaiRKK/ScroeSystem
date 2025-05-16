package com.score.system.mapper;

import com.baomidou.mybatisplus.core.mapper.BaseMapper;
import com.score.system.entity.school.ExamSubjectThreshold;
import org.apache.ibatis.annotations.Mapper;
import org.apache.ibatis.annotations.Select;

import java.util.List;

@Mapper
public interface ExamSubjectThresholdMapper extends BaseMapper<ExamSubjectThreshold> {
    @Select(" SELECT * FROM exam_subject_threshold WHERE exam_id = #{examId}")
    List<ExamSubjectThreshold> selectByExamId(int examId);
}
