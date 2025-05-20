package com.score.system.controller;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.CriticalConfig;
import com.score.system.service.CriticalStudentService;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/critical")
public class CriticalController {
    private final CriticalStudentService criticalStudentService;

    public CriticalController(CriticalStudentService criticalStudentService) {
        this.criticalStudentService = criticalStudentService;
    }

    @PostMapping("/config/add")
    public ResponseResult<Boolean> addCriticalConfig(@Validated @RequestBody CriticalConfig criticalConfig) {
        return criticalStudentService.addCriticalConfig(criticalConfig);
    }

    @PostMapping("/config/batchAdd")
    public ResponseResult<Boolean> batchAddCriticalConfig(@Validated @RequestBody List<CriticalConfig> criticalConfigs) {
        return criticalStudentService.batchAddCriticalConfig(criticalConfigs);
    }

    @GetMapping("/config/get")
    public ResponseResult<List<CriticalConfig>> getAllCriticalConfig() {
        return criticalStudentService.getAllCriticalConfig();
    }

    @GetMapping("/generate")
    public ResponseResult<String> generateCriticalStudents(@RequestParam("grade") int grade,
                                                           @RequestParam("year") int year){
        return criticalStudentService.generateCriticalStudents(grade, year);
    }
}
