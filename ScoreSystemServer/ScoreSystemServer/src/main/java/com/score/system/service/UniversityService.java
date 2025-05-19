package com.score.system.service;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.university.University;

import java.util.List;

public interface UniversityService {
    ResponseResult<Boolean> addUniversity(University university);
    ResponseResult<Boolean> batchAddUniversity(List<University> universities);
    ResponseResult<List<University>> getAllUniversity();
}
