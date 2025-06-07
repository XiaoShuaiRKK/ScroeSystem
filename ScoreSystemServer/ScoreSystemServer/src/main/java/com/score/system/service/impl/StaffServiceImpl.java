package com.score.system.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.baomidou.mybatisplus.core.conditions.query.QueryWrapper;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.ClassEntity;
import com.score.system.entity.user.*;
import com.score.system.mapper.*;
import com.score.system.service.StaffService;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.util.*;
import java.util.stream.Collectors;

@Service
public class StaffServiceImpl implements StaffService {
    private final TeacherMapper teacherMapper;
    private final StudentMapper studentMapper;
    private final ClassMapper classMapper;
    private final UserMapper userMapper;
    private final StudentSubjectSelectionMapper studentSubjectSelectionMapper;
    private final StudentClassHistoryMapper studentClassHistoryMapper;
    private final BCryptPasswordEncoder passwordEncoder = new BCryptPasswordEncoder();

    public StaffServiceImpl(TeacherMapper teacherMapper, StudentMapper studentMapper, ClassMapper classMapper, UserMapper userMapper, StudentSubjectSelectionMapper studentSubjectSelectionMapper, StudentClassHistoryMapper studentClassHistoryMapper) {
        this.teacherMapper = teacherMapper;
        this.studentMapper = studentMapper;
        this.classMapper = classMapper;
        this.userMapper = userMapper;
        this.studentSubjectSelectionMapper = studentSubjectSelectionMapper;
        this.studentClassHistoryMapper = studentClassHistoryMapper;
    }

    @Override
    @Transactional(rollbackFor = Exception.class)
    public ResponseResult<Boolean> addStudent(StudentDTO studentDTO) {
        if (classMapper.selectById(studentDTO.getClassId()) == null) {
            return ResponseResult.fail("班级不存在", false);
        }

        if (studentDTO.getEnrollmentDate() != null && studentDTO.getEnrollmentDate().isAfter(LocalDate.now())) {
            return ResponseResult.fail("入学时间不能晚于当前时间", false);
        }

        // 检查学号是否重复
        LambdaQueryWrapper<Student> queryWrapper = new LambdaQueryWrapper<>();
        queryWrapper.eq(Student::getStudentNumber, studentDTO.getStudentNumber());
        if (studentMapper.selectOne(queryWrapper) != null) {
            return ResponseResult.fail("学号已存在", false);
        }

        // 创建用户
        User user = new User();
        user.setName(studentDTO.getName());
        user.setUsername(studentDTO.getUserName());
        user.setPasswordHash(passwordEncoder.encode(studentDTO.getPassword()));
        user.setLevel(3); // 普通用户
        user.setRole(3);  // 学生

        LambdaQueryWrapper<User> userQuery = new LambdaQueryWrapper<>();
        userQuery.eq(User::getUsername, user.getUsername());
        if (userMapper.selectOne(userQuery) != null) {
            return ResponseResult.fail(user.getName() + " 用户名已经存在");
        }

        userMapper.insert(user);

        // 插入学生记录
        Student student = StudentConverter.toEntity(studentDTO, user.getId());
        studentMapper.insert(student);

        // 插入选科记录
        StudentSubjectSelection selection = new StudentSubjectSelection();
        selection.setStudentNumber(student.getStudentNumber());
        selection.setSubjectGroupId(studentDTO.getSubjectGroupId());
        selection.setElectiveCourse1Id(studentDTO.getElectiveCourse1Id());
        selection.setElectiveCourse2Id(studentDTO.getElectiveCourse2Id());
        studentSubjectSelectionMapper.insert(selection);

        // 获取班级信息以获取年级
        ClassEntity classEntity = classMapper.selectById(studentDTO.getClassId());

        // 插入学生历史班级记录
        StudentClassHistory history = new StudentClassHistory();
        history.setStudentNumber(student.getStudentNumber());
        history.setClassId(studentDTO.getClassId().longValue());
        history.setGrade(classEntity.getGrade());
        history.setYear(studentDTO.getYear()); // 注意：year 由前端传入
        studentClassHistoryMapper.insert(history);

        return ResponseResult.success("添加成功", true);
    }


    @Override
    @Transactional
    public ResponseResult<Boolean> batchAddStudent(List<StudentDTO> studentDTOList) {
        Set<String> studentNumbers = new HashSet<>();
        Set<String> usernames = new HashSet<>();
        Set<Integer> classIds = new HashSet<>();

        for (StudentDTO dto : studentDTOList) {
            if (!studentNumbers.add(dto.getStudentNumber())) {
                return ResponseResult.fail("学号重复: " + dto.getStudentNumber());
            }
            if (!usernames.add(dto.getUserName())) {
                return ResponseResult.fail("用户名重复: " + dto.getUserName());
            }
            if (dto.getEnrollmentDate() != null && dto.getEnrollmentDate().isAfter(LocalDate.now())) {
                return ResponseResult.fail("入学时间不能晚于当前时间: " + dto.getName());
            }
            classIds.add(dto.getClassId());
        }

        // 校验数据库中是否存在重复学号
        List<Student> existing = studentMapper.selectList(
                new LambdaQueryWrapper<Student>().in(Student::getStudentNumber, studentNumbers));
        if (!existing.isEmpty()) {
            return ResponseResult.fail("学号已存在: " + existing.get(0).getStudentNumber());
        }

        // 批量查询班级信息
        Map<Integer, ClassEntity> classMap = classMapper.selectBatchIds(classIds).stream()
                .collect(Collectors.toMap(ClassEntity::getId, c -> c));
        if (classMap.size() < classIds.size()) {
            return ResponseResult.fail("存在无效班级 ID");
        }

        // 准备数据
        List<User> users = new ArrayList<>();
        Map<String, String> passwordCache = new HashMap<>();

        for (StudentDTO dto : studentDTOList) {
            User user = new User();
            user.setName(dto.getName());
            user.setUsername(dto.getUserName());

            // 避免重复加密相同密码
            String encodedPwd = passwordCache.computeIfAbsent(dto.getPassword(), passwordEncoder::encode);
            user.setPasswordHash(encodedPwd);
            user.setLevel(3);
            user.setRole(3);
            users.add(user);
        }

        // 批量插入用户并获取 ID
        userMapper.batchInsert(users);

        List<Student> students = new ArrayList<>();
        List<StudentSubjectSelection> selections = new ArrayList<>();
        List<StudentClassHistory> histories = new ArrayList<>();

        for (int i = 0; i < studentDTOList.size(); i++) {
            StudentDTO dto = studentDTOList.get(i);
            User user = users.get(i);
            ClassEntity classEntity = classMap.get(Long.valueOf(dto.getClassId()));

            Student student = StudentConverter.toEntity(dto, user.getId());
            students.add(student);

            StudentSubjectSelection sel = new StudentSubjectSelection();
            sel.setStudentNumber(student.getStudentNumber());
            sel.setSubjectGroupId(dto.getSubjectGroupId());
            sel.setElectiveCourse1Id(dto.getElectiveCourse1Id());
            sel.setElectiveCourse2Id(dto.getElectiveCourse2Id());
            selections.add(sel);

            StudentClassHistory history = new StudentClassHistory();
            history.setStudentNumber(student.getStudentNumber());
            history.setClassId(dto.getClassId().longValue());
            history.setGrade(classEntity.getGrade());
            history.setYear(dto.getYear());
            histories.add(history);
        }

        // 批量插入学生信息
        studentMapper.insertBatchSomeColumn(students);
        studentSubjectSelectionMapper.insertBatchSomeColumn(selections);
        studentClassHistoryMapper.insertBatchSomeColumn(histories);

        return ResponseResult.success("批量添加成功", true);
    }

    @Override
    @Transactional
    public ResponseResult<Boolean> deleteStudent(StudentDTO studentDTO) {
        LambdaQueryWrapper<Student> queryWrapper = new LambdaQueryWrapper<Student>().eq(Student::getStudentNumber, studentDTO.getStudentNumber());
        Student student = studentMapper.selectOne(queryWrapper);
        if (student == null) {
            return ResponseResult.fail("学生编号: " + studentDTO.getStudentNumber() + " 不存在",false);
        }
        studentClassHistoryMapper.delete(
                new LambdaQueryWrapper<StudentClassHistory>().eq(StudentClassHistory::getStudentNumber, student.getStudentNumber())
        );
        studentSubjectSelectionMapper.delete(
                new LambdaQueryWrapper<StudentSubjectSelection>().eq(StudentSubjectSelection::getStudentNumber, student.getStudentNumber())
        );
        int studentDeletedResult = studentMapper.deleteById(student.getId());
        int userDeletedResult = userMapper.deleteById(student.getUserId());
        if(studentDeletedResult > 0 && userDeletedResult > 0) {
            return ResponseResult.success("删除成功",true);
        }else{
            throw new IllegalArgumentException("删除失败");
        }
    }

    @Override
    @Transactional
    public ResponseResult<Boolean> updateStudent(StudentDTO studentDTO) {
        // 1. 查询学生信息
        Student student = studentMapper.selectOne(
                new LambdaQueryWrapper<Student>().eq(Student::getStudentNumber, studentDTO.getStudentNumber())
        );
        if (student == null) {
            return ResponseResult.fail("该学生不存在，学号：" + studentDTO.getStudentNumber(), false);
        }

        // 2. 查询用户
        User user = userMapper.selectById(student.getUserId());
        if (user == null) {
            return ResponseResult.fail("关联用户不存在", false);
        }

        // 3. 校验用户名是否被其他人占用
        if (studentDTO.getUserName() != null && !studentDTO.getUserName().equals(user.getUsername())) {
            User exists = userMapper.selectOne(
                    new LambdaQueryWrapper<User>()
                            .eq(User::getUsername, studentDTO.getUserName())
                            .ne(User::getId, user.getId())
            );
            if (exists != null) {
                return ResponseResult.fail("用户名已存在: " + studentDTO.getUserName(), false);
            }
            user.setUsername(studentDTO.getUserName());
        }

        // 4. 校验入学时间
        if (studentDTO.getEnrollmentDate() != null &&
                studentDTO.getEnrollmentDate().isAfter(LocalDate.now())) {
            return ResponseResult.fail("入学时间不能晚于当前时间", false);
        }

        boolean studentUpdated = false;

        // 5. 修改学生信息（仅有变化时才修改）
        if (studentDTO.getEnrollmentDate() != null &&
                !studentDTO.getEnrollmentDate().equals(student.getEnrollmentDate())) {
            student.setEnrollmentDate(studentDTO.getEnrollmentDate());
            studentUpdated = true;
        }

        // 6. 如果 classId 变更了，新增一条 StudentClassHistory 记录
        if (studentDTO.getClassId() != null &&
                !studentDTO.getClassId().equals(student.getClassId())) {

            ClassEntity classEntity = classMapper.selectById(studentDTO.getClassId());
            if (classEntity == null) {
                return ResponseResult.fail("班级不存在", false);
            }

            student.setClassId(studentDTO.getClassId());
            studentUpdated = true;

            // 新增历史记录
            StudentClassHistory newHistory = new StudentClassHistory();
            newHistory.setStudentNumber(student.getStudentNumber());
            newHistory.setClassId(studentDTO.getClassId().longValue());
            newHistory.setGrade(classEntity.getGrade());
            newHistory.setYear(studentDTO.getYear());
            studentClassHistoryMapper.insert(newHistory);
        }

        // 7. 修改状态字段
        if (studentDTO.getState() != null &&
                !studentDTO.getState().equals(student.getState())) {
            student.setState(studentDTO.getState());
            studentUpdated = true;
        }

        if (studentUpdated) {
            studentMapper.updateById(student);
        }

        // 8. 修改用户信息
        boolean userUpdated = false;

        if (studentDTO.getName() != null && !studentDTO.getName().equals(user.getName())) {
            user.setName(studentDTO.getName());
            userUpdated = true;
        }

        if (studentDTO.getPassword() != null && !studentDTO.getPassword().isEmpty()) {
            user.setPasswordHash(passwordEncoder.encode(studentDTO.getPassword()));
            userUpdated = true;
        }

        if (userUpdated) {
            userMapper.updateById(user);
        }

        // 9. 修改选课信息（仅在有变动时）
        StudentSubjectSelection selection = studentSubjectSelectionMapper.selectOne(
                new LambdaQueryWrapper<StudentSubjectSelection>()
                        .eq(StudentSubjectSelection::getStudentNumber, student.getStudentNumber())
        );

        boolean selectionUpdated = false;
        if (selection != null) {
            if (!studentDTO.getSubjectGroupId().equals(selection.getSubjectGroupId())) {
                selection.setSubjectGroupId(studentDTO.getSubjectGroupId());
                selectionUpdated = true;
            }
            if (!studentDTO.getElectiveCourse1Id().equals(selection.getElectiveCourse1Id())) {
                selection.setElectiveCourse1Id(studentDTO.getElectiveCourse1Id());
                selectionUpdated = true;
            }
            if (!studentDTO.getElectiveCourse2Id().equals(selection.getElectiveCourse2Id())) {
                selection.setElectiveCourse2Id(studentDTO.getElectiveCourse2Id());
                selectionUpdated = true;
            }
            if (selectionUpdated) {
                studentSubjectSelectionMapper.updateById(selection);
            }
        }

        return ResponseResult.success("更新成功", true);
    }

    @Override
    public ResponseResult<Boolean> batchUpdateStudent(List<StudentDTO> studentDTOList) {
        if(studentDTOList == null || studentDTOList.isEmpty()){
            return ResponseResult.fail("学生列表不能为空",false);
        }
        int successCount = 0;
        List<String> failedDetails = new ArrayList<>();
        for(StudentDTO studentDTO : studentDTOList){
            try {
                ResponseResult<Boolean> result = this.updateStudent(studentDTO);
                if(result.getCode() == 200){
                    successCount++;
                }else {
                    failedDetails.add(studentDTO.getStudentNumber() + ": " + result.getMessage());
                }
            }catch (Exception e){
                failedDetails.add(studentDTO.getStudentNumber());
            }
        }
        int failedCount = failedDetails.size();
        StringBuilder msg = new StringBuilder();
        msg.append("批量更新完成：成功 ").append(successCount)
                .append(" 条，失败 ").append(failedCount).append(" 条。");

        if (failedCount > 0) {
            msg.append("\n失败明细：\n").append(String.join("\n", failedDetails));
            return ResponseResult.fail(msg.toString(), false);
        }

        return ResponseResult.success(msg.toString(), true);
    }

    @Override
    @Transactional(rollbackFor = Exception.class)
    public ResponseResult<Boolean> addTeacher(TeacherDTO teacherDTO) {
        User user = new User();
        user.setName(teacherDTO.getName());
        user.setUsername(teacherDTO.getUsername());
        user.setPasswordHash(passwordEncoder.encode(teacherDTO.getPassword()));
        user.setLevel(2);
        user.setRole(2);
        LambdaQueryWrapper<User> query = new LambdaQueryWrapper<>();
        query.eq(User::getUsername, user.getUsername());
        if(userMapper.selectOne(query) != null) return ResponseResult.fail(user.getName() + " 用户名已经存在");
        userMapper.insert(user);
        Teacher teacher = TeacherConverter.toEntity(teacherDTO, user.getId());
        LambdaQueryWrapper<Teacher> queryWrapper = new LambdaQueryWrapper<>();
        queryWrapper.eq(Teacher::getTeacherNumber, teacherDTO.getTeacherNumber());
        if(teacherMapper.selectOne(queryWrapper) != null) return ResponseResult.fail(teacher.getTeacherNumber() + " 教师编号已经存在");
        teacherMapper.insert(teacher);
        return ResponseResult.success("添加成功",true);
    }

    @Override
    @Transactional(rollbackFor = Exception.class)
    public ResponseResult<Boolean> batchAddTeacher(List<TeacherDTO> teacherDTOList) {
        for (TeacherDTO teacherDTO : teacherDTOList) {
            ResponseResult<Boolean> result = addTeacher(teacherDTO);
            if(result.getCode() != 200){
                throw new IllegalArgumentException("批量添加失败: " + result.getMessage());
            }
        }
        return ResponseResult.success("添加成功",true);

    }

    @Override
    public ResponseResult<List<Teacher>> getAllTeachers() {
        return ResponseResult.success(teacherMapper.selectList(null));
    }

    @Override
    @Transactional(rollbackFor = Exception.class)
    public ResponseResult<Boolean> deleteTeacher(TeacherDTO teacherDTO) {
        LambdaQueryWrapper<Teacher> queryWrapper = new LambdaQueryWrapper<Teacher>().eq(Teacher::getTeacherNumber, teacherDTO.getTeacherNumber());
        Teacher teacher = teacherMapper.selectOne(queryWrapper);
        if(teacher == null){
            return ResponseResult.fail("教师编号 " + teacherDTO.getTeacherNumber() + " 不存在", false);
        }
        //删除教师表记录
        int teacherDeletedResult = teacherMapper.deleteById(teacher.getId());
        //删除对应的用户信息
        int userDeletedResult = userMapper.deleteById(teacher.getUserId());
        if(teacherDeletedResult > 0 && userDeletedResult > 0){
            return ResponseResult.success("教师删除成功",true);
        }else{
            throw new IllegalArgumentException("删除失败");
        }
    }

    @Override
    @Transactional
    public ResponseResult<Boolean> updateTeacher(TeacherDTO teacherDTO) {
        // 1. 查找 Teacher 表记录
        Teacher teacher = teacherMapper.selectOne(
                new LambdaQueryWrapper<Teacher>()
                        .eq(Teacher::getTeacherNumber, teacherDTO.getTeacherNumber())
        );
        if (teacher == null) {
            return ResponseResult.fail("教师编号不存在: " + teacherDTO.getTeacherNumber(), false);
        }

        // 2. 查找对应的用户
        User user = userMapper.selectById(teacher.getUserId());
        if (user == null) {
            return ResponseResult.fail("关联用户不存在", false);
        }

        // 3. 校验用户名是否重复（排除自己）
        if (teacherDTO.getUsername() != null && !teacherDTO.getUsername().equals(user.getUsername())) {
            User exists = userMapper.selectOne(
                    new LambdaQueryWrapper<User>()
                            .eq(User::getUsername, teacherDTO.getUsername())
                            .ne(User::getId, user.getId())
            );
            if (exists != null) {
                return ResponseResult.fail("用户名已存在: " + teacherDTO.getUsername(), false);
            }
            user.setUsername(teacherDTO.getUsername());
        }

        // 4. 更新用户姓名和密码（如有）
        boolean userUpdated = false;

        if (teacherDTO.getUsername() != null && !teacherDTO.getUsername().equals(user.getName())) {
            user.setName(teacherDTO.getUsername());
            userUpdated = true;
        }

        if (teacherDTO.getPassword() != null && !teacherDTO.getPassword().isEmpty()) {
            user.setPasswordHash(passwordEncoder.encode(teacherDTO.getPassword()));
            userUpdated = true;
        }

        if (userUpdated) {
            userMapper.updateById(user);
        }

        // 5. 更新 Teacher 表中的状态或名字
        boolean teacherUpdated = false;

        if (teacherDTO.getName() != null && !teacherDTO.getName().equals(teacher.getName())) {
            teacher.setName(teacherDTO.getName());
            teacherUpdated = true;
        }

        if (teacherDTO.getState() != null && !teacherDTO.getState().equals(teacher.getState())) {
            teacher.setState(teacherDTO.getState());
            teacherUpdated = true;
        }

        if (teacherUpdated) {
            teacherMapper.updateById(teacher);
        }

        return ResponseResult.success("教师信息更新成功", true);
    }
}
