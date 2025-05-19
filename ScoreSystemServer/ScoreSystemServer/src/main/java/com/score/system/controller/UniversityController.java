package com.score.system.controller;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.university.University;
import com.score.system.service.UniversityService;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/university")
@Validated
public class UniversityController {
    private final UniversityService universityService;

    public UniversityController(UniversityService universityService) {
        this.universityService = universityService;
    }

    @PostMapping("/add")
    public ResponseResult<Boolean> addUniversity(@RequestBody @Validated University university) {
        return universityService.addUniversity(university);
    }

    @PostMapping("/batchAdd")
    public ResponseResult<Boolean> batchAddUniversity(@RequestBody @Validated List<University> universityList) {
        return universityService.batchAddUniversity(universityList);
    }

    @GetMapping("/getAll")
    public ResponseResult<List<University>> getUniversityList() {
        return universityService.getAllUniversity();
    }
}
