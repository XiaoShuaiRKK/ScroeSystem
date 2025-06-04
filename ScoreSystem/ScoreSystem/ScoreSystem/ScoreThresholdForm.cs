using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Tls.Crypto;
using Org.BouncyCastle.Utilities;
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
    public partial class ScoreThresholdForm : Form
    {
        private List<Exam> exams;
        private List<ExamSubjectThreshold> examSubjectThresholds;
        private List<ExamSubjectThreshold> previewThresholds;
        private ScoreService scoreService = ScoreService.GetIntance();
        private bool isLoaded = false;
        public ScoreThresholdForm()
        {
            InitializeComponent();
        }

        private void ScoreThresholdForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 考试达标线设置";
            ControlLoad();
        }

        private async void ControlLoad()
        {
            this.comboBox_add_exam.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_exam.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_course.DropDownStyle = ComboBoxStyle.DropDownList;
            exams = await scoreService.GetExams();
            var examDisplayList = exams.Select(e => new
            {
                Id = e.Id,
                Name = $"{e.Name}（{(Enum.IsDefined(typeof(GradeEnum), e.Grade) ? ((GradeEnum)e.Grade).ToString() : "未知年级")}）"
            }).ToList();
            this.comboBox_add_exam.DataSource = examDisplayList.ToList();
            this.comboBox_add_exam.DisplayMember = "Name";
            this.comboBox_add_exam.ValueMember = "Id";
            this.comboBox_exam.DataSource = examDisplayList.ToList();
            this.comboBox_exam.DisplayMember = "Name";
            this.comboBox_exam.ValueMember = "Id";
            var courses = Enum.GetValues(typeof(CourseEnum))
                .Cast<CourseEnum>()
                .Select(c => new
                {
                    Text = c.ToString(),
                    Value = (int)c
                }).ToList();
            this.comboBox_course.DataSource = courses;
            this.comboBox_course.DisplayMember = "Text";
            this.comboBox_course.ValueMember = "Value";

            this.dataGridView_exam.ReadOnly = true;
            this.dataGridView_preview.ReadOnly = true;
            this.dataGridView_exam.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView_preview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            this.num_threshold.DecimalPlaces = 1;
            num_threshold.Increment = 0.5M;
            num_threshold.Minimum = 0M;
            num_threshold.Maximum = 150M;
            if (exams.Any())
            {
                this.isLoaded = true;
            }
            ExamThresholdLoad();
        }

        private async void ExamThresholdLoad()
        {
            if (!isLoaded) return;
            int examId = (int)comboBox_exam.SelectedValue;
            examSubjectThresholds = await scoreService.GetThresholds(examId);
            var displayData = examSubjectThresholds.Select(t => new
            {
                考试名称 = exams.FirstOrDefault(e => e.Id == t.ExamId)?.Name ?? "未知考试",
                科目 = ((CourseEnum)t.CourseId).ToString(),
                达标分数 = t.ThresholdScore
            }).ToList();
            this.dataGridView_exam.DataSource = null;
            this.dataGridView_exam.DataSource = displayData;
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            int examId = (int)comboBox_add_exam.SelectedValue;
            int courseId = (int)comboBox_course.SelectedValue + 1;
            double threshold = (double)num_threshold.Value;
            // 初始化列表
            if (previewThresholds == null)
            {
                previewThresholds = new List<ExamSubjectThreshold>();
            }

            // 检查是否已存在该考试和科目的记录
            if (previewThresholds.Any(t => t.ExamId == examId && t.CourseId == courseId))
            {
                MessageBox.Show("该考试的该科目已设置过达标线，不能重复添加！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 检查阈值是否为0
            if (threshold <= 0)
            {
                MessageBox.Show("达标分数必须大于0！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 检查分数上限
            string courseName = ((CourseEnum)courseId).ToString();
            if ((courseName == "语文" || courseName == "数学" || courseName == "英语") && threshold > 150)
            {
                MessageBox.Show($"语文、数学、英语的达标线不能超过150分！当前设置为：{threshold}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (courseName != "语文" && courseName != "数学" && courseName != "英语" && threshold > 100)
            {
                MessageBox.Show($"除语数英外其他科目的达标线不能超过100分！当前设置为：{threshold}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 添加
            var newThreshold = new ExamSubjectThreshold
            {
                ExamId = examId,
                CourseId = courseId,
                ThresholdScore = threshold
            };

            previewThresholds.Add(newThreshold);
            RefreshPreviewThresholds();
        }

        private void RefreshPreviewThresholds()
        {
            var displayData = previewThresholds.Select(t => new
            {
                考试名称 = exams.FirstOrDefault(e => e.Id == t.ExamId)?.Name ?? "未知考试",
                科目 = ((CourseEnum)t.CourseId).ToString(),
                达标分数 = t.ThresholdScore
            }).ToList();

            dataGridView_preview.DataSource = null;
            dataGridView_preview.DataSource = displayData;
        }

        private void button_template_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                saveFileDialog.FileName = "考试达标分信息模板.xlsx";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //创建一个Excel工程
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("考试达标分信息");

                    //提示
                    IRow tipRow = sheet.CreateRow(0);
                    tipRow.HeightInPoints = 60;
                    tipRow.CreateCell(0).SetCellValue("请按照本模板格式填写考试达标分信息，严禁修改表头顺序。考试名称不能有特殊符号。年级格式应为高一上学期、高二下学期。开始时间不能早于今天、结束时间不能早于开始时间。此行禁止删除！！！");
                    //合并单元格
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 3));
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
                    // 第二行表头
                    IRow headerRow = sheet.CreateRow(1);
                    headerRow.CreateCell(0).SetCellValue("考试名称");
                    headerRow.CreateCell(1).SetCellValue("科目");
                    headerRow.CreateCell(2).SetCellValue("达标分数");

                    for (int i = 0; i <= 2; i++)
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

        private void button_import_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fs);
                ISheet sheet = workbook.GetSheetAt(0);

                previewThresholds = new List<ExamSubjectThreshold>();

                for (int i = 2; i <= sheet.LastRowNum; i++) // 从第3行开始读取（索引从0开始）
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null || row.Cells.All(c => c == null || string.IsNullOrWhiteSpace(c.ToString()))) continue;

                    string examName = row.GetCell(0)?.ToString().Trim();
                    string subjectName = row.GetCell(1)?.ToString().Trim();
                    string thresholdStr = row.GetCell(2)?.ToString().Trim();

                    if (string.IsNullOrWhiteSpace(examName) || string.IsNullOrWhiteSpace(subjectName) || string.IsNullOrWhiteSpace(thresholdStr))
                    {
                        MessageBox.Show($"第{i + 1}行存在空值，已跳过。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    // 获取考试ID
                    var exam = exams.FirstOrDefault(ex => ex.Name == examName);
                    if (exam == null)
                    {
                        MessageBox.Show($"第{i + 1}行考试名称“{examName}”不存在，已跳过。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    // 获取课程ID
                    if (!Enum.TryParse<CourseEnum>(subjectName, out CourseEnum courseEnum))
                    {
                        MessageBox.Show($"第{i + 1}行科目“{subjectName}”不是有效的科目名称，已跳过。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    int courseId = (int)courseEnum;

                    if (!double.TryParse(thresholdStr, out double threshold) || threshold <= 0)
                    {
                        MessageBox.Show($"第{i + 1}行达标分数“{thresholdStr}”无效或小于等于0，已跳过。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    string courseName = ((CourseEnum)courseId).ToString();
                    if ((courseName == "语文" || courseName == "数学" || courseName == "英语") && threshold > 150)
                    {
                        MessageBox.Show($"第{i + 1}行语文/数学/英语达标分不能超过150，已跳过。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }
                    else if (courseName != "语文" && courseName != "数学" && courseName != "英语" && threshold > 100)
                    {
                        MessageBox.Show($"第{i + 1}行非语数英科目达标分不能超过100，已跳过。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    // 检查是否重复
                    if (previewThresholds.Any(t => t.ExamId == exam.Id && t.CourseId == courseId))
                    {
                        MessageBox.Show($"第{i + 1}行考试“{examName}”的科目“{subjectName}”已存在，已跳过。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        continue;
                    }

                    // 添加
                    previewThresholds.Add(new ExamSubjectThreshold
                    {
                        ExamId = exam.Id,
                        CourseId = courseId,
                        ThresholdScore = threshold
                    });
                }
                this.button_add.Enabled = false;
                RefreshPreviewThresholds();
                MessageBox.Show("导入完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void button_save_Click(object sender, EventArgs e)
        {
            using (var loading = new LoadForm())
            {
                try
                {
                    loading.Show();
                    await Task.Delay(100);
                    if (previewThresholds == null || !previewThresholds.Any())
                    {
                        MessageBox.Show("未导入学生数据 无法添加", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    bool isSuccess = await scoreService.AddThreshold(previewThresholds);
                    if (isSuccess)
                    {
                        MessageBox.Show("添加成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ExamThresholdLoad();
                        ClearData();
                    }
                }
                catch (Exception ex)
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
            this.button_add.Enabled = true;
            this.previewThresholds = null;
            this.dataGridView_preview.DataSource = null;
            this.num_threshold.Value = 0;
        }

        private void comboBox_exam_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoaded)
            {
                ExamThresholdLoad();
            }
        }
    }
}
