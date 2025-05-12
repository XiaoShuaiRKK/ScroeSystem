package com.score.system.controller;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.user.StudentSubjectSelection;
import com.score.system.entity.user.StudentSubjectSelectionDTO;
import com.score.system.service.StudentService;
import jakarta.validation.Valid;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.List;

@RestController
@RequestMapping("/student")
@Validated
public class StudentController {
    private final StudentService studentService;

    public StudentController(StudentService studentService) {
        this.studentService = studentService;
    }

    @PostMapping("/subject/group/save")
    public ResponseResult<Boolean> saveStudentSubjectGroup(@RequestBody @Valid StudentSubjectSelectionDTO studentSubjectSelectionDTO) {
        return studentService.saveStudentSelection(studentSubjectSelectionDTO);
    }

    @PostMapping("/subject/group/batchSave")
    public ResponseResult<Boolean> batchSaveStudentSubjectGroup(@RequestBody @Valid List<StudentSubjectSelectionDTO> studentSubjectSelectionList) {
        return studentService.batchSaveStudentSelection(studentSubjectSelectionList);
    }
}
