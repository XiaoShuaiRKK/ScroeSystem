package com.score.system.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.baomidou.mybatisplus.core.conditions.query.QueryWrapper;
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
public class ScoreServiceImpl implements ScoreService {
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
        LambdaQueryWrapper<StudentSubjectSelection> query = new LambdaQueryWrapper<>();
        query.eq(StudentSubjectSelection::getStudentNumber, score.getStudentNumber());
        StudentSubjectSelection selection = studentSubjectSelectionMapper.selectOne(query);
        if(selection == null){
            return ResponseResult.fail("学生还未设置分科: " + score.getStudentNumber());
        }
        // 查看学生是否选择了课程
        Long courseId = score.getCourseId();
        List<Long> mustCourseId = new ArrayList<>();
        mustCourseId.add(1L);
        mustCourseId.add(2L);
        mustCourseId.add(3L);
        mustCourseId.add(selection.getElectiveCourse1Id());
        mustCourseId.add(selection.getElectiveCourse2Id());
        if(selection.getSubjectGroupId() == 2){
            mustCourseId.add(5L);
        } else if (selection.getSubjectGroupId() == 3) {
            mustCourseId.add(4L);
        }
        boolean isSelected = false;
        for(Long l : mustCourseId){
            if (courseId.equals(l)) {
                isSelected = true;
                break;
            }
        }
        if(!isSelected){
            return ResponseResult.fail("学生:" + score.getStudentNumber() + "未选择该课程，无法录入成绩");
        }
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
        try {
            for(Score score : scores){
                ResponseResult<Boolean> result = addScore(score);
                if(result.getCode() == 400){
                    throw new IllegalArgumentException(result.getMessage());
                }else if (result.getCode() == 500){
                    throw new RuntimeException(result.getMessage());
                }
            }
        }catch (IllegalArgumentException e){
            return ResponseResult.fail(e.getMessage());
        }
        return ResponseResult.success("批量添加成功",true);
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
        // 构建唯一Redis缓存键
        String redisKey = "classCourseRankings:" + classId + ":" + examId;
        // 检查缓存是否存在
        if (redisUtil.exists(redisKey)) {
            List<StudentRanking> cached = redisUtil.get(redisKey, List.class);
            return ResponseResult.success("获取班级各科成绩排名成功（缓存）", cached);
        }

        // 1. 查出班级所有学生
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

        // 2. 查出本次考试所有成绩
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
                        .in(Score::getStudentNumber, studentNumbers)
        );

        // 3. 构建 Map<courseId, Map<studentNumber, score>>
        Map<Long, Map<String, Double>> courseStudentScoreMap = new HashMap<>();
        for (Score score : scores) {
            if (score.getScore() != null) {
                courseStudentScoreMap
                        .computeIfAbsent(score.getCourseId(), k -> new HashMap<>())
                        .put(score.getStudentNumber(), score.getScore());
            }
        }

        // 4. 初始化所有学生对象
        Map<String, StudentRanking> studentRankingMap = new HashMap<>();
        for (String studentNumber : studentNumbers) {
            StudentRanking sr = new StudentRanking();
            sr.setStudentNumber(studentNumber);
            sr.setStudentName(studentNameMap.get(studentNumber));
            sr.setRanks(new ArrayList<>());
            studentRankingMap.put(studentNumber, sr);
        }

        // 5. 对每门课程进行排名
        for (Map.Entry<Long, Map<String, Double>> entry : courseStudentScoreMap.entrySet()) {
            Long courseId = entry.getKey();
            Map<String, Double> studentScores = entry.getValue();

            // 排序：按成绩倒序
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
                ranking.setScope("班级");

                studentRankingMap.get(studentNumber).getRanks().add(ranking);
            }

            // 处理无成绩的学生
            for (String studentNumber : studentNumbers) {
                if (!studentScores.containsKey(studentNumber)) {
                    Ranking ranking = new Ranking();
                    ranking.setCourseId(courseId);
                    ranking.setScore(null);
                    ranking.setRank(null);
                    ranking.setTotal(total);
                    ranking.setScope("班级");

                    studentRankingMap.get(studentNumber).getRanks().add(ranking);
                }
            }
        }

        // 转换为列表并过滤空数据
        List<StudentRanking> result = new ArrayList<>(studentRankingMap.values());

        // 将结果存入Redis，有效期10分钟
        redisUtil.set(redisKey, result, TimeUnit.MINUTES, 10);

        return ResponseResult.success("获取班级各科成绩排名成功", result);
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
                    ranking.setScore(null);
                    ranking.setRank(null);
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
        String redisKey = "rank:class:" + classId + ":exam:" + examId;
        if (redisUtil.exists(redisKey)) {
            List<StudentRanking> cached = redisUtil.get(redisKey, List.class);
            return ResponseResult.success("获取成功（缓存）", cached);
        }

        // 获取班级信息
        ClassEntity classEntity = classMapper.selectById(classId);
        if (classEntity == null) return ResponseResult.fail("班级不存在");

        int subjectGroupId = classEntity.getSubjectGroupId().intValue();
        Long extraCourseId = getExtraCourseId(subjectGroupId);

        // 获取学生
        List<Student> students = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().eq(Student::getClassId, classId));
        if (students.isEmpty()) return ResponseResult.fail("该班级无学生");

        Map<String, String> nameMap = students.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, Student::getName));
        List<String> studentNumbers = new ArrayList<>(nameMap.keySet());

        // 获取成绩
        List<Score> scores = scoreMapper.selectList(new LambdaQueryWrapper<Score>()
                .eq(Score::getExamId, examId)
                .in(Score::getStudentNumber, studentNumbers)
                .in(Score::getCourseId, List.of(1L, 2L, 3L, extraCourseId)));

        Map<String, Double> totalScoreMap = new HashMap<>();
        for (Score score : scores) {
            totalScoreMap.merge(score.getStudentNumber(), score.getScore() == null ? 0.0 : score.getScore(), Double::sum);
        }

        for (String sn : studentNumbers) {
            totalScoreMap.putIfAbsent(sn, 0.0);
        }

        // 排序
        List<Map.Entry<String, Double>> sorted = new ArrayList<>(totalScoreMap.entrySet());
        sorted.sort((a, b) -> Double.compare(b.getValue(), a.getValue())); // 倒序

        // 构建返回数据
        int total = sorted.size();
        int rank = 1;
        List<StudentRanking> result = new ArrayList<>();
        for (Map.Entry<String, Double> entry : sorted) {
            Ranking r = new Ranking();
            r.setCourseId(0L);
            r.setScore(entry.getValue());
            r.setRank(rank++);
            r.setTotal(total);
            r.setScope("班级");

            StudentRanking sr = new StudentRanking();
            sr.setStudentNumber(entry.getKey());
            sr.setStudentName(nameMap.get(entry.getKey()));
            sr.setRanks(List.of(r));

            result.add(sr);
        }

        // 缓存结果（非 ZSet）
        redisUtil.set(redisKey, result, TimeUnit.MINUTES, 10);
        return ResponseResult.success("获取成功", result);
    }


    @Override
    public ResponseResult<List<StudentRanking>> getGradeRankingWithGroupCourse(Integer gradeId, Long examId, Integer subjectGroupId) {
        String redisKey = "grade:ranking:exam:" + examId + ":grade:" + gradeId + ":subjectGroup:" + subjectGroupId;
        if (redisUtil.exists(redisKey)) {
            List<StudentRanking> cached = redisUtil.get(redisKey, List.class);
            return ResponseResult.success("获取成功（缓存）", cached);
        }

        // 1. 获取年级下所有班级，过滤subjectGroupId相同的
        List<ClassEntity> classEntities = classMapper.selectList(
                        new LambdaQueryWrapper<ClassEntity>().eq(ClassEntity::getGrade, gradeId)
                                .eq(ClassEntity::getSubjectGroupId, subjectGroupId)
                ).stream()
                .toList();

        if (classEntities.isEmpty()) {
            return ResponseResult.fail("该年级无匹配的班级");
        }

        List<Integer> classIds = classEntities.stream().map(e -> e.getId().intValue()).toList();

        // 2. 获取学生
        List<Student> students = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().in(Student::getClassId, classIds)
        );
        if (students.isEmpty()) {
            return ResponseResult.fail("无学生信息");
        }

        Map<String, String> studentNameMap = students.stream().collect(Collectors.toMap(Student::getStudentNumber, Student::getName));
        List<String> studentNumbers = students.stream().map(Student::getStudentNumber).toList();

        // 3. 获取成绩
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
                        .in(Score::getStudentNumber, studentNumbers)
                        .in(Score::getCourseId, List.of(1L, 2L, 3L, getExtraCourseId(subjectGroupId)))
        );

        Map<String, Double> studentTotalScoreMap = new HashMap<>();
        for (Score score : scores) {
            if (score.getScore() != null) {
                studentTotalScoreMap.merge(score.getStudentNumber(), score.getScore(), Double::sum);
            }
        }

        // 所有学生都要参与排名（即使无成绩）
        for (String sn : studentNumbers) {
            studentTotalScoreMap.putIfAbsent(sn, 0.0);
        }

        // 排序
        List<Map.Entry<String, Double>> sorted = new ArrayList<>(studentTotalScoreMap.entrySet());
        sorted.sort((a, b) -> Double.compare(b.getValue(), a.getValue()));

        int total = sorted.size();
        int rank = 1;
        List<StudentRanking> result = new ArrayList<>();
        for (Map.Entry<String, Double> entry : sorted) {
            Ranking r = new Ranking();
            r.setCourseId(0L);
            r.setScore(entry.getValue());
            r.setRank(rank++);
            r.setTotal(total);
            r.setScope("年级");

            StudentRanking sr = new StudentRanking();
            sr.setStudentNumber(entry.getKey());
            sr.setStudentName(studentNameMap.get(entry.getKey()));
            sr.setRanks(List.of(r));

            result.add(sr);
        }

        redisUtil.set(redisKey, result, TimeUnit.MINUTES, 10);
        return ResponseResult.success("获取成功", result);
    }

    @Override
    public ResponseResult<List<StudentRanking>> getClassRankingWith312(Integer classId, Long examId) {
        String redisKey = "rank:class312:" + classId + ":exam:" + examId;
        if (redisUtil.exists(redisKey)) {
            List<StudentRanking> cached = redisUtil.get(redisKey, List.class);
            return ResponseResult.success("获取成功（缓存）", cached);
        }

        // 获取学生
        List<Student> students = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().eq(Student::getClassId, classId));
        if (students.isEmpty()) return ResponseResult.fail("该班级无学生");

        Map<String, String> nameMap = students.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, Student::getName));
        List<String> studentNumbers = new ArrayList<>(nameMap.keySet());

        // 获取选科信息
        List<StudentSubjectSelection> selections = studentSubjectSelectionMapper.selectList(
                new LambdaQueryWrapper<StudentSubjectSelection>().in(StudentSubjectSelection::getStudentNumber, studentNumbers));

        Map<String, List<Long>> electiveMap = new HashMap<>();
        for (StudentSubjectSelection s : selections) {
            List<Long> courseIds = new ArrayList<>();
            courseIds.add(1L); // 语文
            courseIds.add(2L); // 数学
            courseIds.add(3L); // 英语
            courseIds.add(s.getSubjectGroupId()); // 首选（如物理/历史）
            courseIds.add(s.getElectiveCourse1Id());
            courseIds.add(s.getElectiveCourse2Id());
            electiveMap.put(s.getStudentNumber(), courseIds);
        }

        // 获取成绩（每人实际选课不同）
        Map<String, Double> totalScoreMap = new HashMap<>();
        for (String sn : studentNumbers) {
            List<Long> courseIds = electiveMap.getOrDefault(sn, List.of(1L, 2L, 3L)); // 如果没有选课信息就只算主科
            List<Score> scores = scoreMapper.selectList(
                    new LambdaQueryWrapper<Score>()
                            .eq(Score::getExamId, examId)
                            .eq(Score::getStudentNumber, sn)
                            .in(Score::getCourseId, courseIds));
            double total = scores.stream().mapToDouble(s -> s.getScore() == null ? 0.0 : s.getScore()).sum();
            totalScoreMap.put(sn, total);
        }

        // 排序
        List<Map.Entry<String, Double>> sorted = new ArrayList<>(totalScoreMap.entrySet());
        sorted.sort((a, b) -> Double.compare(b.getValue(), a.getValue()));

        int total = sorted.size();
        int rank = 1;
        List<StudentRanking> result = new ArrayList<>();
        for (Map.Entry<String, Double> entry : sorted) {
            Ranking r = new Ranking();
            r.setCourseId(0L);
            r.setScore(entry.getValue());
            r.setRank(rank++);
            r.setTotal(total);
            r.setScope("班级");

            StudentRanking sr = new StudentRanking();
            sr.setStudentNumber(entry.getKey());
            sr.setStudentName(nameMap.get(entry.getKey()));
            sr.setRanks(List.of(r));
            result.add(sr);
        }

        redisUtil.set(redisKey, result, TimeUnit.MINUTES, 10);
        return ResponseResult.success("获取成功", result);
    }

    @Override
    public ResponseResult<List<StudentRanking>> getGradeRankingWith312(Integer gradeId, Long examId, Integer subjectGroupId) {
        String redisKey = "rank:grade312:grade:" + gradeId + ":exam:" + examId + ":sgid:" + subjectGroupId;
        if (redisUtil.exists(redisKey)) {
            List<StudentRanking> cached = redisUtil.get(redisKey, List.class);
            return ResponseResult.success("获取成功（缓存）", cached);
        }

        // 获取所有班级
        List<ClassEntity> classEntities = classMapper.selectList(
                new LambdaQueryWrapper<ClassEntity>().eq(ClassEntity::getGrade, gradeId));
        if (classEntities.isEmpty()) return ResponseResult.fail("年级无班级");

        List<Integer> classIds = classEntities.stream().map(c -> c.getId().intValue()).toList();

        // 获取所有学生
        List<Student> students = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().in(Student::getClassId, classIds));
        if (students.isEmpty()) return ResponseResult.fail("无学生");

        Map<String, String> nameMap = students.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, Student::getName));
        List<String> studentNumbers = students.stream().map(Student::getStudentNumber).toList();

        // 获取该 subjectGroupId 的学生选科信息
        List<StudentSubjectSelection> selections = studentSubjectSelectionMapper.selectList(
                new LambdaQueryWrapper<StudentSubjectSelection>()
                        .in(StudentSubjectSelection::getStudentNumber, studentNumbers)
                        .eq(StudentSubjectSelection::getSubjectGroupId, subjectGroupId));

        if (selections.isEmpty()) return ResponseResult.fail("该年级该选科组合无学生");

        Map<String, List<Long>> electiveMap = new HashMap<>();
        for (StudentSubjectSelection s : selections) {
            List<Long> courseIds = new ArrayList<>();
            courseIds.add(1L); // 语文
            courseIds.add(2L); // 数学
            courseIds.add(3L); // 英语
            courseIds.add(s.getSubjectGroupId());
            courseIds.add(s.getElectiveCourse1Id());
            courseIds.add(s.getElectiveCourse2Id());
            electiveMap.put(s.getStudentNumber(), courseIds);
        }

        // 获取成绩
        Map<String, Double> totalScoreMap = new HashMap<>();
        for (String sn : electiveMap.keySet()) {
            List<Long> courseIds = electiveMap.get(sn);
            List<Score> scores = scoreMapper.selectList(
                    new LambdaQueryWrapper<Score>()
                            .eq(Score::getExamId, examId)
                            .eq(Score::getStudentNumber, sn)
                            .in(Score::getCourseId, courseIds));
            double total = scores.stream().mapToDouble(s -> s.getScore() == null ? 0.0 : s.getScore()).sum();
            totalScoreMap.put(sn, total);
        }

        // 排序
        List<Map.Entry<String, Double>> sorted = new ArrayList<>(totalScoreMap.entrySet());
        sorted.sort((a, b) -> Double.compare(b.getValue(), a.getValue()));

        int total = sorted.size();
        int rank = 1;
        List<StudentRanking> result = new ArrayList<>();
        for (Map.Entry<String, Double> entry : sorted) {
            Ranking r = new Ranking();
            r.setCourseId(0L);
            r.setScore(entry.getValue());
            r.setRank(rank++);
            r.setTotal(total);
            r.setScope("年级");

            StudentRanking sr = new StudentRanking();
            sr.setStudentNumber(entry.getKey());
            sr.setStudentName(nameMap.get(entry.getKey()));
            sr.setRanks(List.of(r));
            result.add(sr);
        }

        redisUtil.set(redisKey, result, TimeUnit.MINUTES, 10);
        return ResponseResult.success("获取成功", result);
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

        List<String> studentNumbers = students.stream().map(Student::getStudentNumber).toList();
        Map<String, String> studentNameMap = students.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, Student::getName));

        // 2. 查询成绩，仅限课程ID为1,2,3
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
                        .in(Score::getStudentNumber, studentNumbers)
                        .in(Score::getCourseId, List.of(1L, 2L, 3L))
        );

        // 3. 汇总三科总分
        Map<String, Double> totalScoreMap = new HashMap<>();
        for (Score score : scores) {
            if (score.getScore() != null) {
                totalScoreMap.merge(score.getStudentNumber(), score.getScore(), Double::sum);
            }
        }

        // 4. 排序
        List<String> allStudentNumbers = new ArrayList<>(studentNumbers);
        allStudentNumbers.sort((a, b) -> {
            Double scoreA = totalScoreMap.getOrDefault(a, 0.0);
            Double scoreB = totalScoreMap.getOrDefault(b, 0.0);
            return Double.compare(scoreB, scoreA); // 降序
        });

        // 5. 构建返回结果
        List<StudentRanking> result = new ArrayList<>();
        int total = allStudentNumbers.size();
        int rank = 1;
        for (String studentNumber : allStudentNumbers) {
            Double score = totalScoreMap.get(studentNumber);

            Ranking ranking = new Ranking();
            ranking.setCourseId(0L); // 表示总分
            ranking.setScore(score);
            ranking.setRank(score != null ? rank++ : null);
            ranking.setTotal(total);
            ranking.setScope("班级");

            StudentRanking sr = new StudentRanking();
            sr.setStudentNumber(studentNumber);
            sr.setStudentName(studentNameMap.get(studentNumber));
            sr.setRanks(List.of(ranking));

            result.add(sr);
        }

        return ResponseResult.success("获取班级三科总分排名成功", result);
    }


    @Override
    public ResponseResult<List<StudentRanking>> getGradeTop3CourseTotalRankings(Integer gradeId, Long examId) {
        // 1. 查询年级下所有班级
        List<ClassEntity> classEntities = classMapper.selectList(
                new LambdaQueryWrapper<ClassEntity>().eq(ClassEntity::getGrade, gradeId)
        );
        if (classEntities.isEmpty()) {
            return ResponseResult.fail("该年级没有班级");
        }

        List<Long> classIds = classEntities.stream().map(ClassEntity::getId).toList();

        // 2. 查询所有学生
        List<Student> students = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().in(Student::getClassId, classIds)
        );
        if (students.isEmpty()) {
            return ResponseResult.fail("该年级没有学生");
        }

        List<String> studentNumbers = students.stream().map(Student::getStudentNumber).toList();
        Map<String, String> studentNameMap = students.stream()
                .collect(Collectors.toMap(Student::getStudentNumber, Student::getName));

        // 3. 查询成绩，仅限课程ID为1,2,3
        List<Score> scores = scoreMapper.selectList(
                new LambdaQueryWrapper<Score>()
                        .eq(Score::getExamId, examId)
                        .in(Score::getStudentNumber, studentNumbers)
                        .in(Score::getCourseId, List.of(1L, 2L, 3L))
        );

        // 4. 汇总总分
        Map<String, Double> totalScoreMap = new HashMap<>();
        for (Score score : scores) {
            if (score.getScore() != null) {
                totalScoreMap.merge(score.getStudentNumber(), score.getScore(), Double::sum);
            }
        }

        // 5. 排序
        List<String> allStudentNumbers = new ArrayList<>(studentNumbers);
        allStudentNumbers.sort((a, b) -> {
            Double scoreA = totalScoreMap.getOrDefault(a, 0.0);
            Double scoreB = totalScoreMap.getOrDefault(b, 0.0);
            return Double.compare(scoreB, scoreA); // 降序
        });

        // 6. 构建结果
        List<StudentRanking> result = new ArrayList<>();
        int total = allStudentNumbers.size();
        int rank = 1;
        for (String studentNumber : allStudentNumbers) {
            Double score = totalScoreMap.get(studentNumber);

            Ranking ranking = new Ranking();
            ranking.setCourseId(0L);
            ranking.setScore(score);
            ranking.setRank(score != null ? rank++ : null);
            ranking.setTotal(total);
            ranking.setScope("年级");

            StudentRanking sr = new StudentRanking();
            sr.setStudentNumber(studentNumber);
            sr.setStudentName(studentNameMap.get(studentNumber));
            sr.setRanks(List.of(ranking));

            result.add(sr);
        }

        return ResponseResult.success("获取年级三科总分排名成功", result);
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
}
