package com.score.system.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.university.University;
import com.score.system.mapper.UniversityMapper;
import com.score.system.service.UniversityService;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.List;

@Service
public class UniversityServiceImpl implements UniversityService {
    private final UniversityMapper universityMapper;

    public UniversityServiceImpl(UniversityMapper universityMapper) {
        this.universityMapper = universityMapper;
    }

    @Override
    public ResponseResult<Boolean> addUniversity(University university) {
        LambdaQueryWrapper<University> query = new LambdaQueryWrapper<University>()
                .eq(University::getName, university.getName())
                .eq(University::getYear, university.getYear());
        if(universityMapper.selectCount(query) > 0){
            return ResponseResult.fail("此大学此年份已被录入",false);
        }
        int result = universityMapper.insert(university);
        return result <= 0 ? ResponseResult.fail("添加失败",false) : ResponseResult.success("添加成功",true);
    }

    @Override
    @Transactional(rollbackFor = Exception.class)
    public ResponseResult<Boolean> batchAddUniversity(List<University> universities) {
        for(University university : universities){
            ResponseResult<Boolean> result = addUniversity(university);
            if(result.getCode() != 200){
                throw new IllegalArgumentException(result.getMessage());
            }
        }
        return ResponseResult.success("批量添加成功",true);
    }

    @Override
    public ResponseResult<List<University>> getAllUniversity() {
        return ResponseResult.success(universityMapper.selectList(null));
    }
}
