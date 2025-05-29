package com.score.system.mapper;

import com.score.system.entity.user.StudentSubjectSelection;
import org.apache.ibatis.annotations.Mapper;
import org.apache.ibatis.annotations.Param;

import java.util.List;
import java.util.Set;

@Mapper
public interface StudentSubjectSelectionMapper extends MyBaseMapper<StudentSubjectSelection> {
    // StudentSubjectSelectionMapper.java
    List<StudentSubjectSelection> selectBatchByStudentNumbers(@Param("numbers") Set<String> numbers);
}
