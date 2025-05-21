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
    public partial class ScoreExamForm : Form
    {
        private ScoreService scoreService = ScoreService.GetIntance();
        private List<Exam> exams;
        private List<Exam> previewExams = new List<Exam>();
        private bool isTemplateImport = false;
        public ScoreExamForm()
        {
            InitializeComponent();
        }

        private void ScoreExamForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 考试管理";
            this.dataGridView_exams.ReadOnly = true;
            this.dataGridView_exams.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView_preview.ReadOnly = true;
            this.dataGridView_preview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dateTimePicker_start.MinDate = DateTime.Today;
            this.dateTimePicker_end.MinDate = DateTime.Today;
            ExamsLoad();
            ComboxInit();
        }

        private void ComboxInit()
        {
            var grades = Enum.GetValues(typeof(GradeEnum))
                .Cast<GradeEnum>()
                .Select(g => new
                {
                    Text = g.ToString(),
                    Value = (int)g
                }).ToList();
            comboBox_grade.DisplayMember = "Text";   // 显示内容
            comboBox_grade.ValueMember = "Value";    // 实际值
            comboBox_grade.DataSource = grades;

            comboBox_grade.DropDownStyle = ComboBoxStyle.DropDownList; // 不可编辑
        }

        private async void ExamsLoad()
        {
            exams = await scoreService.GetExams();
            dataGridView_exams.DataSource = null;
            var examViewList = exams.Select(exam => new
            {
                名称 = exam.Name,
                年级 = Enum.IsDefined(typeof(GradeEnum), exam.Grade) ? ((GradeEnum)exam.Grade).ToString() : "未知",
                开始时间 = exam.StartDate.ToString("yyyy-MM-dd"),
                结束时间 = exam.EndDate.ToString("yyyy-MM-dd")
            }).ToList();
            dataGridView_exams.DataSource = examViewList;
        }

        private void PreExamsLoad()
        {
            if (isTemplateImport)
            {
                button_add.Enabled = false;
            }
            dataGridView_preview.DataSource = null;
            var examViewList = previewExams.Select(exam => new
            {
                名称 = exam.Name,
                年级 = Enum.IsDefined(typeof(GradeEnum), exam.Grade) ? ((GradeEnum)exam.Grade).ToString() : "未知",
                开始时间 = exam.StartDate.ToString("yyyy-MM-dd"),
                结束时间 = exam.EndDate.ToString("yyyy-MM-dd")
            }).ToList();
            dataGridView_preview.DataSource = examViewList;
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            string examName = textBox_exam_name.Text.Trim();
            DateTime startTime = dateTimePicker_start.Value;
            DateTime endTime = dateTimePicker_end.Value;
            int grade = (int)comboBox_grade.SelectedValue;
            // 1. 验证长度
            if (examName.Length == 0)
            {
                MessageBox.Show("考试名称不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (examName.Length > 50)
            {
                MessageBox.Show("考试名称不能超过50个字符", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. 验证不能包含特殊字符（允许汉字、英文字母、数字、空格和常见符号）
            if (System.Text.RegularExpressions.Regex.IsMatch(examName, @"[^a-zA-Z0-9\u4e00-\u9fa5\s\-_()（）【】]"))
            {
                MessageBox.Show("考试名称不能包含特殊字符", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 3. 检查是否已存在相同考试名称和年级
            if (previewExams.Any(ex => ex.Name == examName && ex.Grade == grade))
            {
                MessageBox.Show("已存在相同名称和年级的考试，不能重复添加", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Exam exam = new Exam
            {
                Name = examName,
                StartDate = startTime,
                EndDate = endTime,
                Grade = grade
            };
            previewExams.Add(exam);
            PreExamsLoad();
        }

        private void dateTimePicker_start_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker_end.MinDate = dateTimePicker_start.Value;
            if(dateTimePicker_end.Value < dateTimePicker_start.Value)
            {
                dateTimePicker_end.Value = dateTimePicker_start.Value;
            }
        }

        private void button_template_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
                saveFileDialog.FileName = "考试信息模板.xlsx";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //创建一个Excel工程
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("考试信息");

                    //提示
                    IRow tipRow = sheet.CreateRow(0);
                    tipRow.HeightInPoints = 60;
                    tipRow.CreateCell(0).SetCellValue("请按照本模板格式填写考试信息，严禁修改表头顺序。考试名称不能有特殊符号。年级格式应为高一上学期、高二下学期。开始时间不能早于今天、结束时间不能早于开始时间。此行严禁删除！！！");
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
                    IRow headerRow = sheet.CreateRow(1);
                    headerRow.CreateCell(0).SetCellValue("考试名称");
                    headerRow.CreateCell(1).SetCellValue("年级");
                    headerRow.CreateCell(2).SetCellValue("开始时间");
                    headerRow.CreateCell(3).SetCellValue("结束时间");
                    for (int i = 0; i <= 3; i++)
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

            try
            {
                using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook = new XSSFWorkbook(fs);
                    ISheet sheet = workbook.GetSheetAt(0);

                    if (sheet == null || sheet.LastRowNum < 2)
                    {
                        MessageBox.Show("Excel内容为空或格式错误！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    this.previewExams.Clear(); // 清空之前的
                    for (int i = 2; i <= sheet.LastRowNum; i++)
                    {
                        int displayRow = i + 1;
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;

                        string name = row.GetCell(0)?.ToString()?.Trim() ?? "";
                        string gradeStr = row.GetCell(1)?.ToString()?.Trim() ?? "";
                        DateTime startDate = DataUtil.ParseDateCell(row.GetCell(2));
                        DateTime endDate = DataUtil.ParseDateCell(row.GetCell(3));

                        // 名称验证
                        if (string.IsNullOrEmpty(name) || name.Length > 50 ||
                            System.Text.RegularExpressions.Regex.IsMatch(name, @"[^a-zA-Z0-9\u4e00-\u9fa5\s\-_()（）【】]"))
                        {
                            MessageBox.Show($"第{displayRow}行，考试名称为空 或者 考试名称过长 或者 考试名称含有非法字符。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // 年级验证
                        if (!Enum.TryParse<GradeEnum>(gradeStr, out GradeEnum gradeEnum))
                        {
                            MessageBox.Show($"第{displayRow}行，年级 {gradeStr} 无效。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // 时间解析
                        if (startDate == DateTime.MinValue || endDate == DateTime.MinValue)
                        {
                            MessageBox.Show($"第{displayRow}行， 无法解析开始或结束时间。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // 时间逻辑验证
                        if (startDate.Date < DateTime.Today || endDate.Date < startDate.Date)
                        {
                            MessageBox.Show($"第{displayRow}行，（开始时间不能早于今天，结束时间不能早于开始时间）。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        int grade = (int)gradeEnum;

                        // 查重
                        if (previewExams.Any(ex => ex.Name == name && ex.Grade == grade))
                        {
                            MessageBox.Show($"第{displayRow}行，已存在考试记录 请再次检查名称和年级。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // 添加
                        previewExams.Add(new Exam
                        {
                            Name = name,
                            Grade = grade,
                            StartDate = startDate,
                            EndDate = endDate
                        });
                    }
                    isTemplateImport = true;
                    PreExamsLoad();
                    MessageBox.Show("导入完成", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导入失败，文件可能被占用或格式错误。\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button_save_Click(object sender, EventArgs e)
        {
            using (var loading = new LoadForm())
            {
                loading.Show();
                await Task.Delay(100); // 确保窗口显示
                try
                {
                    if (previewExams == null || !previewExams.Any())
                    {
                        MessageBox.Show("未导入考试数据 无法添加", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    bool isSuccess = await scoreService.AddExams(previewExams);
                    if (isSuccess)
                    {
                        MessageBox.Show("添加成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            this.previewExams = null;
            this.dataGridView_preview.DataSource = null;
            this.button_add.Enabled = true;
            ExamsLoad();
        }

        private void menu_threshold_Click(object sender, EventArgs e)
        {
            new ScoreThresholdForm().ShowDialog();
        }
    }
}
