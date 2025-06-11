package com.score.system.controller;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.rank.ClassRankingDTO;
import com.score.system.entity.rank.GradeRankingDTO;
import com.score.system.entity.rank.StudentRanking;
import com.score.system.entity.school.Score;
import com.score.system.service.ScoreService;
import jakarta.validation.Valid;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/score")
@Validated
public class ScoreController {
    private final ScoreService scoreService;

    public ScoreController(@Valid @RequestBody ScoreService scoreService) {
        this.scoreService = scoreService;
    }

    @PostMapping("/add")
    public ResponseResult<Boolean> addScore(Score score) {
        return scoreService.addScore(score);
    }

    @PostMapping("/batchAdd")
    public ResponseResult<Boolean> batchAddScore(@Valid @RequestBody List<Score> scores) {
        return scoreService.batchAddScores(scores);
    }

    @PostMapping("/batchUpdate")
    public ResponseResult<Boolean> batchUpdateScore(@Valid @RequestBody List<Score> scores) {
        return scoreService.batchUpdateScores(scores);
    }

    @GetMapping("/rankings/class")
    public ResponseResult<List<ClassRankingDTO>> calculateClassRankings(@RequestParam("exam_id") Long examId,
                                                                        @RequestParam("student_number")String studentNumber) {
        return scoreService.calculateClassRankings(studentNumber, examId);
    }

    @GetMapping("/rankings/grade")
    public ResponseResult<List<GradeRankingDTO>> calculateGradeRankings(@RequestParam("exam_id") Long examId,
                                                                        @RequestParam("student_number")String studentNumber){
        return scoreService.calculateGradeRankings(studentNumber, examId);
    }

    @GetMapping("/rankings/total/class")
    public ResponseResult<ClassRankingDTO> calculateTotalScoreClassRankings(@RequestParam("exam_id")Long examId,
                                                                            @RequestParam("student_number")String studentNumber){
        return scoreService.calculateTotalScoreClassRankings(studentNumber, examId);
    }

    @GetMapping("/rankings/total/grade")
    public ResponseResult<GradeRankingDTO> calculateTotalScoreGradeRankings(@RequestParam("exam_id")Long examId,
                                                                            @RequestParam("student_number")String studentNumber){
        return scoreService.calculateTotalScoreGradeRankings(studentNumber, examId);
    }

    @GetMapping("/rankings/student")
    public ResponseResult<StudentRanking> getStudentAllRankings(@RequestParam("exam_id")Long examId,
                                                                @RequestParam("student_number")String studentNumber){
        return scoreService.getStudentAllRankings(studentNumber, examId);
    }

    @GetMapping("/rankings/student/total/byClass")
    public ResponseResult<List<StudentRanking>> getClassTotalRankings(@RequestParam("class_id") Integer classId,
                                                                      @RequestParam("exam_id") Long examId){
        return scoreService.getClassTotalRankings(classId, examId);
    }

    @GetMapping("/rankings/student/total/byGrade")
    public ResponseResult<List<StudentRanking>> getGradeTotalRankings(@RequestParam("grade") Integer gradeId,
                                                               @RequestParam("exam_id") Long examId){
        return scoreService.getGradeTotalRankings(gradeId, examId);
    }

    @GetMapping("/rankings/course/grade")
    public ResponseResult<List<StudentRanking>> getGradeCourseRankings(@RequestParam("grade") Integer grade,
                                                                       @RequestParam("exam_id") Long examId){
        return scoreService.getGradeCourseRankings(grade, examId);
    }

    @GetMapping("/rankings/course/class")
    public ResponseResult<List<StudentRanking>> getClassCourseRankings(@RequestParam("class_id") Integer classId,
                                                                       @RequestParam("exam_id") Long examId){
        return scoreService.getClassCourseRankings(classId, examId);
    }

    @GetMapping("/rankings/3-course/class")
    public ResponseResult<List<StudentRanking>> getClassTop3CourseRankings(@RequestParam("class_id") Integer classId,
                                                                           @RequestParam("exam_id")Long examId){
        return scoreService.getClassTop3CourseTotalRankings(classId, examId);
    }

    @GetMapping("/rankings/3-course/grade")
    public ResponseResult<List<StudentRanking>> getGradeTop3CourseTotalRankings(@RequestParam("grade") Integer grade,
                                                                                @RequestParam("exam_id") Long examId){
        return scoreService.getGradeTop3CourseTotalRankings(grade, examId);
    }

    @GetMapping("/rankings/group/one/class")
    public ResponseResult<List<StudentRanking>> getClassRankingWithGroupCourse(@RequestParam("class_id") Integer classId,
                                                                               @RequestParam("exam_id") Long examId){
        return scoreService.getClassRankingWithGroupCourse(classId, examId);
    }

    @GetMapping("/rankings/group/one/grade")
    public ResponseResult<List<StudentRanking>> getGradeRankingWithGroupCourse(@RequestParam("grade") Integer grade,
                                                                               @RequestParam("exam_id") Long examId,
                                                                               @RequestParam("subject_group_id")Integer subjectGroupId){
        return scoreService.getGradeRankingWithGroupCourse(grade, examId, subjectGroupId);
    }

    @GetMapping("/rankings/group/312/class")
    public ResponseResult<List<StudentRanking>> getClassRankingWith312(@RequestParam("class_id") Integer classId,
                                                                       @RequestParam("exam_id") Long examId){
        return scoreService.getClassRankingWith312(classId, examId);
    }

    @GetMapping("/rankings/group/312/grade")
    public ResponseResult<List<StudentRanking>> getGradeRankingWith312(@RequestParam("grade") Integer grade,
                                                                       @RequestParam("exam_id") Long examId,
                                                                       @RequestParam("subject_group_id") Integer subjectGroupId){
        return scoreService.getGradeRankingWith312(grade, examId, subjectGroupId);
    }

    @GetMapping("/rankings/class/all")
    public ResponseResult<List<StudentRanking>> GetClassRankingAll(@RequestParam("class_id") Integer classId,
                                                                   @RequestParam("exam_id") Long examId){
        return scoreService.getClassAllRankings(classId, examId);
    }

    @GetMapping("/rankings/grade/critical")
    public ResponseResult<List<StudentRanking>> GetGradeRankingBySubjectGroup(@RequestParam("exam_id")Long examId,
                                                                              @RequestParam("subject_group_id")Integer subjectGroupId){
        return scoreService.getGradeSubjectGroupRankings(examId, subjectGroupId);
    }
}
