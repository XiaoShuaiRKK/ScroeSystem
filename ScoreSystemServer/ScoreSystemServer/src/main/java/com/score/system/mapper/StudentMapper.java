package com.score.system.mapper;

import com.score.system.entity.user.Student;
import org.apache.ibatis.annotations.Mapper;
import org.apache.ibatis.annotations.Param;
import org.apache.ibatis.annotations.Select;

import java.util.List;
import java.util.Set;

@Mapper
public interface StudentMapper extends MyBaseMapper<Student> {
    void batchInsert(@Param("list") List<Student> students);
    @Select("select * from student where student_number = #{number}")
    Student selectStudentByNumber(String number);
    List<Student> selectBatchByStudentNumbers(@Param("numbers") Set<String> numbers);
}
