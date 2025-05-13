package com.score.system.mapper;

import com.baomidou.mybatisplus.core.mapper.BaseMapper;
import com.score.system.entity.school.Score;
import org.apache.ibatis.annotations.Mapper;
import org.apache.ibatis.annotations.Select;

import java.util.List;

@Mapper
public interface ScoreMapper extends BaseMapper<Score> {
    @Select("select * from scores where exam_id = #{examId} and student_number = #{studentNumber}")
    List<Score> selectByExamAndStudentNumber(Long examId,String studentNumber);
    @Select("""
            select 
                s.*
            from scores s
            join student stu on s.student_number = stu.student_number
            where s.exam_id = #{examId} and s.course_id = #{courseId} and stu.class_id = #{classId}
            """)
    List<Score> selectByExamCourseAndClassId(Long examId,Long courseId,int classId);
}
