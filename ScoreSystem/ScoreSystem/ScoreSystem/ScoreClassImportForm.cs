using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Bcpg.Sig;
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
    public partial class ScoreClassImportForm : Form
    {
        private List<ClassEntity> previewClass = new List<ClassEntity>();
        private List<ClassEntity> displayClass;
        private List<TeacherVO> teachers;
        private TeacherService teacherService = new TeacherService();
        private ClassService classService = ClassService.GetIntance();
        public ScoreClassImportForm()
        {
            InitializeComponent();
        }

        private void ScoreClassImportForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 班级导入";
            this.dataGridView_class.ReadOnly = true;
            this.dataGridView_preview.ReadOnly = true;
            this.comboBox_grade.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_subject_group.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_teacher.DropDownStyle = ComboBoxStyle.DropDownList;
            dataGridView_preview.CellContentClick += dataGridView_preview_CellContentClick;
            ControlsDataLoad();
            LoadClass();
        }

        private async void ControlsDataLoad()
        {
            // 年级下拉
            comboBox_grade.DataSource = Enum.GetValues(typeof(GradeEnum))
                .Cast<GradeEnum>()
                .Select(g => new { Name = g.ToString(), Value = (int)g })
                .ToList();
            comboBox_grade.DisplayMember = "Name";
            comboBox_grade.ValueMember = "Value";

            comboBox_subject_group.DataSource = Enum.GetValues(typeof(SubjectGroupEnum))
               .Cast<SubjectGroupEnum>()
               .Where(s => s != SubjectGroupEnum.未分组)
               .Select(s => new
               {
                   Value = (int)s,
                   Name = s.ToString()
               }).ToList();
            comboBox_subject_group.DisplayMember = "Name";
            comboBox_subject_group.ValueMember = "Value";

            teachers = await teacherService.GetTeachers();
            comboBox_teacher.DataSource = teachers;
            comboBox_teacher.DisplayMember = "Name";
            comboBox_teacher.ValueMember = "TeacherNumber";
        }

        private void button_template_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            saveFileDialog.FileName = "班级导入模板.xlsx";
            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("班级导入");

            // 提示信息
            IRow tipRow = sheet.CreateRow(0);
            tipRow.HeightInPoints = 60;
            tipRow.CreateCell(0).SetCellValue("请填写班级信息，教师工号请从下拉列表中获取。此行禁止删除！！！");

            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 4));
            IFont tipFont = workbook.CreateFont();
            tipFont.IsItalic = true;
            tipFont.Color = IndexedColors.Grey40Percent.Index;

            ICellStyle tipStyle = workbook.CreateCellStyle();
            tipStyle.SetFont(tipFont);
            tipRow.GetCell(0).CellStyle = tipStyle;

            // 表头
            IRow header = sheet.CreateRow(1);
            header.CreateCell(0).SetCellValue("班级名称");
            header.CreateCell(1).SetCellValue("年级");
            header.CreateCell(2).SetCellValue("学科组");
            header.CreateCell(3).SetCellValue("班主任工号");

            using (var fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }

            MessageBox.Show("模板导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            string className = textBox_name.Text.Trim();
            int grade = (int)comboBox_grade.SelectedValue;
            int subjectGroupId = (int)comboBox_subject_group.SelectedValue;
            string teacherNumber = comboBox_teacher.SelectedValue?.ToString();

            if (string.IsNullOrWhiteSpace(className) || string.IsNullOrWhiteSpace(teacherNumber))
            {
                MessageBox.Show("班级名称和班主任不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TeacherVO teacher = teachers.FirstOrDefault(t => t.TeacherNumber == teacherNumber);
            if (teacher == null)
            {
                MessageBox.Show("无法找到对应的班主任信息", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            previewClass.Add(new ClassEntity
            {
                Name = className,
                Grade = grade,
                SubjectGroupId = subjectGroupId,
                HeadTeacherId = (int)teacher.UserId,
                TeacherName = teacher.Name
            });

            LoadClassPreview();
            LoadClass();
            MessageBox.Show("添加成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    previewClass.Clear();

                    for (int i = 2; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;

                        string className = row.GetCell(0)?.ToString()?.Trim();
                        string gradeStr = row.GetCell(1)?.ToString()?.Trim();
                        string groupStr = row.GetCell(2)?.ToString()?.Trim();
                        string teacherNo = row.GetCell(3)?.ToString()?.Trim();

                        if (string.IsNullOrWhiteSpace(className) || string.IsNullOrWhiteSpace(gradeStr) ||
                            string.IsNullOrWhiteSpace(groupStr) || string.IsNullOrWhiteSpace(teacherNo))
                            throw new Exception($"第{i + 1}行数据不能为空");

                        if (!Enum.TryParse<GradeEnum>(gradeStr, out var gradeEnum))
                            throw new Exception($"第{i + 1}行 年级解析失败");

                        if (!Enum.TryParse<SubjectGroupEnum>(groupStr, out var groupEnum))
                            throw new Exception($"第{i + 1}行 学科组解析失败");

                        TeacherVO teacher = teachers.FirstOrDefault(t => t.TeacherNumber == teacherNo);
                        if (teacher == null)
                            throw new Exception($"第{i + 1}行 找不到工号为 {teacherNo} 的教师");

                        previewClass.Add(new ClassEntity
                        {
                            Name = className,
                            Grade = (int)gradeEnum,
                            SubjectGroupId = (int)groupEnum,
                            HeadTeacherId = (int)teacher.Id,
                            TeacherName = teacher.Name
                        });
                    }

                    LoadClassPreview();
                    MessageBox.Show($"导入成功，共 {previewClass.Count} 条", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导入失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadClassPreview()
        {
            dataGridView_preview.DataSource = null;

            var displayData = previewClass.Select((c, index) => new
            {
                序号 = index + 1,
                班级名称 = c.Name,
                年级 = ((GradeEnum)c.Grade).ToString(),
                学科组 = ((SubjectGroupEnum)c.SubjectGroupId).ToString(),
                班主任 = c.TeacherName
            }).ToList();

            dataGridView_preview.DataSource = displayData;

            // 设置列自适应
            dataGridView_preview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 确保先清除旧的“操作”列，避免重复添加
            if (dataGridView_preview.Columns.Contains("操作"))
                dataGridView_preview.Columns.Remove("操作");

            // 添加操作列
            DataGridViewLinkColumn deleteColumn = new DataGridViewLinkColumn();
            deleteColumn.Name = "操作";
            deleteColumn.HeaderText = "操作";
            deleteColumn.Text = "删除";
            deleteColumn.UseColumnTextForLinkValue = true;

            dataGridView_preview.Columns.Add(deleteColumn);

            // 设置行为：只读、整行选中、不可多选
            dataGridView_preview.ReadOnly = true;
            dataGridView_preview.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView_preview.MultiSelect = false;
        }

        private void dataGridView_preview_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var grid = sender as DataGridView;

            if (grid.Columns[e.ColumnIndex].Name == "操作")
            {
                if (e.RowIndex >= previewClass.Count) return;

                var result = MessageBox.Show("确定要删除这条班级信息吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    previewClass.RemoveAt(e.RowIndex);
                    LoadClassPreview();
                }
            }
        }

        private async void LoadClass()
        { 
            dataGridView_class.DataSource = null;
            displayClass = await classService.GetAllClasses();
            dataGridView_class.DataSource = displayClass.Select(c => new
            {
                班级名称 = c.Name,
                年级 = ((GradeEnum)c.Grade).ToString(),
                学科组 = ((SubjectGroupEnum)c.SubjectGroupId).ToString(),
                班主任 = c.TeacherName
            }).ToList();

            dataGridView_class.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private async void button_save_Click(object sender, EventArgs e)
        {
            using (var loading = new LoadForm())
            {
                loading.Show();
                await Task.Delay(100); // 确保窗口显示
                try
                {
                    if (previewClass == null || !previewClass.Any())
                    {
                        MessageBox.Show("未导入班级数据 无法添加", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    bool isSuccess = await classService.AddClass(previewClass);
                    if (isSuccess)
                    {
                        MessageBox.Show("添加成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearData();
                        LoadClass();
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
            this.textBox_name.Text = "";
            this.previewClass = null;
            this.dataGridView_preview.DataSource = null;
        }
    }
}
