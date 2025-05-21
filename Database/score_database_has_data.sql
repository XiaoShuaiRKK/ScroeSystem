/*
 Navicat Premium Data Transfer

 Source Server         : local
 Source Server Type    : MySQL
 Source Server Version : 80012
 Source Host           : localhost:3306
 Source Schema         : score_database

 Target Server Type    : MySQL
 Target Server Version : 80012
 File Encoding         : 65001

 Date: 21/05/2025 14:42:39
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for classes
-- ----------------------------
DROP TABLE IF EXISTS `classes`;
CREATE TABLE `classes`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `grade` int(11) NOT NULL,
  `head_teacher_id` bigint(20) NOT NULL,
  `subject_group_id` bigint(20) NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `classes_teacher_id_fk`(`head_teacher_id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 2 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of classes
-- ----------------------------
INSERT INTO `classes` VALUES (1, '高一1班', 1, 1, 3, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (2, '高一2班', 1, 2, 2, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (3, '高一3班', 2, 3, 3, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (4, '高一4班', 2, 4, 2, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (5, '高二1班', 3, 5, 3, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (6, '高二2班', 3, 6, 2, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (7, '高二3班', 4, 7, 3, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (8, '高二4班', 4, 8, 2, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (9, '高三1班', 5, 9, 3, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (10, '高三2班', 5, 10, 2, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (11, '高三3班', 6, 11, 3, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (12, '高三4班', 6, 12, 2, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (13, '高一5班', 1, 13, 3, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (14, '高一6班', 1, 14, 2, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (15, '高二5班', 3, 15, 3, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (16, '高二6班', 3, 16, 2, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (17, '高三5班', 6, 17, 3, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (18, '高三6班', 6, 18, 2, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (19, '高一7班', 2, 19, 3, '2025-05-21 10:42:04', '2025-05-21 10:42:04');
INSERT INTO `classes` VALUES (20, '高一8班', 2, 20, 2, '2025-05-21 10:42:04', '2025-05-21 10:42:04');

-- ----------------------------
-- Table structure for courses
-- ----------------------------
DROP TABLE IF EXISTS `courses`;
CREATE TABLE `courses`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of courses
-- ----------------------------
INSERT INTO `courses` VALUES (1, '数学');
INSERT INTO `courses` VALUES (2, '语文');
INSERT INTO `courses` VALUES (3, '英语');
INSERT INTO `courses` VALUES (4, '物理');
INSERT INTO `courses` VALUES (5, '历史');
INSERT INTO `courses` VALUES (6, '化学');
INSERT INTO `courses` VALUES (7, '政治');
INSERT INTO `courses` VALUES (8, '地理');
INSERT INTO `courses` VALUES (9, '生物');

-- ----------------------------
-- Table structure for critical_config
-- ----------------------------
DROP TABLE IF EXISTS `critical_config`;
CREATE TABLE `critical_config`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `grade` int(11) NOT NULL,
  `year` int(11) NOT NULL,
  `university_level` int(11) NOT NULL,
  `target_count` int(11) NOT NULL,
  `critical_ratio` double NOT NULL,
  `subject_group_id` bigint(20) NOT NULL,
  `deleted` tinyint(4) NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Fixed;

-- ----------------------------
-- Records of critical_config
-- ----------------------------
INSERT INTO `critical_config` VALUES (1, 1, 2024, 2, 500, 0.15, 3, 0, '2025-05-21 13:22:01', '2025-05-21 13:22:01');
INSERT INTO `critical_config` VALUES (2, 2, 2024, 3, 300, 0.18, 2, 0, '2025-05-21 13:22:01', '2025-05-21 13:22:01');
INSERT INTO `critical_config` VALUES (3, 3, 2025, 1, 700, 0.12, 3, 0, '2025-05-21 13:22:01', '2025-05-21 13:22:01');
INSERT INTO `critical_config` VALUES (4, 4, 2025, 4, 200, 0.25, 2, 0, '2025-05-21 13:22:01', '2025-05-21 13:22:01');
INSERT INTO `critical_config` VALUES (5, 5, 2026, 2, 600, 0.1, 3, 0, '2025-05-21 13:22:01', '2025-05-21 13:22:01');
INSERT INTO `critical_config` VALUES (6, 6, 2026, 3, 400, 0.2, 2, 0, '2025-05-21 13:22:01', '2025-05-21 13:22:01');
INSERT INTO `critical_config` VALUES (7, 1, 2024, 1, 450, 0.14, 3, 0, '2025-05-21 13:22:01', '2025-05-21 13:22:01');
INSERT INTO `critical_config` VALUES (8, 2, 2024, 4, 150, 0.3, 2, 0, '2025-05-21 13:22:01', '2025-05-21 13:22:01');
INSERT INTO `critical_config` VALUES (9, 3, 2025, 2, 550, 0.16, 3, 0, '2025-05-21 13:22:01', '2025-05-21 13:22:01');
INSERT INTO `critical_config` VALUES (10, 4, 2025, 3, 350, 0.22, 2, 0, '2025-05-21 13:22:01', '2025-05-21 13:22:01');
INSERT INTO `critical_config` VALUES (11, 5, 2026, 1, 650, 0.11, 3, 0, '2025-05-21 13:22:01', '2025-05-21 13:22:01');
INSERT INTO `critical_config` VALUES (12, 6, 2026, 4, 250, 0.28, 2, 0, '2025-05-21 13:22:01', '2025-05-21 13:22:01');
INSERT INTO `critical_config` VALUES (13, 1, 2024, 3, 480, 0.17, 3, 0, '2025-05-21 13:22:01', '2025-05-21 13:22:01');
INSERT INTO `critical_config` VALUES (14, 2, 2024, 2, 320, 0.13, 2, 0, '2025-05-21 13:22:01', '2025-05-21 13:22:01');
INSERT INTO `critical_config` VALUES (15, 3, 2025, 4, 180, 0.27, 3, 0, '2025-05-21 13:22:01', '2025-05-21 13:22:01');
INSERT INTO `critical_config` VALUES (16, 4, 2025, 1, 720, 0.09, 2, 0, '2025-05-21 13:22:01', '2025-05-21 13:22:01');
INSERT INTO `critical_config` VALUES (17, 5, 2026, 3, 380, 0.21, 3, 0, '2025-05-21 13:22:01', '2025-05-21 13:22:01');
INSERT INTO `critical_config` VALUES (18, 6, 2026, 2, 680, 0.08, 2, 0, '2025-05-21 13:22:01', '2025-05-21 13:22:01');

-- ----------------------------
-- Table structure for critical_student_log
-- ----------------------------
DROP TABLE IF EXISTS `critical_student_log`;
CREATE TABLE `critical_student_log`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `exam_id` bigint(20) NOT NULL,
  `student_number` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `student_name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `university_level` int(11) NOT NULL,
  `score_rank` int(11) NOT NULL,
  `target_rank` int(11) NOT NULL,
  `gap` int(11) NOT NULL,
  `score` double NOT NULL,
  `subject_group_id` bigint(20) NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of critical_student_log
-- ----------------------------

-- ----------------------------
-- Table structure for evaluation
-- ----------------------------
DROP TABLE IF EXISTS `evaluation`;
CREATE TABLE `evaluation`  (
  `id` bigint(20) NOT NULL,
  `teacher_id` bigint(20) NOT NULL,
  `student_id` bigint(20) NOT NULL,
  `content` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `rating` int(11) NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `evaluation_teacher_id_fk`(`teacher_id`) USING BTREE,
  INDEX `evalution_student_id_fk`(`student_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of evaluation
-- ----------------------------

-- ----------------------------
-- Table structure for exam_subject_threshold
-- ----------------------------
DROP TABLE IF EXISTS `exam_subject_threshold`;
CREATE TABLE `exam_subject_threshold`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `exam_id` bigint(20) NOT NULL,
  `course_id` bigint(20) NOT NULL,
  `threshold_score` double NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of exam_subject_threshold
-- ----------------------------

-- ----------------------------
-- Table structure for exams
-- ----------------------------
DROP TABLE IF EXISTS `exams`;
CREATE TABLE `exams`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `start_date` date NOT NULL,
  `end_date` date NOT NULL,
  `grade` int(11) NOT NULL,
  `year` int(11) NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of exams
-- ----------------------------
INSERT INTO `exams` VALUES (1, '2024期中考试', '2025-05-22', '2025-05-25', 1, 2024, '2025-05-21 11:23:35', '2025-05-21 11:23:35');
INSERT INTO `exams` VALUES (2, '2025期末考试', '2025-05-23', '2025-05-25', 4, 2024, '2025-05-21 11:23:35', '2025-05-21 11:23:35');
INSERT INTO `exams` VALUES (3, '第一次考试', '2025-05-21', '2025-05-23', 1, 2024, '2025-05-21 11:58:07', '2025-05-21 11:58:07');

-- ----------------------------
-- Table structure for role
-- ----------------------------
DROP TABLE IF EXISTS `role`;
CREATE TABLE `role`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of role
-- ----------------------------
INSERT INTO `role` VALUES (1, '主任');
INSERT INTO `role` VALUES (2, '老师');
INSERT INTO `role` VALUES (3, '学生');
INSERT INTO `role` VALUES (4, '管理员');

-- ----------------------------
-- Table structure for scores
-- ----------------------------
DROP TABLE IF EXISTS `scores`;
CREATE TABLE `scores`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `student_number` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `course_id` bigint(20) NOT NULL,
  `exam_id` bigint(20) NOT NULL,
  `score` double NOT NULL,
  `comment` text CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 41 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of scores
-- ----------------------------
INSERT INTO `scores` VALUES (1, '1001', 6, 1, 85, '优秀', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (2, '1001', 9, 2, 91, '学科优势明显', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (3, '1001', 2, 1, 88, '阅读理解突出', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (4, '1001', 1, 2, 76, '计算需更细心', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (5, '1001', 3, 1, 92, '词汇量丰富', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (6, '1002', 9, 2, 78, '需加强练习', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (7, '1002', 7, 1, 82, '进步显著', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (8, '1002', 2, 2, 85, '作文结构清晰', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (9, '1002', 1, 1, 68, '公式运用生疏', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (10, '1002', 3, 2, 79, '听力部分薄弱', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (11, '1003', 6, 1, 92, '接近满分', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (12, '1003', 8, 2, 68, '基础需巩固', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (13, '1003', 2, 1, 78, '文言文需强化', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (14, '1003', 1, 2, 90, '几何思维强', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (15, '1003', 3, 1, 81, '语法错误较少', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (16, '1004', 9, 1, 74, '中等偏上', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (17, '1004', 7, 2, 81, '进步显著', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (18, '1004', 2, 1, 83, '表达流畅', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (19, '1004', 1, 2, 72, '应用题失分多', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (20, '1004', 3, 1, 89, '口语表现佳', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (21, '1005', 8, 1, 88, '表现稳定', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (22, '1005', 7, 2, 65, '需加强记忆', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (23, '1005', 2, 1, 69, '字迹潦草扣分', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (24, '1005', 1, 2, 94, '解题速度快', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (25, '1005', 3, 1, 77, '完形填空需练习', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (26, '1006', 6, 1, 79, '发挥正常', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (27, '1006', 9, 2, 93, '学科优势明显', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (28, '1006', 2, 1, 85, '古诗赏析优秀', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (29, '1006', 1, 2, 80, '代数运算准确', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (30, '1006', 3, 1, 73, '写作模板化', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (31, '1007', 7, 1, 70, '有待提高', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (32, '1007', 8, 2, 84, '理解深入', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (33, '1007', 2, 1, 90, '作文立意深刻', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (34, '1007', 1, 2, 62, '步骤不完整', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (35, '1007', 3, 1, 88, '阅读速度突出', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (36, '1008', 9, 1, 62, '需补基础', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (37, '1008', 6, 2, 77, '稳步提升', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (38, '1008', 2, 1, 75, '错别字较多', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (39, '1008', 1, 2, 83, '函数应用熟练', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (40, '1008', 3, 1, 71, '介词搭配混淆', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (41, '1009', 8, 1, 90, '优秀', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (42, '1009', 9, 2, 73, '实验部分薄弱', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (43, '1009', 2, 1, 82, '现代文分析佳', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (44, '1009', 1, 2, 78, '立体几何需加强', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (45, '1009', 3, 1, 95, '全卷无扣分', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (46, '1010', 7, 1, 85, '逻辑清晰', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (47, '1010', 6, 2, 69, '计算错误多', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (48, '1010', 2, 1, 72, '标点使用不当', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (49, '1010', 1, 2, 87, '概率题满分', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (50, '1010', 3, 1, 64, '拼写错误频繁', '2025-05-21 11:56:07', '2025-05-21 11:56:07');
INSERT INTO `scores` VALUES (51, '1001', 6, 3, 78, '基础扎实，但速度需提升', '2025-05-21 12:06:07', '2025-05-21 12:06:07');
INSERT INTO `scores` VALUES (52, '1001', 9, 3, 82, '实验操作熟练', '2025-05-21 12:06:07', '2025-05-21 12:06:07');
INSERT INTO `scores` VALUES (53, '1001', 2, 3, 80, '文言文翻译准确', '2025-05-21 12:06:07', '2025-05-21 12:06:07');
INSERT INTO `scores` VALUES (54, '1001', 1, 3, 75, '步骤分丢失较多', '2025-05-21 12:06:07', '2025-05-21 12:06:07');
INSERT INTO `scores` VALUES (55, '1001', 3, 3, 85, '听力满分', '2025-05-21 12:06:07', '2025-05-21 12:06:07');
INSERT INTO `scores` VALUES (56, '1007', 7, 3, 65, '时政热点不熟', '2025-05-21 12:06:07', '2025-05-21 12:06:07');
INSERT INTO `scores` VALUES (57, '1007', 8, 3, 72, '地图判读生疏', '2025-05-21 12:06:07', '2025-05-21 12:06:07');
INSERT INTO `scores` VALUES (58, '1007', 2, 3, 68, '作文偏题', '2025-05-21 12:06:07', '2025-05-21 12:06:07');
INSERT INTO `scores` VALUES (59, '1007', 1, 3, 58, '公式记忆错误', '2025-05-21 12:06:07', '2025-05-21 12:06:07');
INSERT INTO `scores` VALUES (60, '1007', 3, 3, 77, '阅读速度一般', '2025-05-21 12:06:07', '2025-05-21 12:06:07');
INSERT INTO `scores` VALUES (61, '1013', 8, 3, 85, '区域分析准确', '2025-05-21 12:06:07', '2025-05-21 12:06:07');
INSERT INTO `scores` VALUES (62, '1013', 6, 3, 70, '实验细节遗漏', '2025-05-21 12:06:07', '2025-05-21 12:06:07');
INSERT INTO `scores` VALUES (63, '1013', 2, 3, 82, '现代文赏析佳', '2025-05-21 12:06:07', '2025-05-21 12:06:07');
INSERT INTO `scores` VALUES (64, '1013', 1, 3, 90, '几何证明严谨', '2025-05-21 12:06:07', '2025-05-21 12:06:07');
INSERT INTO `scores` VALUES (65, '1013', 3, 3, 73, '语法错误较多', '2025-05-21 12:06:07', '2025-05-21 12:06:07');

-- ----------------------------
-- Table structure for student
-- ----------------------------
DROP TABLE IF EXISTS `student`;
CREATE TABLE `student`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `user_id` bigint(20) NOT NULL,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `student_number` bigint(20) NOT NULL,
  `class_id` int(11) NOT NULL,
  `enrollment_date` date NOT NULL,
  `state` bigint(20) NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = MyISAM AUTO_INCREMENT = 34 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = FIXED;

-- ----------------------------
-- Records of student
-- ----------------------------
INSERT INTO `student` VALUES (1, 28, '张伟', 1001, 1, '2023-09-01', 1, '2025-05-21 11:14:08', '2025-05-21 11:14:08');
INSERT INTO `student` VALUES (2, 29, '王芳', 1002, 7, '2022-09-01', 1, '2025-05-21 11:14:08', '2025-05-21 11:14:08');
INSERT INTO `student` VALUES (3, 30, '李强', 1003, 10, '2023-09-01', 1, '2025-05-21 11:14:08', '2025-05-21 11:14:08');
INSERT INTO `student` VALUES (4, 31, '陈静', 1004, 4, '2023-09-01', 1, '2025-05-21 11:14:09', '2025-05-21 11:14:09');
INSERT INTO `student` VALUES (5, 32, '刘洋', 1005, 5, '2023-09-01', 1, '2025-05-21 11:14:09', '2025-05-21 11:14:09');
INSERT INTO `student` VALUES (6, 33, '赵敏', 1006, 17, '2022-09-01', 1, '2025-05-21 11:14:09', '2025-05-21 11:14:09');
INSERT INTO `student` VALUES (7, 34, '周杰', 1007, 14, '2023-09-01', 1, '2025-05-21 11:14:09', '2025-05-21 11:14:09');
INSERT INTO `student` VALUES (8, 35, '徐丽', 1008, 6, '2023-09-01', 1, '2025-05-21 11:14:10', '2025-05-21 11:14:10');
INSERT INTO `student` VALUES (9, 36, '孙浩', 1009, 12, '2023-09-01', 1, '2025-05-21 11:14:10', '2025-05-21 11:14:10');
INSERT INTO `student` VALUES (10, 37, '马琳', 1010, 3, '2023-09-01', 1, '2025-05-21 11:14:10', '2025-05-21 11:14:10');
INSERT INTO `student` VALUES (11, 38, '吴磊', 1011, 15, '2023-09-01', 1, '2025-05-21 11:14:10', '2025-05-21 11:14:10');
INSERT INTO `student` VALUES (12, 39, '黄婷', 1012, 9, '2022-09-01', 1, '2025-05-21 11:14:11', '2025-05-21 11:14:11');
INSERT INTO `student` VALUES (13, 40, '郭宇', 1013, 13, '2023-09-01', 1, '2025-05-21 11:14:11', '2025-05-21 11:14:11');
INSERT INTO `student` VALUES (14, 41, '何娜', 1014, 8, '2023-09-01', 1, '2025-05-21 11:14:11', '2025-05-21 11:14:11');
INSERT INTO `student` VALUES (15, 42, '林涛', 1015, 18, '2023-09-01', 1, '2025-05-21 11:14:12', '2025-05-21 11:14:12');
INSERT INTO `student` VALUES (16, 43, '高雪', 1016, 19, '2023-09-01', 1, '2025-05-21 11:14:12', '2025-05-21 11:14:12');
INSERT INTO `student` VALUES (17, 44, '程飞', 1017, 16, '2023-09-01', 1, '2025-05-21 11:14:12', '2025-05-21 11:14:12');
INSERT INTO `student` VALUES (18, 45, '袁敏', 1018, 11, '2022-09-01', 1, '2025-05-21 11:14:12', '2025-05-21 11:14:12');
INSERT INTO `student` VALUES (19, 46, '邓超', 1019, 20, '2023-09-01', 1, '2025-05-21 11:14:13', '2025-05-21 11:14:13');
INSERT INTO `student` VALUES (20, 47, '蔡欣', 1020, 8, '2023-09-01', 1, '2025-05-21 11:14:13', '2025-05-21 11:14:13');

-- ----------------------------
-- Table structure for student_class_history
-- ----------------------------
DROP TABLE IF EXISTS `student_class_history`;
CREATE TABLE `student_class_history`  (
  `student_number` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `class_id` bigint(20) NOT NULL,
  `grade` int(11) NOT NULL,
  `year` int(11) NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL
) ENGINE = MyISAM AUTO_INCREMENT = 1 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of student_class_history
-- ----------------------------
INSERT INTO `student_class_history` VALUES ('1001', 1, 1, 2024, '2025-05-21 11:14:08', '2025-05-21 11:14:08');
INSERT INTO `student_class_history` VALUES ('1002', 7, 4, 2024, '2025-05-21 11:14:08', '2025-05-21 11:14:08');
INSERT INTO `student_class_history` VALUES ('1003', 10, 5, 2024, '2025-05-21 11:14:08', '2025-05-21 11:14:08');
INSERT INTO `student_class_history` VALUES ('1004', 4, 2, 2024, '2025-05-21 11:14:09', '2025-05-21 11:14:09');
INSERT INTO `student_class_history` VALUES ('1005', 5, 3, 2024, '2025-05-21 11:14:09', '2025-05-21 11:14:09');
INSERT INTO `student_class_history` VALUES ('1006', 17, 6, 2024, '2025-05-21 11:14:09', '2025-05-21 11:14:09');
INSERT INTO `student_class_history` VALUES ('1007', 14, 1, 2024, '2025-05-21 11:14:09', '2025-05-21 11:14:09');
INSERT INTO `student_class_history` VALUES ('1008', 6, 3, 2024, '2025-05-21 11:14:10', '2025-05-21 11:14:10');
INSERT INTO `student_class_history` VALUES ('1009', 12, 6, 2024, '2025-05-21 11:14:10', '2025-05-21 11:14:10');
INSERT INTO `student_class_history` VALUES ('1010', 3, 2, 2024, '2025-05-21 11:14:10', '2025-05-21 11:14:10');
INSERT INTO `student_class_history` VALUES ('1011', 15, 3, 2024, '2025-05-21 11:14:10', '2025-05-21 11:14:10');
INSERT INTO `student_class_history` VALUES ('1012', 9, 5, 2024, '2025-05-21 11:14:11', '2025-05-21 11:14:11');
INSERT INTO `student_class_history` VALUES ('1013', 13, 1, 2024, '2025-05-21 11:14:11', '2025-05-21 11:14:11');
INSERT INTO `student_class_history` VALUES ('1014', 8, 4, 2024, '2025-05-21 11:14:11', '2025-05-21 11:14:11');
INSERT INTO `student_class_history` VALUES ('1015', 18, 6, 2024, '2025-05-21 11:14:12', '2025-05-21 11:14:12');
INSERT INTO `student_class_history` VALUES ('1016', 19, 2, 2024, '2025-05-21 11:14:12', '2025-05-21 11:14:12');
INSERT INTO `student_class_history` VALUES ('1017', 16, 3, 2024, '2025-05-21 11:14:12', '2025-05-21 11:14:12');
INSERT INTO `student_class_history` VALUES ('1018', 11, 6, 2024, '2025-05-21 11:14:13', '2025-05-21 11:14:13');
INSERT INTO `student_class_history` VALUES ('1019', 20, 2, 2024, '2025-05-21 11:14:13', '2025-05-21 11:14:13');
INSERT INTO `student_class_history` VALUES ('1020', 8, 4, 2024, '2025-05-21 11:14:13', '2025-05-21 11:14:13');

-- ----------------------------
-- Table structure for student_state
-- ----------------------------
DROP TABLE IF EXISTS `student_state`;
CREATE TABLE `student_state`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of student_state
-- ----------------------------
INSERT INTO `student_state` VALUES (1, '正常');
INSERT INTO `student_state` VALUES (2, '已毕业');
INSERT INTO `student_state` VALUES (3, '劝退');
INSERT INTO `student_state` VALUES (4, '休学');

-- ----------------------------
-- Table structure for student_subject_selection
-- ----------------------------
DROP TABLE IF EXISTS `student_subject_selection`;
CREATE TABLE `student_subject_selection`  (
  `student_number` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `subject_group_id` bigint(20) NOT NULL,
  `elective_course1_id` bigint(20) NOT NULL,
  `elective_course2_id` bigint(20) NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`student_number`) USING BTREE,
  INDEX `student_subject_elective_course1_id_fk`(`elective_course1_id`) USING BTREE,
  INDEX `student_subject_elective_coures2_fk`(`elective_course2_id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of student_subject_selection
-- ----------------------------
INSERT INTO `student_subject_selection` VALUES ('1001', 3, 5, 8, '2025-05-21 11:14:08', '2025-05-21 11:14:08');
INSERT INTO `student_subject_selection` VALUES ('1002', 2, 6, 7, '2025-05-21 11:14:08', '2025-05-21 11:14:08');
INSERT INTO `student_subject_selection` VALUES ('1003', 3, 5, 7, '2025-05-21 11:14:08', '2025-05-21 11:14:08');
INSERT INTO `student_subject_selection` VALUES ('1004', 2, 8, 6, '2025-05-21 11:14:09', '2025-05-21 11:14:09');
INSERT INTO `student_subject_selection` VALUES ('1005', 3, 7, 6, '2025-05-21 11:14:09', '2025-05-21 11:14:09');
INSERT INTO `student_subject_selection` VALUES ('1006', 2, 5, 8, '2025-05-21 11:14:09', '2025-05-21 11:14:09');
INSERT INTO `student_subject_selection` VALUES ('1007', 3, 6, 7, '2025-05-21 11:14:09', '2025-05-21 11:14:09');
INSERT INTO `student_subject_selection` VALUES ('1008', 2, 8, 5, '2025-05-21 11:14:10', '2025-05-21 11:14:10');
INSERT INTO `student_subject_selection` VALUES ('1009', 3, 7, 8, '2025-05-21 11:14:10', '2025-05-21 11:14:10');
INSERT INTO `student_subject_selection` VALUES ('1010', 2, 6, 5, '2025-05-21 11:14:10', '2025-05-21 11:14:10');
INSERT INTO `student_subject_selection` VALUES ('1011', 3, 8, 7, '2025-05-21 11:14:10', '2025-05-21 11:14:10');
INSERT INTO `student_subject_selection` VALUES ('1012', 2, 5, 6, '2025-05-21 11:14:11', '2025-05-21 11:14:11');
INSERT INTO `student_subject_selection` VALUES ('1013', 3, 7, 5, '2025-05-21 11:14:11', '2025-05-21 11:14:11');
INSERT INTO `student_subject_selection` VALUES ('1014', 2, 8, 7, '2025-05-21 11:14:11', '2025-05-21 11:14:11');
INSERT INTO `student_subject_selection` VALUES ('1015', 3, 6, 8, '2025-05-21 11:14:12', '2025-05-21 11:14:12');
INSERT INTO `student_subject_selection` VALUES ('1016', 2, 7, 5, '2025-05-21 11:14:12', '2025-05-21 11:14:12');
INSERT INTO `student_subject_selection` VALUES ('1017', 3, 8, 6, '2025-05-21 11:14:12', '2025-05-21 11:14:12');
INSERT INTO `student_subject_selection` VALUES ('1018', 2, 5, 7, '2025-05-21 11:14:13', '2025-05-21 11:14:13');
INSERT INTO `student_subject_selection` VALUES ('1019', 3, 6, 8, '2025-05-21 11:14:13', '2025-05-21 11:14:13');
INSERT INTO `student_subject_selection` VALUES ('1020', 2, 7, 5, '2025-05-21 11:14:13', '2025-05-21 11:14:13');

-- ----------------------------
-- Table structure for subject_group
-- ----------------------------
DROP TABLE IF EXISTS `subject_group`;
CREATE TABLE `subject_group`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of subject_group
-- ----------------------------
INSERT INTO `subject_group` VALUES (1, '未分科');
INSERT INTO `subject_group` VALUES (2, '文综');
INSERT INTO `subject_group` VALUES (3, '理综');

-- ----------------------------
-- Table structure for teacher
-- ----------------------------
DROP TABLE IF EXISTS `teacher`;
CREATE TABLE `teacher`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `user_id` bigint(20) NOT NULL,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `state` bigint(20) NOT NULL,
  `teacher_number` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `teacher_user_id_fk`(`user_id`) USING BTREE,
  INDEX `teacher_state_id_fk`(`state`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 3 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of teacher
-- ----------------------------
INSERT INTO `teacher` VALUES (1, 5, '张伟', 1, 'T1001', '2025-05-21 10:32:14', '2025-05-21 10:32:14');
INSERT INTO `teacher` VALUES (2, 6, '王芳', 1, 'T1002', '2025-05-21 10:32:14', '2025-05-21 10:32:14');
INSERT INTO `teacher` VALUES (3, 7, '李强', 1, 'T1003', '2025-05-21 10:32:15', '2025-05-21 10:32:15');
INSERT INTO `teacher` VALUES (4, 8, '陈静', 1, 'T1004', '2025-05-21 10:32:15', '2025-05-21 10:32:15');
INSERT INTO `teacher` VALUES (5, 9, '刘洋', 1, 'T1005', '2025-05-21 10:32:15', '2025-05-21 10:32:15');
INSERT INTO `teacher` VALUES (6, 10, '赵敏', 1, 'T1006', '2025-05-21 10:32:15', '2025-05-21 10:32:15');
INSERT INTO `teacher` VALUES (7, 11, '周杰', 1, 'T1007', '2025-05-21 10:32:15', '2025-05-21 10:32:15');
INSERT INTO `teacher` VALUES (8, 12, '徐丽', 1, 'T1008', '2025-05-21 10:32:16', '2025-05-21 10:32:16');
INSERT INTO `teacher` VALUES (9, 13, '孙浩', 1, 'T1009', '2025-05-21 10:32:16', '2025-05-21 10:32:16');
INSERT INTO `teacher` VALUES (10, 14, '马琳', 1, 'T1010', '2025-05-21 10:32:16', '2025-05-21 10:32:16');
INSERT INTO `teacher` VALUES (11, 15, '吴磊', 1, 'T1011', '2025-05-21 10:32:16', '2025-05-21 10:32:16');
INSERT INTO `teacher` VALUES (12, 16, '黄婷', 1, 'T1012', '2025-05-21 10:32:16', '2025-05-21 10:32:16');
INSERT INTO `teacher` VALUES (13, 17, '郭宇', 1, 'T1013', '2025-05-21 10:32:17', '2025-05-21 10:32:17');
INSERT INTO `teacher` VALUES (14, 18, '何娜', 1, 'T1014', '2025-05-21 10:32:17', '2025-05-21 10:32:17');
INSERT INTO `teacher` VALUES (15, 19, '林涛', 1, 'T1015', '2025-05-21 10:32:17', '2025-05-21 10:32:17');
INSERT INTO `teacher` VALUES (16, 20, '高雪', 1, 'T1016', '2025-05-21 10:32:17', '2025-05-21 10:32:17');
INSERT INTO `teacher` VALUES (17, 21, '程飞', 1, 'T1017', '2025-05-21 10:32:17', '2025-05-21 10:32:17');
INSERT INTO `teacher` VALUES (18, 22, '袁敏', 1, 'T1018', '2025-05-21 10:32:18', '2025-05-21 10:32:18');
INSERT INTO `teacher` VALUES (19, 23, '邓超', 1, 'T1019', '2025-05-21 10:32:18', '2025-05-21 10:32:18');
INSERT INTO `teacher` VALUES (20, 24, '蔡欣', 1, 'T1020', '2025-05-21 10:32:18', '2025-05-21 10:32:18');

-- ----------------------------
-- Table structure for teacher_course
-- ----------------------------
DROP TABLE IF EXISTS `teacher_course`;
CREATE TABLE `teacher_course`  (
  `teacher_id` bigint(20) NOT NULL,
  `course_id` bigint(20) NOT NULL,
  `class_id` bigint(20) NOT NULL
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of teacher_course
-- ----------------------------

-- ----------------------------
-- Table structure for teacher_state
-- ----------------------------
DROP TABLE IF EXISTS `teacher_state`;
CREATE TABLE `teacher_state`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of teacher_state
-- ----------------------------
INSERT INTO `teacher_state` VALUES (1, '正常');
INSERT INTO `teacher_state` VALUES (2, '辞退');
INSERT INTO `teacher_state` VALUES (3, '休假');
INSERT INTO `teacher_state` VALUES (4, '权限限制');

-- ----------------------------
-- Table structure for university
-- ----------------------------
DROP TABLE IF EXISTS `university`;
CREATE TABLE `university`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `university_level` bigint(20) NOT NULL,
  `science_score_line` double NOT NULL,
  `art_score_line` double NOT NULL,
  `year` int(11) NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `university_level_id_fk`(`university_level`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of university
-- ----------------------------
INSERT INTO `university` VALUES (1, '清华大学', 1, 680, 670, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (2, '北京大学', 2, 675, 672, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (3, '复旦大学', 2, 660, 655, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (4, '浙江大学', 1, 658, 650, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (5, '南京大学', 2, 650, 645, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (6, '上海交通大学', 1, 670, 665, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (7, '中国科学技术大学', 1, 665, 640, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (8, '武汉大学', 2, 640, 635, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (9, '中山大学', 3, 630, 625, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (10, '四川大学', 2, 635, 628, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (11, '南开大学', 2, 645, 638, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (12, '同济大学', 3, 625, 620, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (13, '华中科技大学', 2, 642, 636, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (14, '西安交通大学', 1, 655, 648, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (15, '哈尔滨工业大学', 2, 648, 640, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (16, '中国人民大学', 2, 660, 655, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (17, '北京师范大学', 3, 628, 625, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (18, '厦门大学', 2, 638, 630, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (19, '东南大学', 2, 635, 627, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');
INSERT INTO `university` VALUES (20, '天津大学', 1, 640, 632, 2024, '2025-05-21 09:38:55', '2025-05-21 09:38:55');

-- ----------------------------
-- Table structure for university_level
-- ----------------------------
DROP TABLE IF EXISTS `university_level`;
CREATE TABLE `university_level`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of university_level
-- ----------------------------
INSERT INTO `university_level` VALUES (1, '九八五');
INSERT INTO `university_level` VALUES (2, '双一流');
INSERT INTO `university_level` VALUES (3, '优投');
INSERT INTO `university_level` VALUES (4, '本科');

-- ----------------------------
-- Table structure for user
-- ----------------------------
DROP TABLE IF EXISTS `user`;
CREATE TABLE `user`  (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `username` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `password_hash` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `level` int(11) NOT NULL,
  `role` int(11) NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `user_level_fk`(`level`) USING BTREE,
  INDEX `user_role_fk`(`role`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 42 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of user
-- ----------------------------
INSERT INTO `user` VALUES (1, '小衰', 'xiaoshuai', '$2a$10$WfbpZajJhsiiSe18mEFr6uwQ2tnaippiUpQjWuViYG6/KFFQgi3Ky', 1, 4, '2025-05-21 08:48:45', '2025-05-21 08:48:45');
INSERT INTO `user` VALUES (2, '胡旻', 'humin', '$2a$10$t034U/VbBMzWCvOpHjGCZOmj30Ci9fxRRp66bE/DXSnSiWOpu2ICO', 1, 4, '2025-05-21 08:53:07', '2025-05-21 08:53:07');
INSERT INTO `user` VALUES (3, '王老师', 'wanglaoshi', '$2a$10$0ObtQv6OOtVxwOWhX9G5wOVIJkADVKSnfbLRKEzg4S8RfyGvP5apa', 2, 2, '2025-05-21 10:10:34', '2025-05-21 10:10:34');
INSERT INTO `user` VALUES (4, '赵老师', 'zhaojiaoshi', '$2a$10$62UVB0Jn1AXzJjwHG0vclODqmSPzuzvv43tAgydHFWniloN3x07Ei', 2, 2, '2025-05-21 10:10:34', '2025-05-21 10:10:34');
INSERT INTO `user` VALUES (5, '张伟', 'zhangwei1', '$2a$10$ueS.a1uIvwpuGZijfQciE.YRGLcYVUGP04epfvH2O3odxEtX2D3XG', 2, 2, '2025-05-21 10:32:14', '2025-05-21 10:32:14');
INSERT INTO `user` VALUES (6, '王芳', 'wangfang2', '$2a$10$37RdQ57KFzvE.dfdZ1K1h.hqinkDaV0uWaqh1BHEKKWKPdHc0NQZC', 2, 2, '2025-05-21 10:32:14', '2025-05-21 10:32:14');
INSERT INTO `user` VALUES (7, '李强', 'liqiang3', '$2a$10$YorXN.xs.wmVmv6fQmZBC.uIdU6V.8SE3XQXjbw8XWQTWm0xKEmCO', 2, 2, '2025-05-21 10:32:14', '2025-05-21 10:32:14');
INSERT INTO `user` VALUES (8, '陈静', 'chenjing4', '$2a$10$uYZ4V/lV2mhPgtuR8pRWY.092sbkZR.16edWUdfFQWJhanuraXOEy', 2, 2, '2025-05-21 10:32:15', '2025-05-21 10:32:15');
INSERT INTO `user` VALUES (9, '刘洋', 'liuyang5', '$2a$10$w9pNchCQgk//EZoUdHHY1ObG.8V1VkypkZceQ9fXH7KykXKyQmHwC', 2, 2, '2025-05-21 10:32:15', '2025-05-21 10:32:15');
INSERT INTO `user` VALUES (10, '赵敏', 'zhaomin6', '$2a$10$F4wsnP7wBfnjbguT6wQOXOKYCxm9F7fePM2aPdBWur.BGR21eAmsy', 2, 2, '2025-05-21 10:32:15', '2025-05-21 10:32:15');
INSERT INTO `user` VALUES (11, '周杰', 'zhoujie7', '$2a$10$vDrW2Ir8n7y91plBY1qhQerYJnpCAIyUnHjjAMplosf/unTFgAOIS', 2, 2, '2025-05-21 10:32:15', '2025-05-21 10:32:15');
INSERT INTO `user` VALUES (12, '徐丽', 'xuli8', '$2a$10$sUuRR4afTHCNfPJ45rK22eGeXQerYmCp/cEfN2bL44NlhRpRUcyAO', 2, 2, '2025-05-21 10:32:16', '2025-05-21 10:32:16');
INSERT INTO `user` VALUES (13, '孙浩', 'sunhao9', '$2a$10$dH52nRig0otSauO05gUv6etfr3iZRGLwVsVCAs4R6eBW3YbgMFQqG', 2, 2, '2025-05-21 10:32:16', '2025-05-21 10:32:16');
INSERT INTO `user` VALUES (14, '马琳', 'malin10', '$2a$10$C/hI9TexSluUaoT7idHmMuczJElYCegmMUCCdpz3wb7Y792uu6TRa', 2, 2, '2025-05-21 10:32:16', '2025-05-21 10:32:16');
INSERT INTO `user` VALUES (15, '吴磊', 'wulei11', '$2a$10$ntVqB0x.3cgkcCryj3bhWu2WJqqRht6gWFULKVGrwNzfxykxwBL3i', 2, 2, '2025-05-21 10:32:16', '2025-05-21 10:32:16');
INSERT INTO `user` VALUES (16, '黄婷', 'huangting12', '$2a$10$Lvw8OuM12xCRnxksZH2NsOXhiCUS2IQRQ2klhtU097yYpdpobxzke', 2, 2, '2025-05-21 10:32:16', '2025-05-21 10:32:16');
INSERT INTO `user` VALUES (17, '郭宇', 'guoyu13', '$2a$10$R9NIvqk18f0R3wo0V5KlM.oLafKU4cPtVC7hM0xtD8PQ3wEMNJO9a', 2, 2, '2025-05-21 10:32:17', '2025-05-21 10:32:17');
INSERT INTO `user` VALUES (18, '何娜', 'hena14', '$2a$10$u2dMg8JLCysJvXORnslIee1l52JgslKCVtGWxjDTsoVuPapeftn86', 2, 2, '2025-05-21 10:32:17', '2025-05-21 10:32:17');
INSERT INTO `user` VALUES (19, '林涛', 'lintao15', '$2a$10$8qxXNWtEdWS40jK/OOxAUeLE3uc72eZ.ro3E9bFLdVmC7eGlsO5ae', 2, 2, '2025-05-21 10:32:17', '2025-05-21 10:32:17');
INSERT INTO `user` VALUES (20, '高雪', 'gaoxue16', '$2a$10$H9GynUeFwA6riwy7vyTrY.2.XMY3IilayTBO39i2Q5Nz3GOnxVmfu', 2, 2, '2025-05-21 10:32:17', '2025-05-21 10:32:17');
INSERT INTO `user` VALUES (21, '程飞', 'chengfei17', '$2a$10$JxyArWBPebM0d2EaA8hvu.iaaPJThY62DI4SiWgHauyZoDtbN9xYW', 2, 2, '2025-05-21 10:32:17', '2025-05-21 10:32:17');
INSERT INTO `user` VALUES (22, '袁敏', 'yuanmin18', '$2a$10$eOqnEg/52BnPWXSR..MfhulIXSWPaY1czztDLm98FDN0n1/duMwJy', 2, 2, '2025-05-21 10:32:18', '2025-05-21 10:32:18');
INSERT INTO `user` VALUES (23, '邓超', 'dengchao19', '$2a$10$dkUqK1yvXRrV9sJIrhj1AOXjWcycT7vfRzIfHfchObdG9PWadJZjy', 2, 2, '2025-05-21 10:32:18', '2025-05-21 10:32:18');
INSERT INTO `user` VALUES (24, '蔡欣', 'caixin20', '$2a$10$Xu8BR3uhnDpc6IUHiCadNO8O8BmoNElhIko5B.Gt/UDdeZ7IPKJzq', 2, 2, '2025-05-21 10:32:18', '2025-05-21 10:32:18');
INSERT INTO `user` VALUES (28, '张伟', 'zhangwei1001', '$2a$10$LvNiGixmv5ZaHA8f1t0fk.nq0GVBYjgtQlLxqj1iUY3aRoO5dGH8.', 3, 3, '2025-05-21 11:14:08', '2025-05-21 11:14:08');
INSERT INTO `user` VALUES (29, '王芳', 'wangfang1002', '$2a$10$OAUigVIbQJOdRGZrpVA26.9Xv6sj3JG1EYg/.jzuzRyGyzHcF5DAO', 3, 3, '2025-05-21 11:14:08', '2025-05-21 11:14:08');
INSERT INTO `user` VALUES (30, '李强', 'liqiang1003', '$2a$10$WiQaES13xm1xil4KSLu2QusjfADGjV2mFdYfgZ7C/pjKtLLdWwmSq', 3, 3, '2025-05-21 11:14:08', '2025-05-21 11:14:08');
INSERT INTO `user` VALUES (31, '陈静', 'chenjing1004', '$2a$10$w05zf3c28t65USA6AqDbJu1k1A3UJwBfIhed/ku6oLL70fXYAuOQW', 3, 3, '2025-05-21 11:14:09', '2025-05-21 11:14:09');
INSERT INTO `user` VALUES (32, '刘洋', 'liuyang1005', '$2a$10$XmGmZdjRxF6E7m1fuGwIiO7lGJjVgsfwvu.e//g1Sc7qsFh1T.cTO', 3, 3, '2025-05-21 11:14:09', '2025-05-21 11:14:09');
INSERT INTO `user` VALUES (33, '赵敏', 'zhaomin1006', '$2a$10$NDGVGq8tzRi/ks5iKhKgh.zmwmBnBKSGbYLkG8QHOriTmNMoA.44W', 3, 3, '2025-05-21 11:14:09', '2025-05-21 11:14:09');
INSERT INTO `user` VALUES (34, '周杰', 'zhoujie1007', '$2a$10$ug0XdPuOZSQuYrqh9N.us.0CR3lxoambIGWTw4MwbrLxrJGi0VATG', 3, 3, '2025-05-21 11:14:09', '2025-05-21 11:14:09');
INSERT INTO `user` VALUES (35, '徐丽', 'xuli1008', '$2a$10$slXdNZCLBRyafcuLR6TV9uWV9KYM5T7.P0J/axYGvMqAUti.KEue2', 3, 3, '2025-05-21 11:14:10', '2025-05-21 11:14:10');
INSERT INTO `user` VALUES (36, '孙浩', 'sunhao1009', '$2a$10$bMY6RyqH9XYtq5Mn7BjxTOhb60xsGnHu6c/XSRcAyLpykXmCeE6ru', 3, 3, '2025-05-21 11:14:10', '2025-05-21 11:14:10');
INSERT INTO `user` VALUES (37, '马琳', 'malin101', '$2a$10$uZ33eBT/u96koh6g5ucJf.ho/3iqBvK/YeUvgYxft0RgXbdGXC7bS', 3, 3, '2025-05-21 11:14:10', '2025-05-21 11:14:10');
INSERT INTO `user` VALUES (38, '吴磊', 'wulei111', '$2a$10$GT6xY61Gk3mN1P9F5zyrK.iCOjdUntuMzjL.qIoOJD.aqSN0d2o22', 3, 3, '2025-05-21 11:14:10', '2025-05-21 11:14:10');
INSERT INTO `user` VALUES (39, '黄婷', 'huangting121', '$2a$10$WOM20Jp5duovyhB75/aw1O2TRjH.gwKj398VdsxEEI1N28pjedruC', 3, 3, '2025-05-21 11:14:11', '2025-05-21 11:14:11');
INSERT INTO `user` VALUES (40, '郭宇', 'guoyu113', '$2a$10$Z7ZC90WZ7g0R6mtRHlvpLumkQe5ckyQpXI6HlPoJcfxljg1/3eGVm', 3, 3, '2025-05-21 11:14:11', '2025-05-21 11:14:11');
INSERT INTO `user` VALUES (41, '何娜', 'hena114', '$2a$10$z2eseuU4bVpLXPOYPldEGOq1UaCkQgdGjiLWKInvRtZENB1CHW3vu', 3, 3, '2025-05-21 11:14:11', '2025-05-21 11:14:11');
INSERT INTO `user` VALUES (42, '林涛', 'lintao115', '$2a$10$AG9DDz531Pc.oQ1FOYWc4.27KtWSUpjvjDV/hmIDRJW3AJIKf8Sn6', 3, 3, '2025-05-21 11:14:12', '2025-05-21 11:14:12');
INSERT INTO `user` VALUES (43, '高雪', 'gao1xue16', '$2a$10$UWQTHcMuZo0rRmPJim7bEOI.PDBE2mo3/ma.I13E0O.OHqxCgXlvC', 3, 3, '2025-05-21 11:14:12', '2025-05-21 11:14:12');
INSERT INTO `user` VALUES (44, '程飞', 'chengf1ei17', '$2a$10$RhZ6VzL48tZip5lEaQWXa.j.EShALqTfdpXoj2jVkhSBQKFUY47My', 3, 3, '2025-05-21 11:14:12', '2025-05-21 11:14:12');
INSERT INTO `user` VALUES (45, '袁敏', 'yuanmin181', '$2a$10$pitirlZUv6A1NiZCnOUj5.aozN508HgvHCKkeSRahEdQs7od9MRY6', 3, 3, '2025-05-21 11:14:12', '2025-05-21 11:14:12');
INSERT INTO `user` VALUES (46, '邓超', 'dengchao191', '$2a$10$oDFRjWclUMkMfJaUJQA.Z.R2WdbxGeASboZVkiHIboo8yXtZeG/Pu', 3, 3, '2025-05-21 11:14:13', '2025-05-21 11:14:13');
INSERT INTO `user` VALUES (47, '蔡欣', 'caixin210', '$2a$10$I/X.f5cpOVWNAAn2Sf1yCeJHyMQzTDE37KEwgSXUkLJ7C/0fPEL2S', 3, 3, '2025-05-21 11:14:13', '2025-05-21 11:14:13');

-- ----------------------------
-- Table structure for user_level
-- ----------------------------
DROP TABLE IF EXISTS `user_level`;
CREATE TABLE `user_level`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_unicode_ci ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of user_level
-- ----------------------------
INSERT INTO `user_level` VALUES (1, '管理员');
INSERT INTO `user_level` VALUES (2, '高级用户');
INSERT INTO `user_level` VALUES (3, '普通用户');

SET FOREIGN_KEY_CHECKS = 1;
