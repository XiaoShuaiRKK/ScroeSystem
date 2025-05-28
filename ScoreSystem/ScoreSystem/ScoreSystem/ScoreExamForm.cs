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
            this.dataGridView_preview.ReadOnly = false;
            this.dataGridView_preview.SelectionMode = DataGridViewSelectionMode.CellSelect;
            this.dataGridView_preview.EditMode = DataGridViewEditMode.EditOnEnter;
            this.dataGridView_preview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ExamsLoad();
            PreExamsLoad();
        }

        private async void ExamsLoad()
        {
            exams = await scoreService.GetExams();
            dataGridView_exams.DataSource = null;
            var examViewList = exams.Select(exam => new
            {
                考试号 = exam.Id,
                名称 = exam.Name,
                年级 = Enum.IsDefined(typeof(GradeEnum), exam.Grade) ? ((GradeEnum)exam.Grade).ToString() : "未知",
                开始时间 = exam.StartDate.ToString("yyyy-MM-dd"),
                结束时间 = exam.EndDate.ToString("yyyy-MM-dd")
            }).ToList();
            dataGridView_exams.DataSource = examViewList;
            EnsureExamDeleteColumn();
        }

        private void PreExamsLoad()
        {
            dataGridView_preview.DataSource = null;
            var bindingList = new BindingList<Exam>(previewExams)
            {
                AllowNew = true,
                AllowEdit = true,
                AllowRemove = true
            };
            dataGridView_preview.DataSource = bindingList;

            // 隐藏原始 Grade 列
            dataGridView_preview.Columns["Grade"].Visible = false;

            // 添加下拉框列（年级）
            if (!dataGridView_preview.Columns.Contains("GradeCombo"))
            {
                var gradeCombo = new DataGridViewComboBoxColumn
                {
                    Name = "GradeCombo",
                    HeaderText = "年级",
                    DataPropertyName = "Grade", // 绑定到 Exam 的 Grade 属性
                    DisplayMember = "Text",
                    ValueMember = "Value",
                    DataSource = Enum.GetValues(typeof(GradeEnum))
                                     .Cast<GradeEnum>()
                                     .Select(g => new { Text = g.ToString(), Value = (int)g })
                                     .ToList(),
                    FlatStyle = FlatStyle.Flat,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
                };
                dataGridView_preview.Columns.Insert(1, gradeCombo); // 插入到第二列
            }

            EnsureDeleteButtonColumn();

            // 设置其他列标题
            dataGridView_preview.Columns["Name"].HeaderText = "考试名称";
            dataGridView_preview.Columns["StartDate"].HeaderText = "开始时间";
            dataGridView_preview.Columns["EndDate"].HeaderText = "结束时间";
            dataGridView_preview.Columns["Year"].HeaderText = "学年";
            dataGridView_preview.Columns["Id"].Visible = false;

            dataGridView_preview.Columns["StartDate"].DefaultCellStyle.Format = "yyyy-MM-dd";
            dataGridView_preview.Columns["EndDate"].DefaultCellStyle.Format = "yyyy-MM-dd";

            dataGridView_preview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView_preview.AllowUserToAddRows = true;
            dataGridView_preview.AllowUserToDeleteRows = true;
            dataGridView_preview.ReadOnly = false;

            foreach (DataGridViewColumn col in dataGridView_preview.Columns)
            {
                col.ReadOnly = false;
            }

            // **确保“操作”列总是在最后**
            if (dataGridView_preview.Columns.Contains("DeleteLink"))
            {
                var deleteCol = dataGridView_preview.Columns["DeleteLink"];
                int lastIndex = dataGridView_preview.Columns.Count - 1;
                if (deleteCol.DisplayIndex != lastIndex)
                {
                    deleteCol.DisplayIndex = lastIndex;
                }
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
                        // 再判断是否和数据库中已存在考试重复
                        if (exams.Any(ex => ex.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && ex.Grade == grade))
                        {
                            MessageBox.Show($"第{displayRow}行，系统中已存在相同考试名称和年级。", "格式错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            button_save.Enabled = false; // 禁用按钮
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
                    button_save.Enabled = true;
                    loading.Close();
                }
            }
        }

        private void ClearData()
        {
            this.previewExams = new List<Exam>();
            PreExamsLoad();
            ExamsLoad();
        }

        private void menu_threshold_Click(object sender, EventArgs e)
        {
            new ScoreThresholdForm().ShowDialog();
        }

        private void dataGridView_preview_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void RevertPreviewRow(int rowIndex)
        {
            dataGridView_preview.CancelEdit();
            dataGridView_preview.Refresh();
        }

        private void dataGridView_preview_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView_preview.Columns[e.ColumnIndex].Name == "DeleteLink")
            {
                var exam = dataGridView_preview.Rows[e.RowIndex].DataBoundItem as Exam;
                if (exam != null)
                {
                    previewExams.Remove(exam);
                    PreExamsLoad(); // 重新加载显示
                }
            }
        }

        private void EnsureDeleteButtonColumn()
        {
            if (!dataGridView_preview.Columns.Contains("DeleteLink"))
            {
                var deleteLinkColumn = new DataGridViewLinkColumn
                {
                    Name = "DeleteLink",
                    HeaderText = "操作",
                    Text = "删除",
                    UseColumnTextForLinkValue = true,
                    LinkColor = Color.Red,
                    TrackVisitedState = false,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                };

                dataGridView_preview.Columns.Add(deleteLinkColumn);
            }
        }

        private void EnsureExamDeleteColumn()
        {
            if (!dataGridView_exams.Columns.Contains("DeleteLink"))
            {
                var deleteColumn = new DataGridViewLinkColumn
                {
                    Name = "DeleteLink",
                    HeaderText = "操作",
                    Text = "删除",
                    UseColumnTextForLinkValue = true,
                    LinkColor = Color.Red,
                    TrackVisitedState = false,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
                };

                dataGridView_exams.Columns.Add(deleteColumn);
            }

            // 确保删除列始终在最后
            var col = dataGridView_exams.Columns["DeleteLink"];
            if (col != null && col.DisplayIndex != dataGridView_exams.Columns.Count - 1)
            {
                col.DisplayIndex = dataGridView_exams.Columns.Count - 1;
            }
        }


        private void dataGridView_preview_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["StartDate"].Value = DateTime.Today;
            e.Row.Cells["EndDate"].Value = DateTime.Today;
            e.Row.Cells["Grade"].Value = (int)GradeEnum.高一上学期;
            e.Row.Cells["Year"].Value = DateTime.Today.Year;
        }

        private void dataGridView_preview_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                var dgv = sender as DataGridView;

                if (e.RowIndex < 0 || e.RowIndex >= dgv.Rows.Count)
                    return;

                // 越界保护
                if (e.RowIndex >= dgv.Rows.Count) return;

                var row = dgv.Rows[e.RowIndex];

                // 如果是新行或 DataBoundItem 为 null，跳过验证
                if (row.IsNewRow || row.DataBoundItem == null)
                    return;

                // 如果是 DataRowView 并且被标记为 Deleted，跳过
                if (row.DataBoundItem is DataRowView drv && drv.Row.RowState == DataRowState.Deleted)
                    return;

                // 如果不是 Exam 类型，也跳过
                if (!(row.DataBoundItem is Exam exam))
                    return;

                // 检查需要的列是否存在，防止列被动态移除导致访问异常
                if (!dgv.Columns.Contains("GradeCombo") ||
                    !dgv.Columns.Contains("Name") ||
                    !dgv.Columns.Contains("Year"))
                    return;

                // 安全访问单元格值
                string name = row.Cells["Name"]?.Value?.ToString()?.Trim();
                object gradeObj = row.Cells["GradeCombo"]?.Value;
                string yearStr = row.Cells["Year"]?.Value?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show($"第{e.RowIndex + 1}行：考试名称不能为空", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }

                if (gradeObj == null || !int.TryParse(gradeObj.ToString(), out int grade))
                {
                    MessageBox.Show($"第{e.RowIndex + 1}行：年级格式无效，应为下拉选择", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }

                if (string.IsNullOrWhiteSpace(yearStr) || !int.TryParse(yearStr, out int year))
                {
                    MessageBox.Show($"第{e.RowIndex + 1}行：学年格式无效，应为数字", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }

                int maxYear = DateTime.Today.Year + 10;
                if (year <= 2000 || year >= maxYear)
                {
                    MessageBox.Show($"第{e.RowIndex + 1}行：学年范围必须在 2000 - {maxYear}", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }

                // 检查重复（排除当前行）
                var duplicate = previewExams
                    .Where((ex, index) => index != e.RowIndex)
                    .Any(ex => ex.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && ex.Grade == grade);

                if (duplicate)
                {
                    MessageBox.Show($"第{e.RowIndex + 1}行：预览中已存在相同的考试名称和年级。", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }

                // 最后更新 exam 对象
                exam.Name = name;
                exam.Grade = grade;
                exam.Year = year;
            }
            catch (Exception ex)
            {
                return;
            }
            
        }

        private void dataGridView_preview_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            string columnName = dataGridView_preview.Columns[e.ColumnIndex].HeaderText;
            if (dataGridView_preview.Columns[e.ColumnIndex].ValueType == typeof(DateTime))
            {
                MessageBox.Show($"第{e.RowIndex + 1}行，{columnName} 格式无效，请输入完整的日期（例如：2025-05-01）", "数据输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show($"第{e.RowIndex + 1}行，{columnName} 数据输入错误", "数据输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            e.ThrowException = false; // 防止异常冒泡
        }

        private async void dataGridView_exams_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView_exams.Columns[e.ColumnIndex].Name == "DeleteLink")
            {
                var selectedExam = exams.ElementAtOrDefault(e.RowIndex);
                if (selectedExam == null)
                    return;

                var result = MessageBox.Show($"确定要删除考试 “{selectedExam.Name}”？", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        bool deleted = false;
                        using (var loading = new LoadForm())
                        {
                            loading.Show();
                            await Task.Delay(100); // 确保加载窗显示
                            deleted = await scoreService.DeleteExam(selectedExam); // 假设你有此服务方法
                            loading.Close();
                        }

                        if (deleted)
                        {
                            MessageBox.Show("删除成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ExamsLoad(); // 重新加载考试列表
                        }
                        else
                        {
                            MessageBox.Show("删除失败，请重试", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"删除过程中出现错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
