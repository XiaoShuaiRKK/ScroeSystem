package com.score.system.mapper;

import com.baomidou.mybatisplus.core.mapper.BaseMapper;
import com.score.system.entity.user.Student;
import org.apache.ibatis.annotations.Mapper;
import org.apache.ibatis.annotations.Param;
import org.apache.ibatis.annotations.Select;

import java.util.List;

@Mapper
public interface StudentMapper extends BaseMapper<Student> {
    void batchInsert(@Param("list") List<Student> students);
    @Select("select * from student where student_number = #{number}")
    Student selectStudentByNumber(String number);
}
