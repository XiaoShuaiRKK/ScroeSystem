package com.score.system.service.impl;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.Score;
import com.score.system.mapper.CourseMapper;
import com.score.system.mapper.ExamMapper;
import com.score.system.mapper.ScoreMapper;
import com.score.system.mapper.StudentMapper;
import com.score.system.service.ScoreService;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

@Service
public class ScoreServiceImpl implements ScoreService {
    private final ScoreMapper scoreMapper;
    private final StudentMapper studentMapper;
    private final CourseMapper courseMapper;
    private final ExamMapper examMapper;

    public ScoreServiceImpl(ScoreMapper scoreMapper, StudentMapper studentMapper, CourseMapper courseMapper, ExamMapper examMapper) {
        this.scoreMapper = scoreMapper;
        this.studentMapper = studentMapper;
        this.courseMapper = courseMapper;
        this.examMapper = examMapper;
    }

    @Override
    @Transactional
    public ResponseResult<Boolean> addScore(Score score) {
        if(studentMapper.selectStudentByNumber(score.getStudentNumber()) == null){
            throw new IllegalArgumentException("学号： " + score.getStudentNumber() + " 不存在");
        }
        if(courseMapper.selectById(score.getCourseId()) == null){
            throw new IllegalArgumentException("科目： " + score.getCourseId() + " 不存在");
        }
        if(examMapper.selectById(score.getExamId()) == null){
            throw new IllegalArgumentException("考试不存在: " + score.getExamId());
        }
        int result = scoreMapper.insert(score);
        if(result <= 0){
            throw new RuntimeException("成绩添加失败");
        }
        return ResponseResult.success("成绩添加成功", true);
    }

    @Override
    @Transactional(rollbackFor = Exception.class)
    public ResponseResult<Boolean> batchAddScores(List<Score> scores) {
        try {
            for(Score score : scores){
                addScore(score);
            }
        }catch (IllegalArgumentException e){
            return ResponseResult.fail(e.getMessage());
        }
        return ResponseResult.success("批量添加成功",true);
    }
}
