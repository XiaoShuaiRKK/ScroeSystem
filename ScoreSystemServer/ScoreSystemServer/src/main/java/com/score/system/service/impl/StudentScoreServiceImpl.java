package com.score.system.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.ClassEntity;
import com.score.system.entity.school.Course;
import com.score.system.entity.school.Exam;
import com.score.system.entity.school.Score;
import com.score.system.entity.trend.ScoreTrendPoint;
import com.score.system.entity.trend.StudentScoreTrendResult;
import com.score.system.entity.user.Student;
import com.score.system.entity.user.StudentClassHistory;
import com.score.system.mapper.*;
import com.score.system.service.StudentScoreService;
import org.springframework.stereotype.Service;

import java.util.*;
import java.util.stream.Collectors;

@Service
public class StudentScoreServiceImpl implements StudentScoreService {
    private final StudentMapper studentMapper;
    private final ScoreMapper scoreMapper;
    private final ExamMapper examMapper;
    private final ClassMapper classMapper;
    private final CourseMapper courseMapper;
    private final StudentClassHistoryMapper studentClassHistoryMapper;

    public StudentScoreServiceImpl(StudentMapper studentMapper, ScoreMapper scoreMapper, ExamMapper examMapper, ClassMapper classMapper, CourseMapper courseMapper, StudentClassHistoryMapper studentClassHistoryMapper) {
        this.studentMapper = studentMapper;
        this.scoreMapper = scoreMapper;
        this.examMapper = examMapper;
        this.classMapper = classMapper;
        this.courseMapper = courseMapper;
        this.studentClassHistoryMapper = studentClassHistoryMapper;
    }

    @Override
    public ResponseResult<StudentScoreTrendResult> getStudentScoreTrend(String studentNumber, Integer grade) {
        // 1. 校验学生
        Student student = studentMapper.selectOne(
                new LambdaQueryWrapper<Student>().eq(Student::getStudentNumber, studentNumber)
        );
        if (student == null) {
            return ResponseResult.fail("未找到该学生");
        }

        // 2. 查找该学生在对应年级的学年记录
        StudentClassHistory history = studentClassHistoryMapper.selectOne(
                new LambdaQueryWrapper<StudentClassHistory>()
                        .eq(StudentClassHistory::getStudentNumber, studentNumber)
                        .eq(StudentClassHistory::getGrade, grade)
        );

        if (history == null) {
            return ResponseResult.fail("未找到该学生在指定年级的历史记录");
        }

        Integer year = history.getYear();

        // 3. 查询对应年级+学年下的考试
        List<Exam> exams = examMapper.selectList(
                new LambdaQueryWrapper<Exam>()
                        .eq(Exam::getGrade, grade)
                        .eq(Exam::getYear, year)
        );

        if (exams.isEmpty()) {
            return ResponseResult.fail("该年级学年无考试数据");
        }

        List<Long> examIds = exams.stream().map(Exam::getId).toList();

        // 4. 查询课程映射
        List<Course> courses = courseMapper.selectList(null);
        Map<Long, String> courseMap = courses.stream()
                .collect(Collectors.toMap(Course::getId, Course::getName));

        // 5. 查询该学生的成绩
        List<Score> studentScores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getStudentNumber, studentNumber)
                        .in(Score::getExamId, examIds)
        );

        // 6. 查询同一考试年级段所有人的成绩（用于排名）
        List<Score> allScores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>().in(Score::getExamId, examIds)
        );

        // 7. 构建考试成绩按考试ID和课程ID聚合
        Map<Long, Map<Long, List<Score>>> examCourseScores = allScores.stream()
                .collect(Collectors.groupingBy(Score::getExamId,
                        Collectors.groupingBy(Score::getCourseId)));

        // 8. 构建趋势图数据
        Map<String, List<ScoreTrendPoint>> subjectTrendMap = new HashMap<>();
        for (Score score : studentScores) {
            Long examId = score.getExamId();
            Long courseId = score.getCourseId();
            Double studentScore = score.getScore() == null ? 0.0 : score.getScore();
            String subjectName = courseMap.getOrDefault(courseId, "未知科目");

            List<Score> sameExamCourseScores = examCourseScores
                    .getOrDefault(examId, Collections.emptyMap())
                    .getOrDefault(courseId, Collections.emptyList());

            List<Double> sortedScores = sameExamCourseScores.stream()
                    .map(s -> s.getScore() == null ? 0.0 : s.getScore())
                    .sorted(Comparator.reverseOrder())
                    .toList();

            int rank = 1;
            for (Double s : sortedScores) {
                if (s > studentScore) {
                    rank++;
                } else {
                    break;
                }
            }

            ScoreTrendPoint point = new ScoreTrendPoint();
            point.setExamId(examId);
            point.setScore(studentScore);
            point.setRank(rank);

            subjectTrendMap.computeIfAbsent(subjectName, k -> new ArrayList<>()).add(point);
        }

        // 9. 构建结果返回
        StudentScoreTrendResult result = new StudentScoreTrendResult();
        result.setStudentName(student.getName());
        result.setStudentNumber(studentNumber);
        result.setTrend(subjectTrendMap);

        return ResponseResult.success("查询成功", result);
    }

    @Override
    public ResponseResult<Map<Long, Double>> getStudentExam312Scores(String studentNumber, Integer grade) {
        // 1. 查询学生
        Student student = studentMapper.selectOne(
                new LambdaQueryWrapper<Student>().eq(Student::getStudentNumber, studentNumber)
        );
        if (student == null) {
            return ResponseResult.fail("未找到该学生");
        }

        // 2. 查找该学生在对应年级的学年记录
        StudentClassHistory history = studentClassHistoryMapper.selectOne(
                new LambdaQueryWrapper<StudentClassHistory>()
                        .eq(StudentClassHistory::getStudentNumber, studentNumber)
                        .eq(StudentClassHistory::getGrade, grade)
        );
        if (history == null) {
            return ResponseResult.fail("未找到该学生在该年级的历史记录");
        }

        Integer year = history.getYear();

        // 3. 获取该年级该学年的考试
        List<Exam> exams = examMapper.selectList(
                new LambdaQueryWrapper<Exam>()
                        .eq(Exam::getGrade, grade)
                        .eq(Exam::getYear, year)
        );
        if (exams.isEmpty()) {
            return ResponseResult.fail("该年级学年无考试数据");
        }

        List<Long> examIds = exams.stream().map(Exam::getId).toList();

        // 4. 获取课程名映射
        List<Course> courses = courseMapper.selectList(null);
        Map<Long, String> courseMap = courses.stream()
                .collect(Collectors.toMap(Course::getId, Course::getName));

        // 5. 查询该学生的所有成绩
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getStudentNumber, studentNumber)
                        .in(Score::getExamId, examIds)
        );

        // 6. 构建科目范围
        Set<String> validSubjects = Set.of("语文", "数学", "英语", "物理", "化学", "生物", "政治", "历史", "地理");

        // 7. 分组统计每场考试的总分
        Map<Long, Double> examTotalScores = new HashMap<>();
        for (Score score : scores) {
            Long examId = score.getExamId();
            Long courseId = score.getCourseId();
            Double value = score.getScore() == null ? 0.0 : score.getScore();
            String courseName = courseMap.getOrDefault(courseId, "");

            if (!validSubjects.contains(courseName)) {
                continue; // 不是3+1+2科目，不计入
            }

            examTotalScores.merge(examId, value, Double::sum);
        }

        return ResponseResult.success("统计成功", examTotalScores);
    }


}
