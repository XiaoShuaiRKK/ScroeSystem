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
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("分数信息");

                    // 提示行
                    IRow tipRow = sheet.CreateRow(0);
                    tipRow.HeightInPoints = 60;
                    tipRow.CreateCell(0).SetCellValue("请按照本模板格式填写成绩，一行对应一个学生的全部科目成绩，考试号必须是已发布考试。此行禁止删除！");
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 10));
                    ICellStyle tipStyle = workbook.CreateCellStyle();
                    IFont tipFont = workbook.CreateFont();
                    tipFont.Color = IndexedColors.Grey40Percent.Index;
                    tipFont.IsItalic = true;
                    tipStyle.SetFont(tipFont);
                    tipStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;
                    tipStyle.WrapText = true;
                    tipRow.Cells[0].CellStyle = tipStyle;

                    // 表头行
                    IRow headerRow = sheet.CreateRow(1);
                    string[] headers = new string[]
                    {
                         "学号","考试号", "语文", "数学", "英语", "物理", "历史", "化学", "生物", "政治", "地理"
                    };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        headerRow.CreateCell(i).SetCellValue(headers[i]);
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
                    HashSet<string> uniquenessSet = new HashSet<string>();

                    var courseMap = new Dictionary<string, int>
            {
                {"语文", 0}, {"数学", 1}, {"英语", 2}, {"物理", 3}, {"历史", 4},
                {"化学", 5}, {"生物", 6}, {"政治", 7}, {"地理", 8}
            };

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
                            string examIdStr = row.GetCell(1)?.ToString().Trim();

                            if (string.IsNullOrEmpty(studentNumber))
                            {
                                MessageBox.Show($"第{displayRow}行，学号不能为空。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (teacherService.GetStudent(studentNumber) == null)
                            {
                                MessageBox.Show($"第{displayRow}行，学号不存在(学生不存在)。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (!int.TryParse(examIdStr, out int examId))
                            {
                                MessageBox.Show($"第{displayRow}行，考试号无效或考试不存在。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            foreach (var kvp in courseMap)
                            {
                                string courseName = kvp.Key;
                                int courseId = kvp.Value;
                                ICell cell = row.GetCell(2 + courseId); // 科目列从第2列开始

                                if (cell == null || cell.CellType == CellType.Blank)
                                {
                                    showScores.Add(new ScoreVO
                                    {
                                        StudentNumber = studentNumber,
                                        ExamName = examId.ToString(),
                                        CourseName = courseName,
                                        Score = 0.0,
                                        Comment = "空白"
                                    });
                                    continue;
                                }

                                string scoreStr = cell.ToString().Trim();
                                if (!double.TryParse(scoreStr, out double scoreVal))
                                {
                                    MessageBox.Show($"第{displayRow}行，{courseName} 分数字段“{scoreStr}”无效，必须为数字。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }

                                if (scoreVal < 0)
                                {
                                    MessageBox.Show($"第{displayRow}行，{courseName} 分数不能为负数。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }

                                if ((courseId <= 2 && scoreVal > 150) || (courseId > 2 && scoreVal > 100))
                                {
                                    MessageBox.Show($"第{displayRow}行，{courseName} 分数超出有效范围。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }

                                showScores.Add(new ScoreVO
                                {
                                    StudentNumber = studentNumber,
                                    ExamName = examId.ToString(),
                                    CourseName = courseName,
                                    Score = scoreVal,
                                    Comment = scoreVal == 0 ? "跳过（0分）" : null
                                });

                                if (scoreVal == 0)
                                {
                                    continue; // 不加入scores
                                }

                                string key = $"{studentNumber}_{examId}_{courseId}";
                                if (uniquenessSet.Contains(key))
                                {
                                    MessageBox.Show($"第{displayRow}行，学生“{studentNumber}”的“{courseName}”成绩重复。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                                uniquenessSet.Add(key);

                                var scoreEntity = new ScoreEntity
                                {
                                    StudentNumber = studentNumber,
                                    ExamId = examId,
                                    CourseId = courseId + 1,
                                    Score = scoreVal,
                                    Comment = null
                                };
                                scores.Add(scoreEntity);
                            }
                        }
                    }

                    this.scores = scores;
                    this.showScores = showScores;

                    // ---- 组装显示表格（每行一位学生，列为各科） ----
                    DataTable pivotedTable = new DataTable();
                    pivotedTable.Columns.Add("学号");
                    pivotedTable.Columns.Add("考试号");

                    string[] subjects = new string[]
                    {
                        "语文", "数学", "英语", "物理", "历史", "化学", "生物", "政治", "地理"
                    };
                    foreach (var subject in subjects)
                    {
                        pivotedTable.Columns.Add(subject);
                    }

                    var grouped = showScores
                        .GroupBy(s => new { s.StudentNumber, s.ExamName })
                        .Select(g =>
                        {
                            var row = pivotedTable.NewRow();
                            row["学号"] = g.Key.StudentNumber;
                            row["考试号"] = g.Key.ExamName;

                            foreach (var score in g)
                            {
                                row[score.CourseName] = score.Score.ToString() ?? "";
                            }

                            return row;
                        });

                    foreach (var row in grouped)
                    {
                        pivotedTable.Rows.Add(row);
                    }

                    dataGridView_preview.DataSource = pivotedTable;

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
