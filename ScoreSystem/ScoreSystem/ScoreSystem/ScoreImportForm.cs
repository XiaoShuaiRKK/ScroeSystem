using MathNet.Numerics.Distributions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ScoreSystem.Data;
using ScoreSystem.Model;
using ScoreSystem.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScoreSystem
{
    public partial class ScoreImportForm : Form
    {
        private ScoreService scoreService = ScoreService.GetIntance();
        private TeacherService teacherService = new TeacherService();
        private List<ScoreEntity> scores;
        private List<ScoreVO> showScores;
        public ScoreImportForm()
        {
            InitializeComponent();
        }

        private void ScoreImportForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 成绩导入";
            dataGridView_preview.ReadOnly = true;
        }

        private void button_template_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                saveFileDialog.FileName = "分数信息模板.xlsx";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //创建一个Excel工程
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("分数信息");

                    //提示
                    IRow tipRow = sheet.CreateRow(0);
                    tipRow.HeightInPoints = 60;
                    tipRow.CreateCell(0).SetCellValue("请按照本模板格式填写分数信息，严禁修改表头顺序。考试名称必须要已经发布(存在)的，评论可以不填。此行禁止删除！！！");
                    //合并单元格
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 9));
                    // 设置提示样式
                    ICellStyle tipStyle = workbook.CreateCellStyle();
                    IFont tipFont = workbook.CreateFont();
                    tipFont.Color = IndexedColors.Grey40Percent.Index;
                    tipFont.IsItalic = true;
                    tipStyle.SetFont(tipFont);
                    tipStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    tipStyle.WrapText = true;
                    tipRow.Cells[0].CellStyle = tipStyle;
                    //创建表头
                    IRow headerRow = sheet.CreateRow(1);
                    headerRow.CreateCell(0).SetCellValue("学号");
                    headerRow.CreateCell(1).SetCellValue("科目");
                    headerRow.CreateCell(2).SetCellValue("考试名称");
                    headerRow.CreateCell(3).SetCellValue("分数");
                    headerRow.CreateCell(4).SetCellValue("评论");
                    for (int i = 0; i <= 4; i++)
                    {
                        sheet.AutoSizeColumn(i);
                    }
                    using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
                    {
                        workbook.Write(fs);
                    }
                    MessageBox.Show("模板已保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("文档正在使用无法进行操作！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_import_score_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    List<ScoreEntity> scores = new List<ScoreEntity>();
                    List<ScoreVO> showScores = new List<ScoreVO>();
                    HashSet<string> uniquenessSet = new HashSet<string>(); // 用于检查重复录入
                    using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        IWorkbook workbook = new XSSFWorkbook(fs);
                        ISheet sheet = workbook.GetSheetAt(0);

                        for (int i = 2; i <= sheet.LastRowNum; i++)
                        {
                            int displayRow = i + 1;
                            IRow row = sheet.GetRow(i);
                            if (row == null || row.Cells.All(c => c.CellType == CellType.Blank)) continue;

                            string studentNumber = row.GetCell(0)?.ToString().Trim();
                            string courseName = row.GetCell(1)?.ToString().Trim();
                            string examName = row.GetCell(2)?.ToString().Trim();
                            string scoreStr = row.GetCell(3)?.ToString().Trim();
                            string comment = row.GetCell(4)?.ToString().Trim();

                            // 校验学号
                            if (string.IsNullOrEmpty(studentNumber))
                            {
                                MessageBox.Show($"第{displayRow}行，学号不能为空。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if(teacherService.GetStudent(studentNumber) == null)
                            {
                                MessageBox.Show($"第{displayRow}行，学号不存在(学生不存在)。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // 校验科目
                            int courseId = DataUtil.ParseCouseNameToId(courseName);
                            if (courseId == -1)
                            {
                                MessageBox.Show($"第{displayRow}行，科目“{courseName}”无效。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // 校验考试
                            int examId = scoreService.GetExamId(examName);
                            if (examId == -1)
                            {
                                MessageBox.Show($"第{displayRow}行，考试“{examName}”不存在。请先发布考试。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // 校验分数
                            if (!double.TryParse(scoreStr, out double scoreVal))
                            {
                                MessageBox.Show($"第{displayRow}行，分数字段“{scoreStr}”无效，必须为数字。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (scoreVal < 0)
                            {
                                MessageBox.Show($"第{displayRow}行，分数不能为负数。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if ((courseId >= 0 && courseId <= 2 && scoreVal > 150) || (courseId > 2 && scoreVal > 100))
                            {
                                MessageBox.Show($"第{displayRow}行，科目“{courseName}”的分数超出有效范围。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            // 唯一性检查（同学号 + 考试 + 科目）
                            string key = $"{studentNumber}_{examId}_{courseId}";
                            if (uniquenessSet.Contains(key))
                            {
                                MessageBox.Show($"第{displayRow}行，学生“{studentNumber}”的“{examName} - {courseName}”成绩重复。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            uniquenessSet.Add(key);
                            // 构建 ScoreEntity
                            ScoreEntity scoreEntity = new ScoreEntity
                            {
                                StudentNumber = studentNumber,
                                CourseId = courseId + 1,
                                ExamId = examId,
                                Score = scoreVal,
                                Comment = comment
                            };
                            scores.Add(scoreEntity);
                            ScoreVO scoreVO = new ScoreVO
                            {
                                StudentNumber = studentNumber,
                                CourseName = courseName,
                                ExamName = examName,
                                Score = scoreVal,
                                Comment = comment
                            };
                            showScores.Add(scoreVO);
                        }
                    }
                    this.scores = scores;
                    this.showScores = showScores;
                    // 将数据绑定到 DataGridView 预览
                    dataGridView_preview.DataSource = showScores.Select(s => new
                    {
                        学号 = s.StudentNumber,
                        科目 = s.CourseName,
                        考试 = s.ExamName,
                        分数 = s.Score,
                        备注 = s.Comment
                    }).ToList();

                    MessageBox.Show("成绩数据预览成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("导入失败，请确认文件格式正确。\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void button_save_Click(object sender, EventArgs e)
        {
            using(var loading = new LoadForm())
            {
                try
                {
                    loading.Show();
                    await Task.Delay(100);
                    if (scores == null || !scores.Any())
                    {
                        MessageBox.Show("未导入学生数据 无法添加", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    bool isSuccess = await scoreService.AddScore(scores);
                    if (isSuccess)
                    {
                        MessageBox.Show("添加成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearData();
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"保存失败: {ex.Message}");
                }
                finally
                {
                    loading.Close();
                }
            }
        }

        private void ClearData()
        {
            dataGridView_preview.DataSource = null;
            scores = null;
            showScores = null;
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
