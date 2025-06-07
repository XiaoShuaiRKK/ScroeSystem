package com.score.system.controller;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.ExamClassSubjectStat;
import com.score.system.entity.school.ExamDTO;
import com.score.system.entity.school.ExamSubjectThreshold;
import com.score.system.service.CourseService;
import com.score.system.service.ExamClassSubjectStatService;
import jakarta.validation.Valid;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/exam")
@Validated
public class ExamController {
    private final CourseService courseService;
    private final ExamClassSubjectStatService examClassSubjectStatService;

    public ExamController(CourseService courseService, ExamClassSubjectStatService examClassSubjectStatService) {
        this.courseService = courseService;
        this.examClassSubjectStatService = examClassSubjectStatService;
    }

    /**
     * 添加考试
     */
    @PostMapping("/add")
    public ResponseResult<Boolean> addExam(@RequestBody @Valid ExamDTO examDTO) {
        return courseService.addExam(examDTO);
    }

    /**
     * 获取考试列表
     */
    @GetMapping("/list")
    public ResponseResult<List<ExamDTO>> getExams() {
        return courseService.getExams();
    }

    @PostMapping("/batchAdd")
    public ResponseResult<Boolean> batchAddExams(@RequestBody @Valid List<ExamDTO> examDTOList) {
        return courseService.batchAddExam(examDTOList);
    }

    @PostMapping("/delete")
    public ResponseResult<Boolean> deleteExam(@RequestBody @Valid ExamDTO examDTO) {
        return courseService.deleteExam(examDTO);
    }

    /**
     * 添加一条达标线
     */
    @PostMapping("/threshold/add")
    public ResponseResult<Boolean> addThreshold(@RequestBody ExamSubjectThreshold threshold) {
        return courseService.addExamSubjectThreshold(threshold);
    }

    /**
     * 批量添加达标线
     */
    @PostMapping("/threshold/batchAdd")
    public ResponseResult<Boolean> batchAddThreshold(@RequestBody List<ExamSubjectThreshold> thresholds) {
        return courseService.batchAddExamSubjectThreshold(thresholds);
    }

    /**
     * 查询某次考试的所有达标线
     */
    @GetMapping("/threshold/list")
    public ResponseResult<List<ExamSubjectThreshold>> getThresholds(@RequestParam("exam_id") Long examId) {
        return courseService.getExamSubjectThresholds(examId.intValue());
    }

    @PostMapping("/stat/generate")
    public ResponseResult<Boolean> generateExamStat(@RequestParam("exam_id")Integer examId){
        return examClassSubjectStatService.generateExamStat(examId);
    }

    @GetMapping("/stat/get")
    public ResponseResult<List<ExamClassSubjectStat>> getExamClassSubjectStat(@RequestParam("exam_id")Integer examId,
                                                                              @RequestParam("subject_group_id")Integer subjectGroupId){
        return examClassSubjectStatService.getExamClassSubjectStat(examId,subjectGroupId);
    }
}
