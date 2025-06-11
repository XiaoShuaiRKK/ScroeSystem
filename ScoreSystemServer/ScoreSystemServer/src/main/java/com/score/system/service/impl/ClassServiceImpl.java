package com.score.system.service.impl;

import com.baomidou.mybatisplus.core.conditions.query.LambdaQueryWrapper;
import com.score.system.entity.ResponseResult;
import com.score.system.entity.school.ClassConverter;
import com.score.system.entity.school.ClassDTO;
import com.score.system.entity.school.ClassEntity;
import com.score.system.entity.user.Student;
import com.score.system.entity.user.StudentClassHistory;
import com.score.system.mapper.ClassMapper;
import com.score.system.mapper.StudentClassHistoryMapper;
import com.score.system.mapper.StudentMapper;
import com.score.system.mapper.TeacherMapper;
import com.score.system.service.ClassService;
import org.springframework.stereotype.Service;

import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;

@Service
public class ClassServiceImpl implements ClassService {
    private final ClassMapper classMapper;
    private final TeacherMapper teacherMapper;
    private final StudentMapper studentMapper;
    private final StudentClassHistoryMapper studentClassHistoryMapper;

    public ClassServiceImpl(ClassMapper classMapper, TeacherMapper teacherMapper, StudentMapper studentMapper, StudentClassHistoryMapper studentClassHistoryMapper) {
        this.classMapper = classMapper;
        this.teacherMapper = teacherMapper;
        this.studentMapper = studentMapper;
        this.studentClassHistoryMapper = studentClassHistoryMapper;
    }

    @Override
    public ResponseResult<Boolean> addClass(ClassDTO classDTO) {
        Long teacherId = classDTO.getHeadTeacherId();
        if(teacherMapper.selectById(teacherId) == null){
            return ResponseResult.fail("老师ID " + teacherId + "不存在");
        }
        if(classMapper.selectByClassName(classDTO.getName()) != null){
            return ResponseResult.fail("班级名称 " + classDTO.getName() + "已存在");
        }
        ClassEntity classEntity = ClassConverter.toEntity(classDTO);
        classEntity.setState(1);
        int result = classMapper.insert(classEntity);
        return result > 0
                ? ResponseResult.success("导入成功",true)
                : ResponseResult.fail("导入失败");
    }

    @Override
    public ResponseResult<Boolean> batchAddClass(List<ClassDTO> classDTOList) {
        List<ClassEntity> classEntities = new ArrayList<>();
        for (ClassDTO classDTO : classDTOList){
            if(teacherMapper.selectById(classDTO.getHeadTeacherId()) == null){
                return ResponseResult.fail("老师ID " + classDTO.getHeadTeacherId() + "不存在");
            }
            if(classMapper.selectByClassName(classDTO.getName()) != null){
                return ResponseResult.fail("班级名称 " + classDTO.getName() + "已存在");
            }
            ClassEntity entity = ClassConverter.toEntity(classDTO);
            entity.setState(1);
            classEntities.add(entity);
        }
        int result = classMapper.batchInsertClass(classEntities);
        return result > 0
                ? ResponseResult.success("导入成功",true)
                : ResponseResult.fail("导入失败");
    }

    @Override
    public ResponseResult<List<ClassDTO>> getAllClasses() {
        List<ClassEntity> classEntities = classMapper.selectList(
                new LambdaQueryWrapper<ClassEntity>()
                        .eq(ClassEntity::getState, 1)
        );
        if (classEntities == null) return ResponseResult.success(new ArrayList<>());
        List<ClassDTO> classDTOList = new ArrayList<>();
        for (ClassEntity classEntity : classEntities){
            String name = teacherMapper.selectById(classEntity.getHeadTeacherId()).getName();
            ClassDTO classDTO = ClassConverter.toDTO(classEntity,name);
            classDTOList.add(classDTO);
        }
        return ResponseResult.success(classDTOList);
    }

    @Override
    public ResponseResult<Boolean> updateClass(ClassDTO classDTO) {
        // 1. 校验班主任是否存在
        Long teacherId = classDTO.getHeadTeacherId();
        if (teacherId != null && teacherMapper.selectById(teacherId) == null) {
            return ResponseResult.fail("老师ID " + teacherId + " 不存在");
        }

        // 2. 校验班级是否存在
        ClassEntity existingClass = classMapper.selectById(classDTO.getId());
        if (existingClass == null) {
            return ResponseResult.fail("班级ID " + classDTO.getId() + " 不存在");
        }

        // 3. 校验班级名称是否重复（排除当前班级自身）
        ClassEntity sameNameClass = classMapper.selectByClassName(classDTO.getName());
        if (sameNameClass != null && !sameNameClass.getId().equals(classDTO.getId())) {
            return ResponseResult.fail("班级名称 " + classDTO.getName() + " 已存在");
        }

        // 4. 转换并更新
        ClassEntity updated = ClassConverter.toEntity(classDTO);
        int result = classMapper.updateById(updated);

        return result > 0
                ? ResponseResult.success("更新成功", true)
                : ResponseResult.fail("更新失败");
    }

    @Override
    public ResponseResult<Boolean> deleteClass(ClassDTO classDTO) {
        if(classDTO == null || classMapper.selectById(classDTO.getId()) == null){
            return ResponseResult.fail("未找到对应班级",false);
        }
        ClassEntity entity = ClassConverter.toEntity(classDTO);
        entity.setState(0);
        classMapper.updateById(entity);
        return ResponseResult.success("删除成功",true);
    }

    @Override
    public ResponseResult<Boolean> upGrade(int grade) {
        boolean isOver = false;
        // 1. 查询指定年级的所有班级
        List<ClassEntity> classList = classMapper.selectList(
                new LambdaQueryWrapper<ClassEntity>()
                        .eq(ClassEntity::getGrade, grade)
        );
        if (classList.isEmpty()) {
            return ResponseResult.fail("年级 " + grade + " 未找到任何班级");
        }
        // 2. 将这些班级的年级 +1（不超过6），必要时修改班级名称
        List<ClassEntity> toUpdate = new ArrayList<>();
        for (ClassEntity clazz : classList) {
            int originalGrade = clazz.getGrade();
            if (originalGrade < 6) {
                int newGrade = originalGrade + 1;
                clazz.setGrade(newGrade);

                // 如果是学期结束，修改班级名称：高一→高二，高二→高三
                String name = clazz.getName();
                if (originalGrade == 2 && name.startsWith("高一")) {
                    clazz.setName(name.replaceFirst("高一", "高二"));
                } else if (originalGrade == 4 && name.startsWith("高二")) {
                    clazz.setName(name.replaceFirst("高二", "高三"));
                }

                toUpdate.add(clazz);
            }else {
                if(!isOver){
                    isOver = true;
                }
                clazz.setState(2);
                clazz.setGrade(originalGrade + 1);
                toUpdate.add(clazz);
            }
        }

        // 批量更新班级
        if (!toUpdate.isEmpty()) {
            for (ClassEntity clazz : toUpdate) {
                classMapper.updateById(clazz);
            }
        }
        if(isOver){
            return ResponseResult.success("恭喜毕业",true);
        }
        // 3. 查询这些班级的所有学生
        List<Integer> classIds = classList.stream().map(ClassEntity::getId).toList();
        List<Student> students = studentMapper.selectList(
                new LambdaQueryWrapper<Student>()
                        .in(Student::getClassId, classIds)
        );
        StudentClassHistory classHistory = studentClassHistoryMapper.selectOne(
                new LambdaQueryWrapper<StudentClassHistory>()
                        .eq(StudentClassHistory::getGrade,grade)
                        .last("limit 1")
        );
        int currentYear;
        if(classHistory.getGrade() % 2 == 0){
            currentYear = classHistory.getYear();
        }else{
            currentYear = classHistory.getYear() + 1;
        }

        // 4. 为每个学生插入升学记录到 student_class_history 表
        List<StudentClassHistory> historyList = new ArrayList<>();
        for (Student student : students) {
            StudentClassHistory history = new StudentClassHistory();
            history.setStudentNumber(student.getStudentNumber());
            history.setClassId(student.getClassId().longValue());
            history.setGrade(Math.min(grade + 1, 6));
            history.setYear(currentYear);
            historyList.add(history);
        }

        if (!historyList.isEmpty()) {
            studentClassHistoryMapper.insertBatchSomeColumn(historyList);
        }

        return ResponseResult.success("年级升学成功", true);
    }
}
