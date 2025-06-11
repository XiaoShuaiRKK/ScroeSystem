package com.score.system.service;

import com.score.system.entity.ResponseResult;
import com.score.system.entity.rank.ClassRankingDTO;
import com.score.system.entity.rank.GradeRankingDTO;
import com.score.system.entity.rank.StudentRanking;
import com.score.system.entity.school.Score;

import java.util.List;

public interface ScoreService {
    ResponseResult<Boolean> addScore(Score score);
    ResponseResult<Boolean> batchAddScores(List<Score> scores);
    ResponseResult<Boolean> updateScore(Score score);
    ResponseResult<Boolean> batchUpdateScores(List<Score> scores);
    ResponseResult<List<ClassRankingDTO>> calculateClassRankings(String studentNumber, Long examId);
    ResponseResult<List<GradeRankingDTO>> calculateGradeRankings(String studentNumber, Long examId);
    ResponseResult<ClassRankingDTO> calculateTotalScoreClassRankings(String studentNumber, Long examId);
    ResponseResult<GradeRankingDTO> calculateTotalScoreGradeRankings(String studentNumber, Long examId);
    ResponseResult<StudentRanking> getStudentAllRankings(String studentNumber, Long examId);
    ResponseResult<List<StudentRanking>> getClassTotalRankings(Integer classId, Long examId);
    ResponseResult<List<StudentRanking>> getGradeTotalRankings(Integer gradeId, Long examId);
    ResponseResult<List<StudentRanking>> getClassCourseRankings(Integer classId, Long examId);
    ResponseResult<List<StudentRanking>> getGradeCourseRankings(Integer gradeId, Long examId);
    ResponseResult<List<StudentRanking>> getClassTop3CourseTotalRankings(Integer classId, Long examId);
    ResponseResult<List<StudentRanking>> getGradeTop3CourseTotalRankings(Integer gradeId, Long examId);
    ResponseResult<List<StudentRanking>> getClassRankingWithGroupCourse(Integer classId, Long examId);
    ResponseResult<List<StudentRanking>> getGradeRankingWithGroupCourse(Integer gradeId, Long examId,Integer subjectGroupId);
    ResponseResult<List<StudentRanking>> getClassRankingWith312(Integer classId, Long examId);
    ResponseResult<List<StudentRanking>> getGradeRankingWith312(Integer gradeId, Long examId, Integer subjectGroupId);
    ResponseResult<List<StudentRanking>> getClassAllRankings(Integer classId, Long examId);
    ResponseResult<List<StudentRanking>> getGradeSubjectGroupRankings(Long examId,Integer subjectGroupId);
}
