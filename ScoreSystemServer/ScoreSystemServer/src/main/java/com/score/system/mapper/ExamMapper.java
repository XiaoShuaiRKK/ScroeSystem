package com.score.system.mapper;

import com.baomidou.mybatisplus.core.mapper.BaseMapper;
import com.score.system.entity.school.Exam;
import com.score.system.entity.school.ExamDTO;
import org.apache.ibatis.annotations.Mapper;
import org.apache.ibatis.annotations.Param;
import org.apache.ibatis.annotations.Select;

import java.util.List;

@Mapper
public interface ExamMapper extends BaseMapper<Exam> {
    @Select("select * from exams where name = #{examName} and grade = #{grade} limit 1")
    Exam selectByExamName(String examName, int grade);
    int batchInsertExams(@Param("exams") List<Exam> examList);
    @Select("SELECT * FROM exams WHERE name = #{name} AND grade = #{grade} AND year = #{year} LIMIT 1")
    Exam selectByExamNameAndGradeAndYear(String name,int grade,int year);
}
