package com.score.system.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.baomidou.mybatisplus.core.conditions.query.QueryWrapper;
import com.baomidou.mybatisplus.extension.service.impl.ServiceImpl;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.rank.ClassRankingDTO;
import com.score.system.entity.rank.GradeRankingDTO;
import com.score.system.entity.rank.Ranking;
import com.score.system.entity.rank.StudentRanking;
import com.score.system.entity.school.*;
import com.score.system.entity.user.Student;
import com.score.system.entity.user.StudentSubjectSelection;
import com.score.system.mapper.*;
import com.score.system.service.ScoreService;
import com.score.system.util.RedisUtil;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.*;
import java.util.concurrent.TimeUnit;
import java.util.stream.Collectors;

@Service
public class ScoreServiceImpl extends ServiceImpl<ScoreMapper,Score> implements ScoreService {
    private final ScoreMapper scoreMapper;
    private final StudentMapper studentMapper;
    private final CourseMapper courseMapper;
    private final ExamMapper examMapper;
    private final StudentSubjectSelectionMapper studentSubjectSelectionMapper;
    private final RedisUtil redisUtil;
    private final ClassMapper classMapper;

    public ScoreServiceImpl(ScoreMapper scoreMapper, StudentMapper studentMapper, CourseMapper courseMapper, ExamMapper examMapper, StudentSubjectSelectionMapper studentSubjectSelectionMapper, RedisUtil redisUtil, ClassMapper classMapper) {
        this.scoreMapper = scoreMapper;
        this.studentMapper = studentMapper;
        this.courseMapper = courseMapper;
        this.examMapper = examMapper;
        this.studentSubjectSelectionMapper = studentSubjectSelectionMapper;
        this.redisUtil = redisUtil;
        this.classMapper = classMapper;
    }

    @Override
    @Transactional
    public ResponseResult<Boolean> addScore(Score score) {
        if(studentMapper.selectStudentByNumber(score.getStudentNumber()) == null){
            return ResponseResult.fail("学号： " + score.getStudentNumber() + " 不存在");
        }
        if(courseMapper.selectById(score.getCourseId()) == null){
            return ResponseResult.fail("科目： " + score.getCourseId() + " 不存在");
        }
        if(examMapper.selectById(score.getExamId()) == null){
            return ResponseResult.fail("考试不存在: " + score.getExamId());
        }
        Long courseId = score.getCourseId();
//        LambdaQueryWrapper<StudentSubjectSelection> query = new LambdaQueryWrapper<>();
//        query.eq(StudentSubjectSelection::getStudentNumber, score.getStudentNumber());
//        StudentSubjectSelection selection = studentSubjectSelectionMapper.selectOne(query);
//        if(selection == null){
//            return ResponseResult.fail("学生还未设置分科: " + score.getStudentNumber());
//        }
//        // 查看学生是否选择了课程
//        List<Long> mustCourseId = new ArrayList<>();
//        mustCourseId.add(1L);
//        mustCourseId.add(2L);
//        mustCourseId.add(3L);
//        mustCourseId.add(selection.getElectiveCourse1Id());
//        mustCourseId.add(selection.getElectiveCourse2Id());
//        if(selection.getSubjectGroupId() == 2){
//            mustCourseId.add(5L);
//        } else if (selection.getSubjectGroupId() == 3) {
//            mustCourseId.add(4L);
//        }
//        boolean isSelected = false;
//        for(Long l : mustCourseId){
//            if (courseId.equals(l)) {
//                isSelected = true;
//                break;
//            }
//        }
//        if(!isSelected){
//            return ResponseResult.fail("学生:" + score.getStudentNumber() + "未选择该课程，无法录入成绩");
//        }
        if(score.getScore() < 0){
            return ResponseResult.fail("学生:" + score.getStudentNumber() + "|courseId:" + score.getCourseId()
                    + "|score:" + score.getScore() + " 请输入正确的成绩范围");
        }
        if(courseId >= 1 && courseId <= 3){
            if(score.getScore() > 150){
                return ResponseResult.fail("请输入正确的成绩范围");
            }
        }else{
            if(score.getScore() > 100){
                return ResponseResult.fail("请输入正确的成绩范围");
            }
        }
        int result = scoreMapper.insert(score);
        if(result <= 0){
            return ResponseResult.error("成绩添加失败");
        }
        return ResponseResult.success("成绩添加成功", true);
    }

    @Override
    @Transactional(rollbackFor = Exception.class)
    public ResponseResult<Boolean> batchAddScores(List<Score> scores) {
        if (scores == null || scores.isEmpty()) {
            return ResponseResult.fail("成绩列表不能为空");
        }

        Set<String> studentNumbers = scores.stream().map(Score::getStudentNumber).collect(Collectors.toSet());
        Set<Long> courseIds = scores.stream().map(Score::getCourseId).collect(Collectors.toSet());
        Set<Long> examIds = scores.stream().map(Score::getExamId).collect(Collectors.toSet());

        // 批量加载数据
        Map<String, Student> studentMap = studentMapper.selectBatchByStudentNumbers(studentNumbers)
                .stream().collect(Collectors.toMap(Student::getStudentNumber, s -> s));

        Map<Long, Course> courseMap = courseMapper.selectBatchIds(courseIds)
                .stream().collect(Collectors.toMap(Course::getId, c -> c));

        Map<Long, Exam> examMap = examMapper.selectBatchIds(examIds)
                .stream().collect(Collectors.toMap(Exam::getId, e -> e));

        // 可选：分科选课数据
        Map<String, StudentSubjectSelection> selectionMap = studentSubjectSelectionMapper.selectBatchByStudentNumbers(studentNumbers)
                .stream().collect(Collectors.toMap(StudentSubjectSelection::getStudentNumber, s -> s));

        for (Score score : scores) {
            String stuNo = score.getStudentNumber();
            Long courseId = score.getCourseId();
            Long examId = score.getExamId();
            double val = score.getScore();

            // 基础校验
            if (!studentMap.containsKey(stuNo)) {
                throw new IllegalArgumentException("学号不存在: " + stuNo);
            }
            if (!courseMap.containsKey(courseId)) {
                throw new IllegalArgumentException("课程不存在: " + courseId + "，学号：" + stuNo);
            }
            if (!examMap.containsKey(examId)) {
                throw new IllegalArgumentException("考试不存在: " + examId + "，学号：" + stuNo);
            }

            if (val < 0 || (courseId >= 1 && courseId <= 3 && val > 150) || (courseId > 3 && val > 100)) {
                throw new IllegalArgumentException("成绩范围非法: " + val + "（学号：" + stuNo + "，课程ID：" + courseId + "）");
            }

            // 可选：分科选课校验
//            StudentSubjectSelection selection = selectionMap.get(stuNo);
//            if (selection == null) {
//                throw new IllegalArgumentException("学生未设置选科: " + stuNo);
//            }
//
//            List<Long> mustCourses = new ArrayList<>(Arrays.asList(1L, 2L, 3L));
//            if (selection.getElectiveCourse1Id() != null) mustCourses.add(selection.getElectiveCourse1Id());
//            if (selection.getElectiveCourse2Id() != null) mustCourses.add(selection.getElectiveCourse2Id());
//            if (selection.getSubjectGroupId() != null) {
//                if (selection.getSubjectGroupId() == 2L) {
//                    mustCourses.add(5L);
//                } else if (selection.getSubjectGroupId() == 3L) {
//                    mustCourses.add(4L);
//                }
//            }
//
//            if (!mustCourses.contains(courseId)) {
//                throw new IllegalArgumentException("学生未选择该课程（学号：" + stuNo + "，课程ID：" + courseId + "），禁止录入成绩");
//            }
        }

        // 所有校验通过后插入
        boolean result = this.saveBatch(scores);  // 使用 MyBatis-Plus 提供的批量插入
        if (!result) {
            throw new RuntimeException("批量成绩插入失败");
        }

        return ResponseResult.success("成绩批量添加成功", true);
    }

    @Override
    public ResponseResult<List<ClassRankingDTO>> calculateClassRankings(String studentNumber, Long examId) {
        Student student = studentMapper.selectStudentByNumber(studentNumber);
        if(student == null){
            return ResponseResult.fail("学生不存在：" + studentNumber);
        }
        Exam exam = examMapper.selectById(examId);
        if(exam == null){
            return ResponseResult.fail("考试不存在: " + examId);
        }
        //获取学生的成绩
        List<Score> studentScore = scoreMapper.selectByExamAndStudentNumber(examId, studentNumber);
        if(studentScore.isEmpty()){
            return ResponseResult.fail("该学生" + studentNumber + "在该次考试中没有成绩记录");
        }
        List<ClassRankingDTO> resultList = new ArrayList<>();
        for(Score score : studentScore){
            Long courseId = score.getCourseId();
            Integer classId = student.getClassId();
            String redisKey = "classRank:" + examId + ":" + courseId + ":" + classId;
            //如果缓存不存在, 构建排名并缓存
            if(!redisUtil.exists(redisKey)){
                List<Score> classScores = scoreMapper.selectByExamCourseAndClassId(examId, courseId, classId);
                for(Score s : classScores){
                    redisUtil.getRedisTemplate().opsForZSet()
                            .add(redisKey, s.getStudentNumber(), s.getScore());
                }
                redisUtil.getRedisTemplate().expire(redisKey, 30 , TimeUnit.MINUTES);
            }
            //获取当前学生的排名(倒序，即高分在前)
            Long rank = redisUtil.getRedisTemplate().opsForZSet().reverseRank(redisKey, studentNumber);
            //获取总人数
            Long total = redisUtil.getRedisTemplate().opsForZSet().zCard(redisKey);
            //获取分数
            Double redisScore = redisUtil.getRedisTemplate().opsForZSet().score(redisKey,studentScore);
            ClassRankingDTO dto = new ClassRankingDTO();
            dto.setCourseId(courseId);
            dto.setScore(score.getScore());
            dto.setTotalInClass(total != null ? total : 0L);
            dto.setRank(rank != null ? rank.intValue() + 1 : null);
            resultList.add(dto);
        }
        return ResponseResult.success("班级排名获取成功", resultList);
    }

    @Override
    public ResponseResult<List<GradeRankingDTO>> calculateGradeRankings(String studentNumber, Long examId) {
        Student student = studentMapper.selectStudentByNumber(studentNumber);
        if (student == null) return ResponseResult.fail("学生不存在: " + studentNumber);

        ClassEntity clazz = classMapper.selectById(student.getClassId());
        if (clazz == null) return ResponseResult.fail("班级不存在: " + student.getClassId());

        List<Score> studentScores = scoreMapper.selectList(
                new QueryWrapper<Score>().eq("exam_id", examId).eq("student_number", studentNumber));

        List<GradeRankingDTO> result = new ArrayList<>();

        for (Score score : studentScores) {
            String redisKey = buildRedisKey(score.getExamId(), score.getCourseId(), clazz.getSubjectGroupId());

            // 如果缓存不存在则自动构建
            if (!redisUtil.exists(redisKey)) {
                buildGradeRankingToRedis(examId);
            }

            Long rank = redisUtil.zrevrank(redisKey, studentNumber);
            Long total = redisUtil.zcount(redisKey);

            if (rank != null && total != null) {
                int rankInt = rank.intValue();
                GradeRankingDTO dto = new GradeRankingDTO();
                dto.setStudentNumber(studentNumber);
                dto.setCourseId(score.getCourseId());
                dto.setScore(score.getScore());
                dto.setGradeRank(rankInt + 1);
                dto.setTotalStudents(total.intValue());
                result.add(dto);
            }
        }

        return ResponseResult.success(result);
    }

    @Override
    public ResponseResult<ClassRankingDTO> calculateTotalScoreClassRankings(String studentNumber, Long examId) {
        Student student = studentMapper.selectStudentByNumber(studentNumber);
        if (student == null) return ResponseResult.fail("学生不存在: " + studentNumber);
        int classId = student.getClassId();
        StudentSubjectSelection selection = studentSubjectSelectionMapper.selectOne(
                new LambdaQueryWrapper<StudentSubjectSelection>()
                        .eq(StudentSubjectSelection::getStudentNumber, studentNumber)
        );
        if(selection == null) return ResponseResult.fail("未找到该学生的选科信息 : " + studentNumber);
        //需要计算的课程ID
        List<Long> requiredCourseIds = new ArrayList<>();
        requiredCourseIds.add(1L);
        requiredCourseIds.add(2L);
        requiredCourseIds.add(3L);
        if(selection.getSubjectGroupId() == 2){
            requiredCourseIds.add(5L);
        }else if(selection.getSubjectGroupId() == 3){
            requiredCourseIds.add(4L);
        }
        requiredCourseIds.add(selection.getElectiveCourse1Id());
        requiredCourseIds.add(selection.getElectiveCourse2Id());
        //查出该班所有人的成绩
        List<Student> classStudents = studentMapper.selectList(
                new LambdaQueryWrapper<Student>()
                        .eq(Student::getClassId, classId)
        );
        List<String> studentNumbers = classStudents.stream().map(Student::getStudentNumber).toList();
        //查找所有分数
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
                        .in(Score::getStudentNumber, studentNumbers)
                        .in(Score::getCourseId, requiredCourseIds)
        );
        //分组求总分
        Map<String, Double> totalScoreMap = new HashMap<>();
        for(Score score : scores){
            if(score.getScore() == null) continue;
            totalScoreMap.merge(score.getStudentNumber(), score.getScore(), Double::sum);
        }
        String redisKey = "classTotalScore:" + examId + ":" + classId;
        if(!redisUtil.exists(redisKey)){
            redisUtil.delete(redisKey);
            for(Map.Entry<String, Double> entry : totalScoreMap.entrySet()){
                redisUtil.zadd(redisKey, entry.getValue(), entry.getKey());
            }
            redisUtil.expire(redisKey, 30, TimeUnit.MINUTES);
        }
        Long rank = redisUtil.zrevrank(redisKey, studentNumber);
        Long total = redisUtil.zcount(redisKey);
        Double score = redisUtil.zscore(redisKey, studentNumber);
        ClassRankingDTO dto = new ClassRankingDTO();
        dto.setCourseId(0L);
        dto.setScore(score);
        dto.setTotalInClass(total != null ? total : 0L);
        dto.setRank(rank != null ? rank.intValue() + 1 : null);
        return ResponseResult.success("总分班级排名获取成功", dto);
    }

    @Override
    public ResponseResult<GradeRankingDTO> calculateTotalScoreGradeRankings(String studentNumber, Long examId) {
        Student student = studentMapper.selectStudentByNumber(studentNumber);
        if (student == null) return ResponseResult.fail("学生不存在: " + studentNumber);
        ClassEntity classEntity = classMapper.selectById(student.getClassId());
        if(classEntity == null) return ResponseResult.fail("班级不存在: " + student.getClassId());
        Long subjectGroupId = classEntity.getSubjectGroupId();
        String redisKey = "gradeTotalScore:" + examId + ":" + subjectGroupId;
        //检查redis是否存在缓存
        if(!redisUtil.exists(redisKey)){
            List<Student> allStudent = studentMapper.selectList(null);
            Map<String, Long> studentGroupMap = allStudent.stream()
                    .collect(Collectors.toMap(Student::getStudentNumber, s -> {
                        ClassEntity c = classMapper.selectById(s.getClassId());
                        return c != null ? c.getSubjectGroupId() : null;
                    }));
            List<Score> scores = scoreMapper.selectList(
                    new LambdaQueryWrapper<Score>()
                            .eq(Score::getExamId, examId)
            );
            //计算总分并按 subjectGroupId 分类
            Map<String,Double> totalScoreMap = new HashMap<>();
            for(Score score : scores){
                String sNum = score.getStudentNumber();
                Long groupId = studentGroupMap.get(sNum);
                if(groupId != null && groupId.equals(subjectGroupId) && score.getScore() != null){
                    totalScoreMap.merge(sNum, score.getScore(), Double::sum);
                }
            }
            //缓存进redis
            redisUtil.delete(redisKey);
            for (Map.Entry<String, Double> entry : totalScoreMap.entrySet()){
                redisUtil.zadd(redisKey, entry.getValue(), entry.getKey());
            }
            redisUtil.expire(redisKey, 30, TimeUnit.MINUTES);
        }
        //获取当前学生信息
        Long rank = redisUtil.zrevrank(redisKey, studentNumber);
        Long total = redisUtil.zcount(redisKey);
        Double totalScore = redisUtil.zscore(redisKey, studentNumber);
        GradeRankingDTO dto = new GradeRankingDTO();
        dto.setStudentNumber(studentNumber);
        dto.setCourseId(0L);
        dto.setScore(totalScore != null ? totalScore : 0.0);
        dto.setGradeRank(rank != null ? rank.intValue() + 1 : 0);
        dto.setTotalStudents(total != null ? total.intValue() : 0);

        return ResponseResult.success("总分年级排名获取成功", dto);
    }

    @Override
    public ResponseResult<StudentRanking> getStudentAllRankings(String studentNumber, Long examId) {
        Student student = studentMapper.selectStudentByNumber(studentNumber);
        if (student == null) return ResponseResult.fail("学生不存在: " + studentNumber);
        ClassEntity clazz = classMapper.selectById(student.getClassId());
        if (clazz == null) return ResponseResult.fail("班级不存在: " + student.getClassId());

        StudentRanking studentRanking = new StudentRanking();
        studentRanking.setStudentNumber(studentNumber);
        studentRanking.setStudentName(student.getStudentNumber());

        List<Ranking> rankList = new ArrayList<>();
        List<Score> studentScores = scoreMapper.selectByExamAndStudentNumber(examId, studentNumber);
        if (studentScores.isEmpty()) return ResponseResult.fail("学生没有成绩记录");

        // 班级单科排名
        for (Score score : studentScores) {
            String redisKey = "classRank:" + examId + ":" + score.getCourseId() + ":" + student.getClassId();
            if (!redisUtil.exists(redisKey)) {
                List<Score> classScores = scoreMapper.selectByExamCourseAndClassId(examId, score.getCourseId(), student.getClassId());
                for (Score s : classScores) {
                    redisUtil.zadd(redisKey, s.getScore(), s.getStudentNumber());
                }
                redisUtil.expire(redisKey, 30, TimeUnit.MINUTES);
            }
            Long rank = redisUtil.zrevrank(redisKey, studentNumber);
            Long total = redisUtil.zcount(redisKey);
            Course course = courseMapper.selectById(score.getCourseId());
            rankList.add(new Ranking() {{
                setCourseId(score.getCourseId());
                setScore(score.getScore());
                setRank(rank != null ? rank.intValue() + 1 : null);
                setTotal(total != null ? total.intValue() : 0);
                setScope("班级");
            }});
        }

        // 年级单科排名
        buildGradeRankingToRedis(examId); // 确保Redis中有年级排名数据
        for (Score score : studentScores) {
            String redisKey = buildRedisKey(examId, score.getCourseId(), clazz.getSubjectGroupId());
            Long rank = redisUtil.zrevrank(redisKey, studentNumber);
            Long total = redisUtil.zcount(redisKey);
            Course course = courseMapper.selectById(score.getCourseId());
            rankList.add(new Ranking() {{
                setCourseId(score.getCourseId());
                setScore(score.getScore());
                setRank(rank != null ? rank.intValue() + 1 : null);
                setTotal(total != null ? total.intValue() : 0);
                setScope("年级");
            }});
        }

        // 总分排名（班级 + 年级）
        Ranking classTotal = buildTotalRanking(student, examId, "班级");
        if (classTotal != null) rankList.add(classTotal);
        Ranking gradeTotal = buildTotalRanking(student, examId, "年级");
        if (gradeTotal != null) rankList.add(gradeTotal);

        studentRanking.setRanks(rankList);
        return ResponseResult.success("学生排名获取成功", studentRanking);
    }

    @Override
    public ResponseResult<List<StudentRanking>> getClassTotalRankings(Integer classId, Long examId) {
        // 1. 查询该班级所有学生
        List<Student> students = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().eq(Student::getClassId, classId)
        );
        if (students.isEmpty()) {
            return ResponseResult.fail("该班级没有学生");
        }

        List<String> studentNumbers = students.stream()
                .map(Student::getStudentNumber)
                .toList();

        Map<String, String> studentNameMap = students.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, Student::getName));

        // 2. 初始化总分 Map
        Map<String, Double> studentTotalScoreMap = new HashMap<>();
        for (String studentNumber : studentNumbers) {
            studentTotalScoreMap.put(studentNumber, 0.0); // 默认为 0 分
        }

        // 3. 查询成绩并累加总分
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
                        .in(Score::getStudentNumber, studentNumbers)
        );

        for (Score score : scores) {
            if (score.getScore() != null) {
                studentTotalScoreMap.merge(score.getStudentNumber(), score.getScore(), Double::sum);
            }
        }

        // 4. 排序
        List<Map.Entry<String, Double>> sorted = new ArrayList<>(studentTotalScoreMap.entrySet());
        sorted.sort((a, b) -> Double.compare(b.getValue(), a.getValue())); // 高分在前

        // 5. 构建返回结构
        List<StudentRanking> result = new ArrayList<>();
        int total = sorted.size();
        int currentRank = 1;

        for (Map.Entry<String, Double> entry : sorted) {
            String studentNumber = entry.getKey();
            Double totalScore = entry.getValue();

            Ranking ranking = new Ranking();
            ranking.setCourseId(0L);
            ranking.setScore(totalScore);
            ranking.setRank(currentRank++);
            ranking.setTotal(total);
            ranking.setScope("班级");

            StudentRanking studentRanking = new StudentRanking();
            studentRanking.setStudentNumber(studentNumber);
            studentRanking.setStudentName(studentNameMap.get(studentNumber));
            studentRanking.setRanks(List.of(ranking));

            result.add(studentRanking);
        }

        return ResponseResult.success("获取班级学生总分排名成功", result);
    }


    @Override
    public ResponseResult<List<StudentRanking>> getGradeTotalRankings(Integer gradeId, Long examId) {
        // 1. 查询该年级下所有班级
        List<ClassEntity> classEntities = classMapper.selectList(
                new LambdaQueryWrapper<ClassEntity>()
                        .eq(ClassEntity::getGrade, gradeId)
        );
        if (classEntities.isEmpty()) {
            return ResponseResult.fail("该年级没有班级");
        }

        List<Long> classIds = classEntities.stream()
                .map(ClassEntity::getId)
                .toList();

        // 2. 查询学生
        List<Student> students = studentMapper.selectList(
                new LambdaQueryWrapper<Student>()
                        .in(Student::getClassId, classIds)
        );
        if (students.isEmpty()) {
            return ResponseResult.fail("该年级没有学生");
        }

        List<String> studentNumbers = students.stream()
                .map(Student::getStudentNumber)
                .toList();

        Map<String, String> studentNameMap = students.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, Student::getName));

        // 3. 初始化学生分数为 0
        Map<String, Double> studentTotalScoreMap = new HashMap<>();
        for (String studentNumber : studentNumbers) {
            studentTotalScoreMap.put(studentNumber, 0.0);
        }

        // 4. 查询成绩并累计
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
                        .in(Score::getStudentNumber, studentNumbers)
        );

        for (Score score : scores) {
            if (score.getScore() != null) {
                studentTotalScoreMap.merge(score.getStudentNumber(), score.getScore(), Double::sum);
            }
        }

        // 5. 排名
        List<Map.Entry<String, Double>> sorted = new ArrayList<>(studentTotalScoreMap.entrySet());
        sorted.sort((a, b) -> Double.compare(b.getValue(), a.getValue())); // 高分在前

        // 6. 构建结果
        List<StudentRanking> result = new ArrayList<>();
        int total = sorted.size();
        int rank = 1;

        for (Map.Entry<String, Double> entry : sorted) {
            String studentNumber = entry.getKey();
            Double totalScore = entry.getValue();

            Ranking ranking = new Ranking();
            ranking.setCourseId(0L);
            ranking.setScore(totalScore);
            ranking.setRank(rank++);
            ranking.setTotal(total);
            ranking.setScope("年级");

            StudentRanking studentRanking = new StudentRanking();
            studentRanking.setStudentNumber(studentNumber);
            studentRanking.setStudentName(studentNameMap.get(studentNumber));
            studentRanking.setRanks(List.of(ranking));

            result.add(studentRanking);
        }

        return ResponseResult.success("获取年级总分排名成功", result);
    }

    @Override
    public ResponseResult<List<StudentRanking>> getClassCourseRankings(Integer classId, Long examId) {
        String redisKey = "classCourseRankings:" + classId + ":" + examId;
        if (redisUtil.exists(redisKey)) {
            List<StudentRanking> cached = redisUtil.get(redisKey, List.class);
            return ResponseResult.success("获取班级各科成绩排名成功（缓存）", cached);
        }

        // 1. 获取班级学生信息
        List<Student> students = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().eq(Student::getClassId, classId));
        if (students.isEmpty()) {
            return ResponseResult.fail("该班级没有学生");
        }

        List<String> studentNumbers = students.stream()
                .map(Student::getStudentNumber)
                .collect(Collectors.toList());
        int totalStudents = students.size();
        Map<String, String> nameMap = students.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, Student::getName));

        // 2. 获取所有课程和成绩
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
                        .in(Score::getStudentNumber, studentNumbers));

        // 3. 构建数据结构
        Map<Long, Map<String, Double>> courseScores = new HashMap<>(); // 各科成绩
        Map<String, Double> totalScores = new HashMap<>();             // 总分
        Set<Long> allCourseIds = new HashSet<>();                      // 所有课程ID

        // 初始化数据结构
        studentNumbers.forEach(sn -> {
            totalScores.put(sn, 0.0);
        });

        // 处理成绩数据
        for (Score score : scores) {
            Long courseId = score.getCourseId();
            String sn = score.getStudentNumber();
            Double scoreValue = score.getScore() != null ? score.getScore() : 0.0;

            // 记录课程ID
            allCourseIds.add(courseId);

            // 更新课程成绩
            courseScores.computeIfAbsent(courseId, k -> new HashMap<>())
                    .put(sn, scoreValue);

            // 累计总分
            totalScores.put(sn, totalScores.get(sn) + scoreValue);
        }

        // 4. 计算各科排名
        Map<Long, Map<String, Integer>> courseRankings = new HashMap<>();
        for (Long courseId : allCourseIds) {
            // 获取包含所有学生的成绩（无成绩默认0分）
            Map<String, Double> fullScores = studentNumbers.stream()
                    .collect(Collectors.toMap(
                            sn -> sn,
                            sn -> courseScores.getOrDefault(courseId, new HashMap<>())
                                    .getOrDefault(sn, 0.0)
                    ));

            // 排序并生成排名
            List<Map.Entry<String, Double>> sorted = new ArrayList<>(fullScores.entrySet());
            sorted.sort((a, b) -> Double.compare(b.getValue(), a.getValue()));

            Map<String, Integer> rankMap = new LinkedHashMap<>();
            int currentRank = 1;
            for (int i = 0; i < sorted.size(); i++) {
                Map.Entry<String, Double> entry = sorted.get(i);
                // 处理并列
                if (i > 0 && entry.getValue().equals(sorted.get(i-1).getValue())) {
                    rankMap.put(entry.getKey(), rankMap.get(sorted.get(i-1).getKey()));
                } else {
                    rankMap.put(entry.getKey(), currentRank);
                }
                currentRank = i + 1;
            }
            courseRankings.put(courseId, rankMap);
        }

        // 5. 计算总分排名
        List<Map.Entry<String, Double>> sortedTotal = totalScores.entrySet().stream()
                .sorted((a, b) -> Double.compare(b.getValue(), a.getValue()))
                .toList();

        Map<String, Integer> totalRankMap = new LinkedHashMap<>();
        int currentTotalRank = 1;
        for (int i = 0; i < sortedTotal.size(); i++) {
            Map.Entry<String, Double> entry = sortedTotal.get(i);
            if (i > 0 && entry.getValue().equals(sortedTotal.get(i-1).getValue())) {
                totalRankMap.put(entry.getKey(), totalRankMap.get(sortedTotal.get(i-1).getKey()));
            } else {
                totalRankMap.put(entry.getKey(), currentTotalRank);
            }
            currentTotalRank = i + 1;
        }

        // 6. 构建结果（按总分排序）
        List<StudentRanking> result = new ArrayList<>();
        for (Map.Entry<String, Double> totalEntry : sortedTotal) {
            String sn = totalEntry.getKey();
            StudentRanking sr = new StudentRanking();
            sr.setStudentNumber(sn);
            sr.setStudentName(nameMap.get(sn));

            List<Ranking> rankings = new ArrayList<>();

            // 添加各科排名信息
            for (Long courseId : allCourseIds) {
                Double score = courseScores.getOrDefault(courseId, new HashMap<>())
                        .getOrDefault(sn, 0.0);
                Integer rank = courseRankings.get(courseId).get(sn);

                rankings.add(new Ranking(
                        courseId,
                        score,
                        rank,
                        totalStudents,
                        "班级"
                ));
            }

            // 添加总分信息
            rankings.add(new Ranking(
                    0L,
                    totalEntry.getValue(),
                    totalRankMap.get(sn),
                    totalStudents,
                    "班级"
            ));

            sr.setRanks(rankings);
            result.add(sr);
        }

        // 7. 缓存结果
        redisUtil.set(redisKey, result, TimeUnit.MINUTES, 10);
        return ResponseResult.success("获取班级各科成绩及排名成功", result);
    }


    @Override
    public ResponseResult<List<StudentRanking>> getGradeCourseRankings(Integer gradeId, Long examId) {
        // 构建唯一Redis缓存键
        String redisKey = "gradeCourseRankings:" + gradeId + ":" + examId;
        // 检查缓存是否存在
        if (redisUtil.exists(redisKey)) {
            List<StudentRanking> cached = redisUtil.get(redisKey, List.class);
            return ResponseResult.success("获取年级各科成绩排名成功（缓存）", cached);
        }

        // 1. 获取年级下所有班级
        List<ClassEntity> classEntities = classMapper.selectList(
                new LambdaQueryWrapper<ClassEntity>().eq(ClassEntity::getGrade, gradeId)
        );
        if (classEntities.isEmpty()) {
            return ResponseResult.fail("该年级没有班级");
        }

        List<Long> classIds = classEntities.stream().map(ClassEntity::getId).toList();

        // 2. 获取这些班级的所有学生
        List<Student> students = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().in(Student::getClassId, classIds)
        );
        if (students.isEmpty()) {
            return ResponseResult.fail("该年级没有学生");
        }

        List<String> studentNumbers = students.stream()
                .map(Student::getStudentNumber)
                .toList();

        Map<String, String> studentNameMap = students.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, Student::getName));

        // 3. 获取成绩
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
                        .in(Score::getStudentNumber, studentNumbers)
        );

        // 4. 构建 Map<courseId, Map<studentNumber, score>>
        Map<Long, Map<String, Double>> courseStudentScoreMap = new HashMap<>();
        for (Score score : scores) {
            if (score.getScore() != null) {
                courseStudentScoreMap
                        .computeIfAbsent(score.getCourseId(), k -> new HashMap<>())
                        .put(score.getStudentNumber(), score.getScore());
            }
        }

        // 5. 初始化学生对象
        Map<String, StudentRanking> studentRankingMap = new HashMap<>();
        for (String studentNumber : studentNumbers) {
            StudentRanking sr = new StudentRanking();
            sr.setStudentNumber(studentNumber);
            sr.setStudentName(studentNameMap.get(studentNumber));
            sr.setRanks(new ArrayList<>());
            studentRankingMap.put(studentNumber, sr);
        }

        // 6. 对每门课程进行排名
        for (Map.Entry<Long, Map<String, Double>> entry : courseStudentScoreMap.entrySet()) {
            Long courseId = entry.getKey();
            Map<String, Double> studentScores = entry.getValue();

            List<Map.Entry<String, Double>> sortedList = new ArrayList<>(studentScores.entrySet());
            sortedList.sort((a, b) -> Double.compare(b.getValue(), a.getValue()));

            int rank = 1;
            int total = studentNumbers.size();

            for (Map.Entry<String, Double> e : sortedList) {
                String studentNumber = e.getKey();
                Double score = e.getValue();

                Ranking ranking = new Ranking();
                ranking.setCourseId(courseId);
                ranking.setScore(score);
                ranking.setRank(rank++);
                ranking.setTotal(total);
                ranking.setScope("年级");

                studentRankingMap.get(studentNumber).getRanks().add(ranking);
            }

            // 处理无成绩的学生
            for (String studentNumber : studentNumbers) {
                if (!studentScores.containsKey(studentNumber)) {
                    Ranking ranking = new Ranking();
                    ranking.setCourseId(courseId);
                    ranking.setScore(0.0D);
                    ranking.setRank(0);
                    ranking.setTotal(total);
                    ranking.setScope("年级");

                    studentRankingMap.get(studentNumber).getRanks().add(ranking);
                }
            }
        }

        // 转换为列表
        List<StudentRanking> result = new ArrayList<>(studentRankingMap.values());

        // 将结果存入Redis，有效期10分钟
        redisUtil.set(redisKey, result, TimeUnit.MINUTES, 10);

        return ResponseResult.success("获取年级各科成绩排名成功", result);
    }

    @Override
    public ResponseResult<List<StudentRanking>> getClassRankingWithGroupCourse(Integer classId, Long examId) {
        String redisKey = "rank:classGroup:" + classId + ":exam:" + examId;
        if (redisUtil.exists(redisKey)) {
            List<StudentRanking> cached = redisUtil.get(redisKey, List.class);
            return ResponseResult.success("获取成功（缓存）", cached);
        }

        // 1. 获取班级信息
        ClassEntity classEntity = classMapper.selectById(classId);
        if (classEntity == null) return ResponseResult.fail("班级不存在");
        Long subjectGroupId = classEntity.getSubjectGroupId();

        // 2. 确定分科组合课程（3+1）
        List<Long> courseIds = new ArrayList<>(List.of(1L, 2L, 3L)); // 语数外
        Long extraCourseId = getExtraCourseId(subjectGroupId.intValue());
        courseIds.add(extraCourseId); // 根据分科组合添加第四科

        // 3. 获取所有学生
        List<Student> students = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().eq(Student::getClassId, classId)
        );
        if (students.isEmpty()) return ResponseResult.fail("该班级无学生");

        List<String> studentNumbers = students.stream()
                .map(Student::getStudentNumber)
                .collect(Collectors.toList());
        int totalStudents = students.size();
        Map<String, String> nameMap = students.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, Student::getName));

        // 4. 初始化各科成绩容器（包含所有学生）
        Map<Long, List<ScoreEntry>> courseScoreEntries = new HashMap<>();
        for (Long courseId : courseIds) {
            List<ScoreEntry> entries = studentNumbers.stream()
                    .map(sn -> new ScoreEntry(sn, 0.0))
                    .collect(Collectors.toList());
            courseScoreEntries.put(courseId, entries);
        }

        // 5. 查询并更新实际成绩
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
                        .in(Score::getStudentNumber, studentNumbers)
                        .in(Score::getCourseId, courseIds)
        );

        Map<String, Map<Long, Double>> actualScores = new HashMap<>();
        for (Score score : scores) {
            String sn = score.getStudentNumber();
            Long cid = score.getCourseId();
            Double val = score.getScore() != null ? score.getScore() : 0.0;
            actualScores.computeIfAbsent(sn, k -> new HashMap<>()).put(cid, val);
        }

        // 更新各科成绩
        for (Long courseId : courseIds) {
            List<ScoreEntry> entries = courseScoreEntries.get(courseId);
            for (ScoreEntry entry : entries) {
                Double actual = actualScores.getOrDefault(entry.studentNumber, new HashMap<>())
                        .getOrDefault(courseId, 0.0);
                entry.score = actual;
            }
        }

        // 6. 计算各科排名
        Map<Long, Map<String, Integer>> courseRankings = new HashMap<>();
        for (Long courseId : courseIds) {
            List<ScoreEntry> sorted = courseScoreEntries.get(courseId).stream()
                    .sorted((a, b) -> Double.compare(b.score, a.score))
                    .toList();

            Map<String, Integer> rankMap = new LinkedHashMap<>();
            int currentRank = 1;
            for (int i = 0; i < sorted.size(); i++) {
                ScoreEntry entry = sorted.get(i);
                if (i > 0 && entry.score.equals(sorted.get(i-1).score)) {
                    rankMap.put(entry.studentNumber, rankMap.get(sorted.get(i-1).studentNumber));
                } else {
                    rankMap.put(entry.studentNumber, currentRank);
                }
                currentRank = i + 2;
            }
            courseRankings.put(courseId, rankMap);
        }

        // 7. 计算总分
        Map<String, Double> totalScores = studentNumbers.stream()
                .collect(Collectors.toMap(
                        sn -> sn,
                        sn -> courseIds.stream()
                                .mapToDouble(cid -> actualScores.getOrDefault(sn, new HashMap<>())
                                        .getOrDefault(cid, 0.0))
                                .sum()
                ));

        // 8. 总分排名
        List<Map.Entry<String, Double>> sortedTotal = totalScores.entrySet().stream()
                .sorted((a, b) -> Double.compare(b.getValue(), a.getValue()))
                .toList();

        Map<String, Integer> totalRankMap = new LinkedHashMap<>();
        int currentTotalRank = 1;
        for (int i = 0; i < sortedTotal.size(); i++) {
            Map.Entry<String, Double> entry = sortedTotal.get(i);
            if (i > 0 && entry.getValue().equals(sortedTotal.get(i-1).getValue())) {
                totalRankMap.put(entry.getKey(), totalRankMap.get(sortedTotal.get(i-1).getKey()));
            } else {
                totalRankMap.put(entry.getKey(), currentTotalRank);
            }
            currentTotalRank = i + 2;
        }

        // 9. 构建结果
        List<StudentRanking> result = sortedTotal.stream()
                .map(entry -> {
                    String sn = entry.getKey();
                    List<Ranking> rankings = new ArrayList<>();

                    // 添加各科信息
                    for (Long courseId : courseIds) {
                        Double score = actualScores.getOrDefault(sn, new HashMap<>())
                                .getOrDefault(courseId, 0.0);
                        Integer rank = courseRankings.get(courseId).getOrDefault(sn, 0);

                        rankings.add(new Ranking() {{
                            setCourseId(courseId);
                            setScore(score);
                            setRank(rank > 0 ? rank : 0);
                            setTotal(totalStudents);
                            setScope("班级");
                        }});
                    }

                    // 添加总分信息
                    rankings.add(new Ranking() {{
                        setCourseId(0L);
                        setScore(entry.getValue());
                        setRank(totalRankMap.get(sn));
                        setTotal(totalStudents);
                        setScope("班级");
                    }});

                    return new StudentRanking() {{
                        setStudentNumber(sn);
                        setStudentName(nameMap.get(sn));
                        setRanks(rankings);
                    }};
                })
                .collect(Collectors.toList());

        // 缓存结果
        redisUtil.set(redisKey, result, TimeUnit.MINUTES, 10);
        return ResponseResult.success("获取分科组合排名成功", result);
    }



    @Override
    public ResponseResult<List<StudentRanking>> getGradeRankingWithGroupCourse(Integer gradeId, Long examId, Integer subjectGroupId) {
        String redisKey = "gradeGroupRank:" + gradeId + ":exam:" + examId + ":sgid:" + subjectGroupId;
        if (redisUtil.exists(redisKey)) {
            List<StudentRanking> cached = redisUtil.get(redisKey, List.class);
            return ResponseResult.success("获取成功（缓存）", cached);
        }

        // 1. 确定分科组合课程
        Long extraCourseId = getExtraCourseId(subjectGroupId);
        List<Long> courseIds = List.of(1L, 2L, 3L, extraCourseId);

        // 2. 获取相关班级和学生
        List<ClassEntity> classEntities = classMapper.selectList(
                new LambdaQueryWrapper<ClassEntity>()
                        .eq(ClassEntity::getGrade, gradeId)
                        .eq(ClassEntity::getSubjectGroupId, subjectGroupId)
        );
        if (classEntities.isEmpty()) return ResponseResult.fail("该科目组合无班级");

        List<Long> classIds = classEntities.stream()
                .map(ClassEntity::getId)
                .collect(Collectors.toList());

        List<Student> students = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().in(Student::getClassId, classIds)
        );
        if (students.isEmpty()) return ResponseResult.fail("无学生信息");

        List<String> studentNumbers = students.stream()
                .map(Student::getStudentNumber)
                .collect(Collectors.toList());
        int totalStudents = studentNumbers.size();
        Map<String, String> nameMap = students.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, Student::getName));

        // 3. 初始化各科成绩容器
        Map<Long, List<ScoreEntry>> courseScoreEntries = new HashMap<>();
        for (Long courseId : courseIds) {
            List<ScoreEntry> entries = studentNumbers.stream()
                    .map(sn -> new ScoreEntry(sn, 0.0))
                    .collect(Collectors.toList());
            courseScoreEntries.put(courseId, entries);
        }

        // 4. 查询并更新实际成绩
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
                        .in(Score::getStudentNumber, studentNumbers)
                        .in(Score::getCourseId, courseIds)
        );

        Map<String, Map<Long, Double>> actualScores = new HashMap<>();
        for (Score score : scores) {
            String sn = score.getStudentNumber();
            Long cid = score.getCourseId();
            Double val = score.getScore() != null ? score.getScore() : 0.0;
            actualScores.computeIfAbsent(sn, k -> new HashMap<>()).put(cid, val);
        }

        // 更新各科成绩
        for (Long courseId : courseIds) {
            List<ScoreEntry> entries = courseScoreEntries.get(courseId);
            for (ScoreEntry entry : entries) {
                Double actual = actualScores.getOrDefault(entry.studentNumber, new HashMap<>())
                        .getOrDefault(courseId, 0.0);
                entry.score = actual;
            }
        }

        // 5. 计算各科排名（年级范围）
        Map<Long, Map<String, Integer>> courseRankings = new HashMap<>();
        for (Long courseId : courseIds) {
            List<ScoreEntry> sorted = courseScoreEntries.get(courseId).stream()
                    .sorted((a, b) -> Double.compare(b.score, a.score))
                    .toList();

            Map<String, Integer> rankMap = new LinkedHashMap<>();
            int currentRank = 1;
            for (int i = 0; i < sorted.size(); i++) {
                ScoreEntry entry = sorted.get(i);
                if (i > 0 && entry.score.equals(sorted.get(i-1).score)) {
                    rankMap.put(entry.studentNumber, rankMap.get(sorted.get(i-1).studentNumber));
                } else {
                    rankMap.put(entry.studentNumber, currentRank);
                }
                currentRank = i + 2;
            }
            courseRankings.put(courseId, rankMap);
        }

        // 6. 计算总分
        Map<String, Double> totalScores = studentNumbers.stream()
                .collect(Collectors.toMap(
                        sn -> sn,
                        sn -> courseIds.stream()
                                .mapToDouble(cid -> actualScores.getOrDefault(sn, new HashMap<>())
                                        .getOrDefault(cid, 0.0))
                                .sum()
                ));

        // 7. 总分排名
        List<Map.Entry<String, Double>> sortedTotal = totalScores.entrySet().stream()
                .sorted((a, b) -> Double.compare(b.getValue(), a.getValue()))
                .toList();

        Map<String, Integer> totalRankMap = new LinkedHashMap<>();
        int currentTotalRank = 1;
        for (int i = 0; i < sortedTotal.size(); i++) {
            Map.Entry<String, Double> entry = sortedTotal.get(i);
            if (i > 0 && entry.getValue().equals(sortedTotal.get(i-1).getValue())) {
                totalRankMap.put(entry.getKey(), totalRankMap.get(sortedTotal.get(i-1).getKey()));
            } else {
                totalRankMap.put(entry.getKey(), currentTotalRank);
            }
            currentTotalRank = i + 2;
        }

        // 8. 构建结果
        List<StudentRanking> result = sortedTotal.stream()
                .map(entry -> {
                    String sn = entry.getKey();
                    List<Ranking> rankings = new ArrayList<>();

                    // 添加各科信息
                    for (Long courseId : courseIds) {
                        Double score = actualScores.getOrDefault(sn, new HashMap<>())
                                .getOrDefault(courseId, 0.0);
                        Integer rank = courseRankings.get(courseId).getOrDefault(sn, 0);

                        rankings.add(new Ranking() {{
                            setCourseId(courseId);
                            setScore(score);
                            setRank(rank > 0 ? rank : 0);
                            setTotal(totalStudents);
                            setScope("年级");
                        }});
                    }

                    // 添加总分信息
                    rankings.add(new Ranking() {{
                        setCourseId(0L);
                        setScore(entry.getValue());
                        setRank(totalRankMap.get(sn));
                        setTotal(totalStudents);
                        setScope("年级");
                    }});

                    return new StudentRanking() {{
                        setStudentNumber(sn);
                        setStudentName(nameMap.get(sn));
                        setRanks(rankings);
                    }};
                })
                .collect(Collectors.toList());

        // 缓存结果
        redisUtil.set(redisKey, result, TimeUnit.MINUTES, 10);
        return ResponseResult.success("获取分科组合年级排名成功", result);
    }

    @Override
    public ResponseResult<List<StudentRanking>> getClassRankingWith312(Integer classId, Long examId) {
        String redisKey = "rank:class312:" + classId + ":exam:" + examId;
        if (redisUtil.exists(redisKey)) {
            List<StudentRanking> cached = redisUtil.get(redisKey, List.class);
            return ResponseResult.success("获取成功（缓存）", cached);
        }

        // 1. 获取班级信息
        ClassEntity classEntity = classMapper.selectById(classId);
        if (classEntity == null) return ResponseResult.fail("班级不存在");

        // 2. 获取所有学生
        List<Student> students = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().eq(Student::getClassId, classId));
        if (students.isEmpty()) return ResponseResult.fail("该班级无学生");

        List<String> studentNumbers = students.stream()
                .map(Student::getStudentNumber)
                .collect(Collectors.toList());
        int totalStudents = students.size();
        Map<String, String> nameMap = students.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, Student::getName));

        // 3. 获取选科信息并收集所有科目
        List<StudentSubjectSelection> selections = studentSubjectSelectionMapper.selectList(
                new LambdaQueryWrapper<StudentSubjectSelection>().in(StudentSubjectSelection::getStudentNumber, studentNumbers));
        Map<String, StudentSubjectSelection> selectionMap = selections.stream()
                .collect(Collectors.toMap(StudentSubjectSelection::getStudentNumber, s -> s));

        // 4. 收集班级涉及的所有科目
        Set<Long> allCourseIds = new HashSet<>(List.of(1L, 2L, 3L)); // 主科必含
        selections.forEach(selection -> {
            allCourseIds.add(selection.getSubjectGroupId());
            allCourseIds.add(selection.getElectiveCourse1Id());
            allCourseIds.add(selection.getElectiveCourse2Id());
        });

        // 5. 初始化各科成绩容器（包含所有学生）
        Map<Long, Map<String, Double>> courseScores = new HashMap<>();
        allCourseIds.forEach(courseId -> {
            Map<String, Double> scores = studentNumbers.stream()
                    .collect(Collectors.toMap(sn -> sn, sn -> 0.0));
            courseScores.put(courseId, scores);
        });

        // 6. 查询实际成绩并填充
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
                        .in(Score::getStudentNumber, studentNumbers)
                        .in(Score::getCourseId, allCourseIds)
        );

        scores.forEach(score -> {
            Long courseId = score.getCourseId();
            String sn = score.getStudentNumber();
            courseScores.get(courseId).put(sn, score.getScore() != null ? score.getScore() : 0.0);
        });

        // 7. 计算各科排名（处理并列）
        Map<Long, Map<String, Integer>> courseRankings = new HashMap<>();
        allCourseIds.forEach(courseId -> {
            List<Map.Entry<String, Double>> entries = new ArrayList<>(courseScores.get(courseId).entrySet());
            entries.sort((a, b) -> Double.compare(b.getValue(), a.getValue()));

            Map<String, Integer> rankMap = new LinkedHashMap<>();
            int currentRank = 1;
            for (int i = 0; i < entries.size(); i++) {
                Map.Entry<String, Double> entry = entries.get(i);
                if (i > 0 && entry.getValue().equals(entries.get(i-1).getValue())) {
                    rankMap.put(entry.getKey(), rankMap.get(entries.get(i-1).getKey()));
                } else {
                    rankMap.put(entry.getKey(), currentRank);
                }
                currentRank = i + 1;
            }
            courseRankings.put(courseId, rankMap);
        });

        // 8. 计算总分（根据实际选科）
        Map<String, Double> totalScores = studentNumbers.stream()
                .collect(Collectors.toMap(
                        sn -> sn,
                        sn -> {
                            StudentSubjectSelection selection = selectionMap.get(sn);
                            if (selection == null) { // 未选科只算主科
                                return courseScores.get(1L).get(sn)
                                        + courseScores.get(2L).get(sn)
                                        + courseScores.get(3L).get(sn);
                            }
                            return List.of(1L, 2L, 3L,
                                            selection.getSubjectGroupId(),
                                            selection.getElectiveCourse1Id(),
                                            selection.getElectiveCourse2Id())
                                    .stream()
                                    .mapToDouble(cid -> courseScores.get(cid).get(sn))
                                    .sum();
                        }
                ));

        // 9. 按总分降序排序
        List<Map.Entry<String, Double>> sortedTotal = totalScores.entrySet().stream()
                .sorted((a, b) -> Double.compare(b.getValue(), a.getValue()))
                .toList();

        // 10. 计算总分排名（处理并列）
        Map<String, Integer> totalRankMap = new LinkedHashMap<>();
        int currentTotalRank = 1;
        for (int i = 0; i < sortedTotal.size(); i++) {
            Map.Entry<String, Double> entry = sortedTotal.get(i);
            if (i > 0 && entry.getValue().equals(sortedTotal.get(i-1).getValue())) {
                totalRankMap.put(entry.getKey(), totalRankMap.get(sortedTotal.get(i-1).getKey()));
            } else {
                totalRankMap.put(entry.getKey(), currentTotalRank);
            }
            currentTotalRank = i + 1;
        }

        // 11. 构建结果（按总分排序）
        List<StudentRanking> result = new ArrayList<>();
        for (Map.Entry<String, Double> entry : sortedTotal) {
            String sn = entry.getKey();
            StudentSubjectSelection selection = selectionMap.get(sn);

            // 获取相关科目
            List<Long> relevantCourses = new ArrayList<>(List.of(1L, 2L, 3L));
            if (selection != null) {
                relevantCourses.add(selection.getSubjectGroupId());
                relevantCourses.add(selection.getElectiveCourse1Id());
                relevantCourses.add(selection.getElectiveCourse2Id());
            }

            // 构建各科排名信息
            List<Ranking> rankings = relevantCourses.stream()
                    .map(courseId -> new Ranking(
                            courseId,
                            courseScores.get(courseId).get(sn),
                            courseRankings.get(courseId).get(sn),
                            totalStudents,
                            "班级"
                    )).collect(Collectors.toList());

            // 添加总分排名
            rankings.add(new Ranking(
                    0L,
                    entry.getValue(),
                    totalRankMap.get(sn),
                    totalStudents,
                    "班级"
            ));

            result.add(new StudentRanking(
                    sn,
                    nameMap.get(sn),
                    rankings
            ));
        }

        // 缓存结果
        redisUtil.set(redisKey, result, TimeUnit.MINUTES, 10);
        return ResponseResult.success("获取3+1+2科目排名成功", result);
    }

    @Override
    public ResponseResult<List<StudentRanking>> getGradeRankingWith312(Integer gradeId, Long examId, Integer subjectGroupId) {
        String redisKey = "rank:grade312:" + gradeId + ":exam:" + examId + ":sgid:" + subjectGroupId;
        if (redisUtil.exists(redisKey)) {
            List<StudentRanking> cached = redisUtil.get(redisKey, List.class);
            return ResponseResult.success("获取成功（缓存）", cached);
        }

        // 1. 获取年级所有班级
        List<ClassEntity> classEntities = classMapper.selectList(
                new LambdaQueryWrapper<ClassEntity>().eq(ClassEntity::getGrade, gradeId));
        if (classEntities.isEmpty()) return ResponseResult.fail("年级无班级");

        // 2. 获取所有学生
        List<Student> students = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().in(Student::getClassId,
                        classEntities.stream().map(c -> c.getId().intValue()).collect(Collectors.toList())));
        if (students.isEmpty()) return ResponseResult.fail("无学生信息");

        List<String> studentNumbers = students.stream()
                .map(Student::getStudentNumber)
                .collect(Collectors.toList());
        Map<String, String> nameMap = students.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, Student::getName));

        // 3. 获取有效选科学生（过滤指定科目组合）
        List<StudentSubjectSelection> selections = studentSubjectSelectionMapper.selectList(
                new LambdaQueryWrapper<StudentSubjectSelection>()
                        .in(StudentSubjectSelection::getStudentNumber, studentNumbers)
                        .eq(StudentSubjectSelection::getSubjectGroupId, subjectGroupId));
        if (selections.isEmpty()) return ResponseResult.fail("该选科组合无学生");

        // 4. 收集所有相关科目
        Set<Long> allCourseIds = new HashSet<>();
        Map<String, StudentSubjectSelection> selectionMap = new HashMap<>();
        selections.forEach(selection -> {
            selectionMap.put(selection.getStudentNumber(), selection);
            allCourseIds.addAll(List.of(
                    1L, 2L, 3L, // 主科
                    selection.getSubjectGroupId(),
                    selection.getElectiveCourse1Id(),
                    selection.getElectiveCourse2Id()
            ));
        });

        // 5. 初始化各科成绩容器
        Map<Long, Map<String, Double>> courseScores = new HashMap<>();
        allCourseIds.forEach(courseId -> {
            Map<String, Double> scores = studentNumbers.stream()
                    .collect(Collectors.toMap(sn -> sn, sn -> 0.0));
            courseScores.put(courseId, scores);
        });

        // 6. 查询并填充成绩
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
                        .in(Score::getStudentNumber, selectionMap.keySet())
                        .in(Score::getCourseId, allCourseIds));

        scores.forEach(score -> {
            Long courseId = score.getCourseId();
            String sn = score.getStudentNumber();
            courseScores.get(courseId).put(sn, score.getScore() != null ? score.getScore() : 0.0);
        });

        // 7. 计算各科排名（处理并列）
        Map<Long, Map<String, Integer>> courseRankings = new HashMap<>();
        allCourseIds.forEach(courseId -> {
            List<Map.Entry<String, Double>> entries = new ArrayList<>(courseScores.get(courseId).entrySet());
            entries.sort((a, b) -> Double.compare(b.getValue(), a.getValue()));

            Map<String, Integer> rankMap = new LinkedHashMap<>();
            int currentRank = 1;
            for (int i = 0; i < entries.size(); i++) {
                Map.Entry<String, Double> entry = entries.get(i);
                if (i > 0 && entry.getValue().equals(entries.get(i-1).getValue())) {
                    rankMap.put(entry.getKey(), rankMap.get(entries.get(i-1).getKey()));
                } else {
                    rankMap.put(entry.getKey(), currentRank);
                }
                currentRank = i + 1;
            }
            courseRankings.put(courseId, rankMap);
        });

        // 8. 计算总分（根据选科组合）
        Map<String, Double> totalScores = selectionMap.keySet().stream()
                .collect(Collectors.toMap(
                        sn -> sn,
                        sn -> {
                            StudentSubjectSelection selection = selectionMap.get(sn);
                            return List.of(1L, 2L, 3L,
                                            selection.getSubjectGroupId(),
                                            selection.getElectiveCourse1Id(),
                                            selection.getElectiveCourse2Id())
                                    .stream()
                                    .mapToDouble(cid -> courseScores.get(cid).get(sn))
                                    .sum();
                        }));

        // 9. 总分排序（降序）
        List<Map.Entry<String, Double>> sortedTotal = totalScores.entrySet().stream()
                .sorted((a, b) -> Double.compare(b.getValue(), a.getValue()))
                .toList();

        // 10. 计算总分排名（处理并列）
        Map<String, Integer> totalRankMap = new LinkedHashMap<>();
        int currentTotalRank = 1;
        for (int i = 0; i < sortedTotal.size(); i++) {
            Map.Entry<String, Double> entry = sortedTotal.get(i);
            if (i > 0 && entry.getValue().equals(sortedTotal.get(i-1).getValue())) {
                totalRankMap.put(entry.getKey(), totalRankMap.get(sortedTotal.get(i-1).getKey()));
            } else {
                totalRankMap.put(entry.getKey(), currentTotalRank);
            }
            currentTotalRank = i + 1;
        }

        // 11. 构建结果
        List<StudentRanking> result = sortedTotal.stream()
                .map(entry -> {
                    String sn = entry.getKey();
                    StudentSubjectSelection selection = selectionMap.get(sn);

                    List<Long> relevantCourses = List.of(1L, 2L, 3L,
                            selection.getSubjectGroupId(),
                            selection.getElectiveCourse1Id(),
                            selection.getElectiveCourse2Id());

                    List<Ranking> rankings = relevantCourses.stream()
                            .map(courseId -> new Ranking(
                                    courseId,
                                    courseScores.get(courseId).get(sn),
                                    courseRankings.get(courseId).get(sn),
                                    sortedTotal.size(),
                                    "年级"
                            )).collect(Collectors.toList());

                    rankings.add(new Ranking(
                            0L,
                            entry.getValue(),
                            totalRankMap.get(sn),
                            sortedTotal.size(),
                            "年级"
                    ));

                    return new StudentRanking(
                            sn,
                            nameMap.get(sn),
                            rankings
                    );
                })
                .collect(Collectors.toList());

        // 缓存结果
        redisUtil.set(redisKey, result, TimeUnit.MINUTES, 10);
        return ResponseResult.success("获取3+1+2年级排名成功", result);
    }


    //
    private ResponseResult<List<StudentRanking>> buildRankingsFromCache(Set<Object> members, String redisKey, String scope) {
        List<Student> students = studentMapper.selectList(new LambdaQueryWrapper<Student>()
                .in(Student::getStudentNumber, members));
        Map<String, String> nameMap = students.stream().collect(Collectors.toMap(Student::getStudentNumber, Student::getName));

        List<StudentRanking> result = new ArrayList<>();
        int total = members.size();
        for (Object sn : members) {
            Double score = redisUtil.zscore(redisKey, sn);
            Long rank = redisUtil.zrevrank(redisKey, sn);

            StudentRanking sr = new StudentRanking();
            sr.setStudentNumber((String) sn);
            sr.setStudentName(nameMap.getOrDefault(sn, "未知"));

            Ranking r = new Ranking();
            r.setCourseId(0L);
            r.setScore(score);
            r.setRank(rank != null ? rank.intValue() + 1 : null);
            r.setTotal(total);
            r.setScope(scope);

            sr.setRanks(List.of(r));
            result.add(sr);
        }
        return ResponseResult.success(scope + "总分排名成功", result);
    }

    //获取分科对应的科目
    private Long getExtraCourseId(int subjectGroupId) {
        return switch (subjectGroupId) {
            case 2 -> 5L;
            case 3 -> 4L;
            default -> 0L;
        };
    }


    private Ranking buildTotalRanking(Student student, Long examId, String scope) {
        List<Long> requiredCourseIds = new ArrayList<>(List.of(1L, 2L, 3L));
        StudentSubjectSelection selection = studentSubjectSelectionMapper.selectOne(
                new LambdaQueryWrapper<StudentSubjectSelection>().eq(StudentSubjectSelection::getStudentNumber, student.getStudentNumber()));
        if (selection == null) return null;
        requiredCourseIds.add(selection.getElectiveCourse1Id());
        requiredCourseIds.add(selection.getElectiveCourse2Id());
        if (selection.getSubjectGroupId() == 2) requiredCourseIds.add(5L);
        else if (selection.getSubjectGroupId() == 3) requiredCourseIds.add(4L);

        String redisKey;
        Map<String, Double> totalScoreMap = new HashMap<>();

        if ("班级".equals(scope)) {
            List<Student> classStudents = studentMapper.selectList(
                    new LambdaQueryWrapper<Student>().eq(Student::getClassId, student.getClassId()));
            List<String> numbers = classStudents.stream().map(Student::getStudentNumber).toList();

            List<Score> scores = scoreMapper.selectList(
                    new LambdaQueryWrapper<Score>()
                            .eq(Score::getExamId, examId)
                            .in(Score::getStudentNumber, numbers)
                            .in(Score::getCourseId, requiredCourseIds));
            for (Score s : scores) {
                if (s.getScore() == null) continue;
                totalScoreMap.merge(s.getStudentNumber(), s.getScore(), Double::sum);
            }

            redisKey = "classTotalScore:" + examId + ":" + student.getClassId();
        } else {
            List<Student> allStudent = studentMapper.selectList(null);
            Map<String, Long> studentGroupMap = allStudent.stream()
                    .collect(Collectors.toMap(Student::getStudentNumber, s -> {
                        ClassEntity c = classMapper.selectById(s.getClassId());
                        return c != null ? c.getSubjectGroupId() : null;
                    }));
            List<Score> scores = scoreMapper.selectList(
                    new LambdaQueryWrapper<Score>().eq(Score::getExamId, examId));
            for (Score s : scores) {
                String sNum = s.getStudentNumber();
                if (s.getScore() == null || !studentGroupMap.getOrDefault(sNum, -1L).equals(selection.getSubjectGroupId())) continue;
                totalScoreMap.merge(sNum, s.getScore(), Double::sum);
            }
            redisKey = "gradeTotalScore:" + examId + ":" + selection.getSubjectGroupId();
        }

        if (!redisUtil.exists(redisKey)) {
            redisUtil.delete(redisKey);
            for (Map.Entry<String, Double> entry : totalScoreMap.entrySet()) {
                redisUtil.zadd(redisKey, entry.getValue(), entry.getKey());
            }
            redisUtil.expire(redisKey, 30, TimeUnit.MINUTES);
        }

        Long rank = redisUtil.zrevrank(redisKey, student.getStudentNumber());
        Long total = redisUtil.zcount(redisKey);
        Double score = redisUtil.zscore(redisKey, student.getStudentNumber());

        return new Ranking() {{
            setCourseId(0L);
            setScore(score != null ? score : 0.0);
            setRank(rank != null ? rank.intValue() + 1 : null);
            setTotal(total != null ? total.intValue() : 0);
            setScope(scope);
        }};
    }


    @Override
    public ResponseResult<List<StudentRanking>> getClassTop3CourseTotalRankings(Integer classId, Long examId) {
        // 1. 查询班级下所有学生
        List<Student> students = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().eq(Student::getClassId, classId)
        );
        if (students.isEmpty()) {
            return ResponseResult.fail("该班级没有学生");
        }

        List<String> studentNumbers = students.stream()
                .map(Student::getStudentNumber)
                .collect(Collectors.toList());
        int totalStudents = students.size();
        Map<String, String> studentNameMap = students.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, Student::getName));

        // 2. 初始化各科成绩容器（包含所有学生）
        Map<Long, List<ScoreEntry>> courseScoreEntries = new HashMap<>();
        List<Long> courseIds = List.of(1L, 2L, 3L);
        for (Long courseId : courseIds) {
            List<ScoreEntry> entries = studentNumbers.stream()
                    .map(sn -> new ScoreEntry(sn, 0.0)) // 默认0分
                    .collect(Collectors.toList());
            courseScoreEntries.put(courseId, entries);
        }

        // 3. 查询并更新实际成绩
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
                        .in(Score::getStudentNumber, studentNumbers)
                        .in(Score::getCourseId, courseIds)
        );

        // 用实际成绩覆盖默认值
        Map<String, Map<Long, Double>> actualScores = new HashMap<>();
        for (Score score : scores) {
            String sn = score.getStudentNumber();
            Long cid = score.getCourseId();
            Double val = score.getScore() != null ? score.getScore() : 0.0;

            actualScores.computeIfAbsent(sn, k -> new HashMap<>()).put(cid, val);
        }

        // 更新各科成绩列表
        for (Long courseId : courseIds) {
            List<ScoreEntry> entries = courseScoreEntries.get(courseId);
            for (ScoreEntry entry : entries) {
                Double actual = actualScores.getOrDefault(entry.studentNumber, new HashMap<>())
                        .getOrDefault(courseId, 0.0);
                entry.score = actual;
            }
        }

        // 4. 计算各科排名（包含所有学生）
        Map<Long, Map<String, Integer>> courseRankings = new HashMap<>();
        for (Long courseId : courseIds) {
            List<ScoreEntry> sorted = courseScoreEntries.get(courseId).stream()
                    .sorted((a, b) -> Double.compare(b.score, a.score))
                    .toList();

            Map<String, Integer> rankMap = new LinkedHashMap<>();
            int currentRank = 1;
            for (int i = 0; i < sorted.size(); i++) {
                ScoreEntry entry = sorted.get(i);
                // 处理并列
                if (i > 0 && entry.score.equals(sorted.get(i-1).score)) {
                    rankMap.put(entry.studentNumber, rankMap.get(sorted.get(i-1).studentNumber));
                } else {
                    rankMap.put(entry.studentNumber, currentRank);
                }
                currentRank = i + 2;
            }
            courseRankings.put(courseId, rankMap);
        }

        // 5. 计算总分（包含所有学生）
        Map<String, Double> totalScores = studentNumbers.stream()
                .collect(Collectors.toMap(
                        sn -> sn,
                        sn -> courseIds.stream()
                                .mapToDouble(cid -> actualScores.getOrDefault(sn, new HashMap<>())
                                        .getOrDefault(cid, 0.0))
                                .sum()
                ));

        // 6. 总分排名（包含所有学生）
        List<Map.Entry<String, Double>> sortedTotal = totalScores.entrySet().stream()
                .sorted((a, b) -> Double.compare(b.getValue(), a.getValue()))
                .toList();

        Map<String, Integer> totalRankMap = new LinkedHashMap<>();
        int currentTotalRank = 1;
        for (int i = 0; i < sortedTotal.size(); i++) {
            Map.Entry<String, Double> entry = sortedTotal.get(i);
            if (i > 0 && entry.getValue().equals(sortedTotal.get(i-1).getValue())) {
                totalRankMap.put(entry.getKey(), totalRankMap.get(sortedTotal.get(i-1).getKey()));
            } else {
                totalRankMap.put(entry.getKey(), currentTotalRank);
            }
            currentTotalRank = i + 2;
        }

        // 7. 构建结果（确保所有学生都被包含）
        List<StudentRanking> result = sortedTotal.stream()
                .map(entry -> {
                    String sn = entry.getKey();
                    List<Ranking> rankings = new ArrayList<>();

                    // 添加三科信息
                    for (Long courseId : courseIds) {
                        Double score = actualScores.getOrDefault(sn, new HashMap<>())
                                .getOrDefault(courseId, 0.0);
                        Integer rank = courseRankings.get(courseId).getOrDefault(sn, 0);

                        rankings.add(new Ranking() {{
                            setCourseId(courseId);
                            setScore(score);
                            setRank(rank > 0 ? rank : 0); // 无记录显示0
                            setTotal(totalStudents);
                            setScope("班级");
                        }});
                    }

                    // 添加总分信息
                    rankings.add(new Ranking() {{
                        setCourseId(0L);
                        setScore(entry.getValue());
                        setRank(totalRankMap.get(sn));
                        setTotal(totalStudents);
                        setScope("班级");
                    }});

                    return new StudentRanking() {{
                        setStudentNumber(sn);
                        setStudentName(studentNameMap.get(sn));
                        setRanks(rankings);
                    }};
                })
                .collect(Collectors.toList());

        return ResponseResult.success("获取三科成绩及排名成功", result);
    }


    @Override
    public ResponseResult<List<StudentRanking>> getGradeTop3CourseTotalRankings(Integer gradeId, Long examId) {
        // 1. 获取年级下所有班级
        List<ClassEntity> classEntities = classMapper.selectList(
                new LambdaQueryWrapper<ClassEntity>().eq(ClassEntity::getGrade, gradeId)
        );
        if (classEntities.isEmpty()) {
            return ResponseResult.fail("该年级没有班级");
        }

        List<Long> classIds = classEntities.stream()
                .map(ClassEntity::getId)
                .collect(Collectors.toList());

        // 2. 获取所有学生
        List<Student> students = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().in(Student::getClassId, classIds)
        );
        if (students.isEmpty()) {
            return ResponseResult.fail("该年级没有学生");
        }

        List<String> studentNumbers = students.stream()
                .map(Student::getStudentNumber)
                .collect(Collectors.toList());
        int totalStudents = studentNumbers.size();
        Map<String, String> studentNameMap = students.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, Student::getName));

        // 3. 初始化各科成绩容器（包含所有学生）
        Map<Long, List<ScoreEntry>> courseScoreEntries = new HashMap<>();
        List<Long> courseIds = List.of(1L, 2L, 3L);
        for (Long courseId : courseIds) {
            List<ScoreEntry> entries = studentNumbers.stream()
                    .map(sn -> new ScoreEntry(sn, 0.0)) // 默认0分
                    .collect(Collectors.toList());
            courseScoreEntries.put(courseId, entries);
        }

        // 4. 查询并更新实际成绩
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
                        .in(Score::getStudentNumber, studentNumbers)
                        .in(Score::getCourseId, courseIds)
        );

        Map<String, Map<Long, Double>> actualScores = new HashMap<>();
        for (Score score : scores) {
            String sn = score.getStudentNumber();
            Long cid = score.getCourseId();
            Double val = score.getScore() != null ? score.getScore() : 0.0;

            actualScores.computeIfAbsent(sn, k -> new HashMap<>()).put(cid, val);
        }

        // 更新各科成绩列表
        for (Long courseId : courseIds) {
            List<ScoreEntry> entries = courseScoreEntries.get(courseId);
            for (ScoreEntry entry : entries) {
                Double actual = actualScores.getOrDefault(entry.studentNumber, new HashMap<>())
                        .getOrDefault(courseId, 0.0);
                entry.score = actual;
            }
        }

        // 5. 计算各科排名（年级范围）
        Map<Long, Map<String, Integer>> courseRankings = new HashMap<>();
        for (Long courseId : courseIds) {
            List<ScoreEntry> sorted = courseScoreEntries.get(courseId).stream()
                    .sorted((a, b) -> Double.compare(b.score, a.score))
                    .toList();

            Map<String, Integer> rankMap = new LinkedHashMap<>();
            int currentRank = 1;
            for (int i = 0; i < sorted.size(); i++) {
                ScoreEntry entry = sorted.get(i);
                // 处理并列
                if (i > 0 && entry.score.equals(sorted.get(i-1).score)) {
                    rankMap.put(entry.studentNumber, rankMap.get(sorted.get(i-1).studentNumber));
                } else {
                    rankMap.put(entry.studentNumber, currentRank);
                }
                currentRank = i + 2;
            }
            courseRankings.put(courseId, rankMap);
        }

        // 6. 计算总分（包含所有学生）
        Map<String, Double> totalScores = studentNumbers.stream()
                .collect(Collectors.toMap(
                        sn -> sn,
                        sn -> courseIds.stream()
                                .mapToDouble(cid -> actualScores.getOrDefault(sn, new HashMap<>())
                                        .getOrDefault(cid, 0.0))
                                .sum()
                ));

        // 7. 总分排名（年级范围）
        List<Map.Entry<String, Double>> sortedTotal = totalScores.entrySet().stream()
                .sorted((a, b) -> Double.compare(b.getValue(), a.getValue()))
                .toList();

        Map<String, Integer> totalRankMap = new LinkedHashMap<>();
        int currentTotalRank = 1;
        for (int i = 0; i < sortedTotal.size(); i++) {
            Map.Entry<String, Double> entry = sortedTotal.get(i);
            if (i > 0 && entry.getValue().equals(sortedTotal.get(i-1).getValue())) {
                totalRankMap.put(entry.getKey(), totalRankMap.get(sortedTotal.get(i-1).getKey()));
            } else {
                totalRankMap.put(entry.getKey(), currentTotalRank);
            }
            currentTotalRank = i + 2;
        }

        // 8. 构建最终结果
        List<StudentRanking> result = sortedTotal.stream()
                .map(entry -> {
                    String sn = entry.getKey();
                    List<Ranking> rankings = new ArrayList<>();

                    // 添加三科信息
                    for (Long courseId : courseIds) {
                        Double score = actualScores.getOrDefault(sn, new HashMap<>())
                                .getOrDefault(courseId, 0.0);
                        Integer rank = courseRankings.get(courseId).getOrDefault(sn, 0);

                        rankings.add(new Ranking() {{
                            setCourseId(courseId);
                            setScore(score);
                            setRank(rank > 0 ? rank : 0);
                            setTotal(totalStudents);
                            setScope("年级");
                        }});
                    }

                    // 添加总分信息
                    rankings.add(new Ranking() {{
                        setCourseId(0L);
                        setScore(entry.getValue());
                        setRank(totalRankMap.get(sn));
                        setTotal(totalStudents);
                        setScope("年级");
                    }});

                    return new StudentRanking() {{
                        setStudentNumber(sn);
                        setStudentName(studentNameMap.get(sn));
                        setRanks(rankings);
                    }};
                })
                .collect(Collectors.toList());

        return ResponseResult.success("获取年级三科成绩及排名成功", result);
    }



    private String buildRedisKey(Long examId,Long courseId,Long subjectGroupId){
        return String.format("rank:%d:%d:%d", examId, courseId, subjectGroupId);
    }

    private void buildGradeRankingToRedis(Long examId){
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
        );
        if(scores.isEmpty()) return;
        Map<Long, ClassEntity> classMap = classMapper.selectList(null)
                .stream().collect(Collectors.toMap(ClassEntity::getId, c -> c));
        Map<String, Student> studentMap = studentMapper.selectList(null)
                .stream().collect(Collectors.toMap(Student::getStudentNumber, s -> s));

        Map<String, List<Score>> grouped = new HashMap<>();

        for (Score score : scores) {
            Student student = studentMap.get(score.getStudentNumber());
            if (student == null || score.getScore() == null) continue;
            ClassEntity clazz = classMap.get(Long.valueOf(student.getClassId()));
            if (clazz == null) continue;

            String redisKey = buildRedisKey(score.getExamId(), score.getCourseId(), clazz.getSubjectGroupId());
            grouped.computeIfAbsent(redisKey, k -> new ArrayList<>()).add(score);
        }

        for (Map.Entry<String, List<Score>> entry : grouped.entrySet()) {
            String redisKey = entry.getKey();
            redisUtil.delete(redisKey); // 清理旧缓存
            for (Score score : entry.getValue()) {
                redisUtil.zadd(redisKey, score.getScore(), score.getStudentNumber());
            }
        }
    }

    @Override
    public ResponseResult<List<StudentRanking>> getClassAllRankings(Integer classId, Long examId) {
        // 1. 获取班级学生
        List<Student> classStudents = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().eq(Student::getClassId, classId)
        );
        if (classStudents.isEmpty()) return ResponseResult.fail("该班级没有学生");

        List<String> classStudentNumbers = classStudents.stream().map(Student::getStudentNumber).toList();

        // 2. 获取班级
        ClassEntity clazz = classMapper.selectById(classId);
        if (clazz == null) return ResponseResult.fail("班级不存在");

        int grade = clazz.getGrade();

        // 3. 获取年级学生
        List<ClassEntity> gradeClasses = classMapper.selectList(
                new LambdaQueryWrapper<ClassEntity>().eq(ClassEntity::getGrade, grade)
        );
        List<Integer> gradeClassIds = gradeClasses.stream().map(c -> c.getId().intValue()).toList();

        List<Student> gradeStudents = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().in(Student::getClassId, gradeClassIds)
        );
        List<String> gradeStudentNumbers = gradeStudents.stream().map(Student::getStudentNumber).toList();

        Map<String, String> studentNameMap = gradeStudents.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, Student::getName));

        // 4. 获取选科信息
        Map<String, StudentSubjectSelection> selectionMap = studentSubjectSelectionMapper.selectBatchIds(gradeStudentNumbers).stream()
                .collect(Collectors.toMap(StudentSubjectSelection::getStudentNumber, s -> s));

        // 5. 获取成绩
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
                        .in(Score::getStudentNumber, gradeStudentNumbers)
        );

        Set<Long> allCourseIds = scores.stream().map(Score::getCourseId).collect(Collectors.toSet());

        Map<String, Map<Long, Double>> scoreMap = new HashMap<>();
        for (Score score : scores) {
            scoreMap.computeIfAbsent(score.getStudentNumber(), k -> new HashMap<>())
                    .put(score.getCourseId(), score.getScore() != null ? score.getScore() : 0.0);
        }

        for (String sn : gradeStudentNumbers) {
            scoreMap.putIfAbsent(sn, new HashMap<>());
            for (Long cid : allCourseIds) {
                scoreMap.get(sn).putIfAbsent(cid, 0.0);
            }
        }

        // 6. 固定语数英 ID
        List<Long> core3 = List.of(1L, 2L, 3L);

        // 获取样本学生选科
        StudentSubjectSelection sample = selectionMap.getOrDefault(classStudentNumbers.get(0), null);
        if (sample == null) return ResponseResult.fail("未找到选科信息");

        Long subject4 = sample.getElectiveCourse1Id();
        Long subject5 = sample.getElectiveCourse2Id();
        Long subjectOne = sample.getSubjectGroupId();

        List<Long> threePlusOnePlusTwo = new ArrayList<>(core3);
        threePlusOnePlusTwo.add(subjectOne);
        threePlusOnePlusTwo.add(subject4);
        threePlusOnePlusTwo.add(subject5);

        // 分数统计
        Map<String, Double> totalScores = new HashMap<>();
        Map<String, Double> threeScores = new HashMap<>();
        Map<String, Double> tptScores = new HashMap<>();

        for (String sn : gradeStudentNumbers) {
            Map<Long, Double> sc = scoreMap.get(sn);
            totalScores.put(sn, sc.values().stream().mapToDouble(Double::doubleValue).sum());
            threeScores.put(sn, core3.stream().mapToDouble(cid -> sc.getOrDefault(cid, 0.0)).sum());
            tptScores.put(sn, threePlusOnePlusTwo.stream().mapToDouble(cid -> sc.getOrDefault(cid, 0.0)).sum());
        }

        // 文理科分组（根据 subjectGroupId）
        List<String> liberalArtsStudents = gradeStudents.stream()
                .filter(s -> selectionMap.get(s.getStudentNumber()) != null &&
                        selectionMap.get(s.getStudentNumber()).getSubjectGroupId() == 2L)
                .map(Student::getStudentNumber)
                .toList();

        List<String> scienceStudents = gradeStudents.stream()
                .filter(s -> selectionMap.get(s.getStudentNumber()) != null &&
                        selectionMap.get(s.getStudentNumber()).getSubjectGroupId() == 3L)
                .map(Student::getStudentNumber)
                .toList();

        List<String> liberalArtsClass = classStudentNumbers.stream()
                .filter(sn -> selectionMap.get(sn) != null &&
                        selectionMap.get(sn).getSubjectGroupId() == 2L)
                .toList();

        List<String> scienceClass = classStudentNumbers.stream()
                .filter(sn -> selectionMap.get(sn) != null &&
                        selectionMap.get(sn).getSubjectGroupId() == 3L)
                .toList();

        // 排名计算
        Map<String, Integer> gradeTotalRank = calculateRank(totalScores, gradeStudentNumbers);
        Map<String, Integer> classTotalRank = calculateRank(totalScores, classStudentNumbers);

        Map<String, Integer> grade3Rank = calculateRank(threeScores, gradeStudentNumbers);
        Map<String, Integer> class3Rank = calculateRank(threeScores, classStudentNumbers);

        Map<String, Integer> gradeTptRank = new HashMap<>();
        gradeTptRank.putAll(calculateRank(tptScores, liberalArtsStudents));
        gradeTptRank.putAll(calculateRank(tptScores, scienceStudents));

        Map<String, Integer> classTptRank = new HashMap<>();
        classTptRank.putAll(calculateRank(tptScores, liberalArtsClass));
        classTptRank.putAll(calculateRank(tptScores, scienceClass));

        // 单科排名
        Map<Long, Map<String, Integer>> courseGradeRanks = new HashMap<>();
        Map<Long, Map<String, Integer>> courseClassRanks = new HashMap<>();

        for (Long cid : allCourseIds) {
            Map<String, Double> courseScores = new HashMap<>();
            for (String sn : gradeStudentNumbers) {
                courseScores.put(sn, scoreMap.get(sn).getOrDefault(cid, 0.0));
            }
            courseGradeRanks.put(cid, calculateRank(courseScores, gradeStudentNumbers));

            Map<String, Double> classScores = courseScores.entrySet().stream()
                    .filter(e -> classStudentNumbers.contains(e.getKey()))
                    .collect(Collectors.toMap(Map.Entry::getKey, Map.Entry::getValue));
            courseClassRanks.put(cid, calculateRank(classScores, classStudentNumbers));
        }

        List<StudentRanking> result = classStudentNumbers.stream().map(sn -> {
            List<Ranking> rankings = new ArrayList<>();

            for (Long cid : allCourseIds) {
                rankings.add(new Ranking(cid, scoreMap.get(sn).get(cid), courseClassRanks.get(cid).get(sn), classStudentNumbers.size(), "班级"));
                rankings.add(new Ranking(cid, scoreMap.get(sn).get(cid), courseGradeRanks.get(cid).get(sn), gradeStudentNumbers.size(), "年级"));
            }

            rankings.add(new Ranking(-2L, threeScores.get(sn), class3Rank.get(sn), classStudentNumbers.size(), "班级"));
            rankings.add(new Ranking(-2L, threeScores.get(sn), grade3Rank.get(sn), gradeStudentNumbers.size(), "年级"));

            // ✅ 核心修改：根据文理分组分别统计 total
            boolean isLiberal = selectionMap.get(sn) != null && selectionMap.get(sn).getSubjectGroupId() == 2L;
            int classTptTotal = isLiberal ? liberalArtsClass.size() : scienceClass.size();
            int gradeTptTotal = isLiberal ? liberalArtsStudents.size() : scienceStudents.size();

            rankings.add(new Ranking(-3L, tptScores.get(sn), classTptRank.get(sn), classTptTotal, "班级"));
            rankings.add(new Ranking(-3L, tptScores.get(sn), gradeTptRank.get(sn), gradeTptTotal, "年级"));

            rankings.add(new Ranking(-1L, totalScores.get(sn), classTotalRank.get(sn), classStudentNumbers.size(), "班级"));
            rankings.add(new Ranking(-1L, totalScores.get(sn), gradeTotalRank.get(sn), gradeStudentNumbers.size(), "年级"));

            StudentRanking studentRanking = new StudentRanking();
            studentRanking.setStudentNumber(sn);
            studentRanking.setStudentName(studentNameMap.get(sn));
            studentRanking.setRanks(rankings);
            return studentRanking;
        }).toList();


        return ResponseResult.success("查询成功", result);
    }

    @Override
    public ResponseResult<List<StudentRanking>> getGradeSubjectGroupRankings(Long examId,Integer subjectGroupId) {
        // 0. 根据 examId 查询考试，获取 gradeId
        Exam exam = examMapper.selectById(examId);
        if (exam == null) {
            return ResponseResult.fail("未找到对应的考试信息");
        }
        Integer gradeId = exam.getGrade();

        // 1. 获取年级中 subjectGroupId 相同的班级
        List<ClassEntity> targetClasses = classMapper.selectList(
                new LambdaQueryWrapper<ClassEntity>()
                        .eq(ClassEntity::getGrade, gradeId)
                        .eq(ClassEntity::getSubjectGroupId, subjectGroupId)
        );
        if (targetClasses.isEmpty()) {
            return ResponseResult.fail("该年级下没有对应分科的班级");
        }

        List<Integer> classIds = targetClasses.stream().map(c -> c.getId().intValue()).toList();

        // 2. 获取这些班级下的学生
        List<Student> students = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().in(Student::getClassId, classIds)
        );
        if (students.isEmpty()) {
            return ResponseResult.fail("该分科下没有学生");
        }

        List<String> studentNumbers = students.stream().map(Student::getStudentNumber).toList();
        Map<String, String> studentNameMap = students.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, Student::getName));

        // 3. 查询这些学生的成绩
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
                        .in(Score::getStudentNumber, studentNumbers)
        );
        if (scores.isEmpty()) {
            return ResponseResult.fail("该考试下没有学生成绩");
        }

        // 4. 构建学生-课程成绩 Map
        Map<String, Map<Long, Double>> scoreMap = new HashMap<>();
        for (Score score : scores) {
            scoreMap.computeIfAbsent(score.getStudentNumber(), k -> new HashMap<>())
                    .put(score.getCourseId(), score.getScore() != null ? score.getScore() : 0.0);
        }

        // 获取所有出现的课程 id
        Set<Long> allCourseIds = scores.stream().map(Score::getCourseId).collect(Collectors.toSet());

        // 保证每个学生都有完整的成绩项（如果没有，补0）
        for (String sn : studentNumbers) {
            scoreMap.putIfAbsent(sn, new HashMap<>());
            for (Long cid : allCourseIds) {
                scoreMap.get(sn).putIfAbsent(cid, 0.0);
            }
        }

        // 5. 构建 3+1+2 科目组合
        List<Long> threeCourseIds = List.of(1L, 2L, 3L); // 语数英

        // 选出选考科目（非语数英）中选前两个分数最高的科目
        Map<String, Double> totalScoreMap = new HashMap<>();

        for (String sn : studentNumbers) {
            Map<Long, Double> studentScores = scoreMap.get(sn);

            List<Map.Entry<Long, Double>> optionalSubjects = studentScores.entrySet().stream()
                    .filter(e -> !threeCourseIds.contains(e.getKey()))
                    .sorted((a, b) -> Double.compare(b.getValue(), a.getValue()))
                    .toList();

            double threeSum = threeCourseIds.stream().mapToDouble(cid -> studentScores.getOrDefault(cid, 0.0)).sum();
            double one = optionalSubjects.size() >= 1 ? optionalSubjects.get(0).getValue() : 0.0;
            double two = optionalSubjects.size() >= 2 ? optionalSubjects.get(1).getValue() : 0.0;

            double total = threeSum + one + two;
            totalScoreMap.put(sn, total);
        }

        // 6. 计算排名（高分排名靠前）
        Map<String, Integer> rankMap = calculateRank(totalScoreMap);

        // 7. 构建返回结构
        List<StudentRanking> result = studentNumbers.stream().map(sn -> {
            Ranking rank = new Ranking(-100L, totalScoreMap.get(sn), rankMap.get(sn), studentNumbers.size(), "3+1+2");

            StudentRanking studentRanking = new StudentRanking();
            studentRanking.setStudentNumber(sn);
            studentRanking.setStudentName(studentNameMap.get(sn));
            studentRanking.setRanks(List.of(rank));
            return studentRanking;
        }).collect(Collectors.toList());

        // 8. 排序：按排名升序
        result.sort(Comparator.comparingInt(sr -> sr.getRanks().get(0).getRank()));

        return ResponseResult.success("查询成功", result);
    }

    // 排名计算函数
    private Map<String, Integer> calculateRank(Map<String, Double> scoreMap, List<String> scope) {
        List<Map.Entry<String, Double>> sorted = scoreMap.entrySet().stream()
                .filter(e -> scope.contains(e.getKey()))
                .sorted(Map.Entry.<String, Double>comparingByValue().reversed())
                .toList();
        Map<String, Integer> result = new HashMap<>();
        for (int i = 0; i < sorted.size(); i++) {
            result.put(sorted.get(i).getKey(), i + 1);
        }
        return result;
    }

    private Map<String, Integer> calculateRank(Map<String, Double> scoreMap) {
        List<Map.Entry<String, Double>> sorted = scoreMap.entrySet().stream()
                .sorted((a, b) -> Double.compare(b.getValue(), a.getValue()))
                .toList();

        Map<String, Integer> rankMap = new LinkedHashMap<>();
        int rank = 1;
        for (int i = 0; i < sorted.size(); i++) {
            if (i > 0 && sorted.get(i).getValue().equals(sorted.get(i - 1).getValue())) {
                rankMap.put(sorted.get(i).getKey(), rankMap.get(sorted.get(i - 1).getKey()));
            } else {
                rankMap.put(sorted.get(i).getKey(), rank);
            }
            rank = i + 2;
        }
        return rankMap;
    }

    // 辅助类用于临时存储成绩数据
    private static class ScoreEntry {
        String studentNumber;
        Double score;

        public ScoreEntry(String studentNumber, Double score) {
            this.studentNumber = studentNumber;
            this.score = score;
        }
    }
}
