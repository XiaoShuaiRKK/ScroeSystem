package com.score.system.controller;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.ClassDTO;
import com.score.system.service.ClassService;
import jakarta.validation.Valid;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/class")
@Validated
public class ClassController {
    private final ClassService classService;

    public ClassController(ClassService classService) {
        this.classService = classService;
    }

    // 添加单个班级
    @PostMapping("/add")
    public ResponseResult<Boolean> addClass(@RequestBody @Valid ClassDTO classDTO) {
        return classService.addClass(classDTO);
    }

    // 批量添加班级
    @PostMapping("/batchAdd")
    public ResponseResult<Boolean> batchAddClass(@RequestBody @Valid List<@Valid ClassDTO> classDTOList) {
        return classService.batchAddClass(classDTOList);
    }

    @PostMapping("/update")
    public ResponseResult<Boolean> updateClass(@RequestBody @Valid ClassDTO classDTO) {
        return classService.updateClass(classDTO);
    }

    @GetMapping("/getAll")
    public ResponseResult<List<ClassDTO>> getAllClasses(){
        return classService.getAllClasses();
    }

    @PostMapping("/delete")
    public ResponseResult<Boolean> deleteClass(@RequestBody ClassDTO classDTO) {
        return classService.deleteClass(classDTO);
    }

    @PostMapping("/up/grade")
    public ResponseResult<Boolean> upGrade(@RequestParam("grade")int grade) {
        return classService.upGrade(grade);
    }
}
