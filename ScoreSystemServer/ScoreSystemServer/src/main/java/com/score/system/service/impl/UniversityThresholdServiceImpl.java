package com.score.system.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.rank.Ranking;
import com.score.system.entity.rank.StudentRanking;
import com.score.system.entity.school.ClassEntity;
import com.score.system.entity.school.Score;
import com.score.system.entity.university.GradeThresholdPredictionResult;
import com.score.system.entity.university.ThresholdPredictionResult;
import com.score.system.entity.university.ThresholdRankingResult;
import com.score.system.entity.university.University;
import com.score.system.entity.user.Student;
import com.score.system.mapper.ClassMapper;
import com.score.system.mapper.ScoreMapper;
import com.score.system.mapper.StudentMapper;
import com.score.system.mapper.UniversityMapper;
import com.score.system.service.UniversityThresholdService;
import com.score.system.util.RedisUtil;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.TimeUnit;
import java.util.stream.Collectors;

@Service
public class UniversityThresholdServiceImpl implements UniversityThresholdService {
    private final RedisUtil redisUtil;
    private final UniversityMapper universityMapper;
    private final StudentMapper studentMapper;
    private final ScoreMapper scoreMapper;
    private final ClassMapper classMapper;

    public UniversityThresholdServiceImpl(RedisUtil redisUtil, UniversityMapper universityMapper, StudentMapper studentMapper, ScoreMapper scoreMapper, ClassMapper classMapper) {
        this.redisUtil = redisUtil;
        this.universityMapper = universityMapper;
        this.studentMapper = studentMapper;
        this.scoreMapper = scoreMapper;
        this.classMapper = classMapper;
    }

    @Override
    public ResponseResult<ThresholdRankingResult> getGradeThresholdRanking(Integer gradeId, Long examId) {
        List<ClassEntity> classEntities = classMapper.selectList(new LambdaQueryWrapper<ClassEntity>().eq(ClassEntity::getGrade, gradeId));
        List<Integer> classIds = classEntities.stream().map(c -> c.getId().intValue()).toList();

        List<Student> students = studentMapper.selectList(new LambdaQueryWrapper<Student>().in(Student::getClassId, classIds));
        if (students.isEmpty()) return ResponseResult.fail("无学生数据");

        Map<String, Student> studentMap = students.stream().collect(Collectors.toMap(Student::getStudentNumber, s -> s));
        List<String> studentNumbers = new ArrayList<>(studentMap.keySet());

        List<Score> scores = scoreMapper.selectList(new LambdaQueryWrapper<Score>().eq(Score::getExamId, examId).in(Score::getStudentNumber, studentNumbers));

        Map<String, Double> totalScoreMap = new HashMap<>();
        for (Score score : scores) {
            totalScoreMap.merge(score.getStudentNumber(), score.getScore() == null ? 0 : score.getScore(), Double::sum);
        }

        List<Map.Entry<String, Double>> sorted = new ArrayList<>(totalScoreMap.entrySet());
        sorted.sort((a, b) -> Double.compare(b.getValue(), a.getValue()));

        int total = sorted.size();
        Map<String, StudentRanking> rankingMap = new HashMap<>();
        int rank = 1;
        for (Map.Entry<String, Double> entry : sorted) {
            StudentRanking sr = new StudentRanking();
            sr.setStudentNumber(entry.getKey());
            sr.setStudentName(studentMap.get(entry.getKey()).getName());
            sr.setRanks(List.of(new Ranking(0L, entry.getValue(), rank++, total, "年级")));
            rankingMap.put(entry.getKey(), sr);
        }

        List<University> universities = universityMapper.selectList(null);
        Map<String, Integer> levelCounts = new HashMap<>();
        Map<String, Double> levelRates = new HashMap<>();
        Map<String, List<StudentRanking>> levelStudentList = new HashMap<>();

        for (University u : universities) {
            String levelName = getLevelName(u.getUniversityLevel());
            double threshold = isScienceGroup(u) ? u.getScienceScoreLine() : u.getArtScoreLine();
            List<StudentRanking> qualified = rankingMap.values().stream()
                    .filter(r -> r.getRanks().get(0).getScore() >= threshold)
                    .collect(Collectors.toList());

            levelCounts.put(levelName, qualified.size());
            levelRates.put(levelName, (double) qualified.size() / total);
            levelStudentList.put(levelName, qualified);
        }

        ThresholdRankingResult result = new ThresholdRankingResult();
        result.setLevelCounts(levelCounts);
        result.setLevelRates(levelRates);
        result.setLevelStudentList(levelStudentList);

        return ResponseResult.success("获取成功", result);
    }

    @Override
    public ResponseResult<List<ThresholdPredictionResult>> predictStudentThresholdProbability(String studentNumber) {
        List<Score> scores = scoreMapper.selectList(new LambdaQueryWrapper<Score>().eq(Score::getStudentNumber, studentNumber));
        Map<Long, List<Score>> examGrouped = scores.stream().collect(Collectors.groupingBy(Score::getExamId));
        List<University> universities = universityMapper.selectList(null);

        List<ThresholdPredictionResult> resultList = new ArrayList<>();
        for (University u : universities) {
            int total = 0, pass = 0;
            double threshold = isScienceGroup(u) ? u.getScienceScoreLine() : u.getArtScoreLine();
            for (List<Score> examScores : examGrouped.values()) {
                total++;
                double sum = examScores.stream().mapToDouble(s -> s.getScore() == null ? 0 : s.getScore()).sum();
                if (sum >= threshold) pass++;
            }
            ThresholdPredictionResult res = new ThresholdPredictionResult();
            res.setLevel(getLevelName(u.getUniversityLevel()));
            res.setTotalExams(total);
            res.setQualifiedExams(pass);
            res.setProbability(total == 0 ? 0.0 : (double) pass / total);
            resultList.add(res);
        }
        return ResponseResult.success("获取成功", resultList);
    }

    @Override
    public ResponseResult<List<StudentRanking>> getClassLevelStudentList(Integer classId, Long universityLevel, Long examId) {
        List<Student> students = studentMapper.selectList(new LambdaQueryWrapper<Student>().eq(Student::getClassId, classId));
        List<String> studentNumbers = students.stream().map(Student::getStudentNumber).toList();
        Map<String, Student> studentMap = students.stream().collect(Collectors.toMap(Student::getStudentNumber, s -> s));

        List<Score> scores = scoreMapper.selectList(new LambdaQueryWrapper<Score>().eq(Score::getExamId, examId).in(Score::getStudentNumber, studentNumbers));
        Map<String, Double> totalScoreMap = new HashMap<>();
        for (Score score : scores) {
            totalScoreMap.merge(score.getStudentNumber(), score.getScore() == null ? 0 : score.getScore(), Double::sum);
        }

        List<University> universities = universityMapper.selectList(new LambdaQueryWrapper<University>().eq(University::getUniversityLevel, universityLevel));
        if (universities.isEmpty()) return ResponseResult.fail("无对应大学层次设置");
        University u = universities.get(0);
        double threshold = isScienceGroup(u) ? u.getScienceScoreLine() : u.getArtScoreLine();

        List<Map.Entry<String, Double>> sorted = new ArrayList<>(totalScoreMap.entrySet());
        sorted.sort((a, b) -> Double.compare(b.getValue(), a.getValue()));

        int total = sorted.size();
        List<StudentRanking> result = new ArrayList<>();
        int rank = 1;
        for (Map.Entry<String, Double> entry : sorted) {
            if (entry.getValue() < threshold) continue;
            StudentRanking sr = new StudentRanking();
            sr.setStudentNumber(entry.getKey());
            sr.setStudentName(studentMap.get(entry.getKey()).getName());
            sr.setRanks(List.of(new Ranking(0L, entry.getValue(), rank++, total, "年级")));
            result.add(sr);
        }
        return ResponseResult.success("获取成功", result);
    }

    @Override
    public ResponseResult<List<GradeThresholdPredictionResult>> predictGradeThresholdProbability(Integer gradeId) {
        String redisKey = "grade:threshold:probability:" + gradeId;

        // 从 Redis 缓存读取
        if (redisUtil.exists(redisKey)) {
            List<GradeThresholdPredictionResult> cachedResult = redisUtil.get(redisKey, List.class);
            return ResponseResult.success("获取成功（来自缓存）", cachedResult);
        }

        // 没有缓存则继续查询逻辑
        List<ClassEntity> classEntities = classMapper.selectList(new LambdaQueryWrapper<ClassEntity>().eq(ClassEntity::getGrade, gradeId));
        List<Integer> classIds = classEntities.stream().map(c -> c.getId().intValue()).toList();

        List<Student> students = studentMapper.selectList(new LambdaQueryWrapper<Student>().in(Student::getClassId, classIds));
        if (students.isEmpty()) return ResponseResult.fail("无学生数据");

        Map<String, Student> studentMap = students.stream().collect(Collectors.toMap(Student::getStudentNumber, s -> s));
        List<String> studentNumbers = new ArrayList<>(studentMap.keySet());

        List<Score> scores = scoreMapper.selectList(new LambdaQueryWrapper<Score>().in(Score::getStudentNumber, studentNumbers));
        Map<String, List<Score>> groupedByStudent = scores.stream().collect(Collectors.groupingBy(Score::getStudentNumber));

        List<University> universities = universityMapper.selectList(null);
        List<GradeThresholdPredictionResult> resultList = new ArrayList<>();

        for (String studentNumber : studentNumbers) {
            List<Score> studentScores = groupedByStudent.getOrDefault(studentNumber, List.of());
            Map<Long, List<Score>> examGrouped = studentScores.stream().collect(Collectors.groupingBy(Score::getExamId));

            List<ThresholdPredictionResult> predictionList = new ArrayList<>();
            for (University u : universities) {
                double threshold = isScienceGroup(u) ? u.getScienceScoreLine() : u.getArtScoreLine();
                int total = 0, pass = 0;
                for (List<Score> examScores : examGrouped.values()) {
                    total++;
                    double sum = examScores.stream().mapToDouble(s -> s.getScore() == null ? 0 : s.getScore()).sum();
                    if (sum >= threshold) pass++;
                }
                ThresholdPredictionResult tpr = new ThresholdPredictionResult();
                tpr.setLevel(getLevelName(u.getUniversityLevel()));
                tpr.setTotalExams(total);
                tpr.setQualifiedExams(pass);
                tpr.setProbability(total == 0 ? 0.0 : (double) pass / total);
                predictionList.add(tpr);
            }

            GradeThresholdPredictionResult gtr = new GradeThresholdPredictionResult();
            gtr.setStudentNumber(studentNumber);
            gtr.setStudentName(studentMap.get(studentNumber).getName());
            gtr.setPredictionResults(predictionList);

            resultList.add(gtr);
        }

        // 设置缓存：缓存 30 分钟
        redisUtil.set(redisKey, resultList, TimeUnit.MINUTES, 30);

        return ResponseResult.success("获取成功", resultList);
    }


    private boolean isScienceGroup(University u) {
        // 这里可以改成按学生 subjectGroupId 判断，简化为根据得分线类型判断
        return u.getScienceScoreLine() != null && u.getScienceScoreLine() > 0;
    }

    private String getLevelName(Long level) {
        return switch (level.intValue()) {
            case 1 -> "985";
            case 2 -> "双一流";
            case 3 -> "优投";
            case 4 -> "本科";
            default -> "其他";
        };
    }
}
