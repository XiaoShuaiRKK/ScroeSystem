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
    public partial class ScoreTeacherForm : Form
    {
        private FormAutoScaler autoScaler;
        private List<Teacher> teachersPreview = new List<Teacher>();
        private List<TeacherVO> teacherDisplay;
        private TeacherService service = TeacherService.GetIntance();
        public ScoreTeacherForm()
        {
            InitializeComponent();
            autoScaler = new FormAutoScaler(this);
        }

        private void ScoreTeacherForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 老师管理";
            this.comboBox_state.DropDownStyle = ComboBoxStyle.DropDownList;
            ControlsLoad();
            DataLoad();
        }

        private void ControlsLoad()
        {
            // 年级下拉
            comboBox_state.DataSource = Enum.GetValues(typeof(TeacherStateEnum))
                .Cast<TeacherStateEnum>()
                .Select(g => new { Name = g.ToString(), Value = (int)g })
                .ToList();
            comboBox_state.DisplayMember = "Name";
            comboBox_state.ValueMember = "Value";
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
                    teachersPreview.Clear();

                    for (int i = 2; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;

                        string name = row.GetCell(0)?.ToString()?.Trim();
                        string username = row.GetCell(1)?.ToString()?.Trim();
                        string password = row.GetCell(2)?.ToString()?.Trim();
                        string teacherNumber = row.GetCell(3)?.ToString()?.Trim();
                        string stateStr = row.GetCell(4)?.ToString()?.Trim();

                        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                            throw new Exception($"第{i + 1}行：姓名、用户名和密码不能为空");

                        if (!Enum.TryParse<TeacherStateEnum>(stateStr, out var state))
                            throw new Exception($"第{i + 1}行 状态解析失败");

                        teachersPreview.Add(new Teacher
                        {
                            Name = name,
                            Username = username,
                            Password = password,
                            TeacherNumber = teacherNumber,
                            State = (int)state
                        });
                    }

                    LoadPreview();
                    MessageBox.Show($"导入成功，共 {teachersPreview.Count} 条", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导入失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_template_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            saveFileDialog.FileName = "老师导入模板.xlsx";
            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("老师导入");

            // 提示信息
            IRow tipRow = sheet.CreateRow(0);
            tipRow.HeightInPoints = 60;
            tipRow.CreateCell(0).SetCellValue("请按照本模板填写，用户名不可重复，状态如“正常,辞退,休假,权限限制”。此行不用删除");

            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 4));
            IFont tipFont = workbook.CreateFont();
            tipFont.IsItalic = true;
            tipFont.Color = IndexedColors.Grey40Percent.Index;

            ICellStyle tipStyle = workbook.CreateCellStyle();
            tipStyle.SetFont(tipFont);
            tipRow.GetCell(0).CellStyle = tipStyle;

            // 表头
            IRow header = sheet.CreateRow(1);
            header.CreateCell(0).SetCellValue("姓名");
            header.CreateCell(1).SetCellValue("用户名");
            header.CreateCell(2).SetCellValue("密码");
            header.CreateCell(3).SetCellValue("工号");
            header.CreateCell(4).SetCellValue("状态");

            using (var fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            string name = textBox_name.Text.Trim();
            string username = textBox_username.Text.Trim();
            string password = textBox_password.Text.Trim();
            string teacherNumber = textBox_number.Text.Trim();
            int state = (int)comboBox_state.SelectedValue;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("姓名、用户名、密码为必填项", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (teachersPreview.Any(t => t.Username == username))
            {
                MessageBox.Show("该用户名已存在于预览列表中", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            teachersPreview.Add(new Teacher
            {
                Name = name,
                Username = username,
                Password = password,
                TeacherNumber = teacherNumber,
                State = state
            });

            LoadPreview();
            MessageBox.Show("添加成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void button_save_Click(object sender, EventArgs e)
        {
            if (!teachersPreview.Any())
            {
                MessageBox.Show("没有可保存的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var loading = new LoadForm())
            {
                loading.Show();
                await Task.Delay(100);

                try
                {
                    bool result = await service.AddTeacher(teachersPreview); // 你需实现该服务

                    if (result)
                    {
                        MessageBox.Show("保存成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        teachersPreview.Clear();
                        LoadPreview();
                        DataLoad();
                    }
                    else
                    {
                        MessageBox.Show("保存失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存异常: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    loading.Close();
                }
            }
        }

        private void LoadPreview()
        {
            dataGridView_preview.DataSource = null;
            dataGridView_preview.DataSource = teachersPreview
                .Select(t => new TeacherPreviewVO
                {
                    姓名 = t.Name,
                    用户名 = t.Username,
                    密码 = t.Password,
                    工号 = t.TeacherNumber,
                    状态 = ((TeacherStateEnum)t.State).ToString()
                })
                .ToList();

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
            dataGridView_preview.ReadOnly = false;
            dataGridView_preview.MultiSelect = false;
            dataGridView_preview.Columns["操作"].ReadOnly = true;

            // 替换“状态”列为 ComboBox 列
            ReplaceStateColumnWithComboBox();
        }

        private void ReplaceStateColumnWithComboBox()
        {
            int columnIndex = dataGridView_preview.Columns["状态"].Index;
            // 先删除旧列
            dataGridView_preview.Columns.RemoveAt(columnIndex);

            // 添加新 ComboBox 列
            DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn();
            comboBoxColumn.Name = "状态";
            comboBoxColumn.HeaderText = "状态";
            comboBoxColumn.DataPropertyName = "状态"; // 必须匹配 DataSource 中的属性名
            comboBoxColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;

            // 设置枚举值作为下拉项
            comboBoxColumn.Items.AddRange(Enum.GetNames(typeof(TeacherStateEnum)));

            dataGridView_preview.Columns.Insert(columnIndex, comboBoxColumn);
        }

        private async void DataLoad()
        {
            dataGridView_exams.DataSource = null;
            teacherDisplay = await service.GetTeachers();
            dataGridView_exams.DataSource = teacherDisplay
                .Select(t => new
                {
                    姓名 = t.Name,
                    工号 = t.TeacherNumber,
                    状态 = ((TeacherStateEnum)t.State).ToString()
                })
                .ToList();

            dataGridView_exams.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 确保先清除旧的“操作”列，避免重复添加
            if (dataGridView_exams.Columns.Contains("操作"))
                dataGridView_exams.Columns.Remove("操作");

            // 添加操作列
            DataGridViewLinkColumn deleteColumn = new DataGridViewLinkColumn();
            deleteColumn.Name = "操作";
            deleteColumn.HeaderText = "操作";
            deleteColumn.Text = "删除";
            deleteColumn.UseColumnTextForLinkValue = true;

            dataGridView_exams.Columns.Add(deleteColumn);
        }

        private void dataGridView_preview_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var grid = sender as DataGridView;

            if (grid.Columns[e.ColumnIndex].Name == "操作")
            {
                if (e.RowIndex >= teachersPreview.Count) return;
                teachersPreview.RemoveAt(e.RowIndex);
                LoadPreview();
            }
        }

        private void dataGridView_preview_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var row = dataGridView_preview.Rows[e.RowIndex];
            string name = row.Cells["姓名"].Value?.ToString()?.Trim();
            string username = row.Cells["用户名"].Value?.ToString()?.Trim();
            string teacherNumber = row.Cells["工号"].Value?.ToString()?.Trim();
            string stateStr = row.Cells["状态"].Value?.ToString()?.Trim();
            // 基本验证
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show($"第{e.RowIndex + 1}行：姓名和用户名不能为空", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RevertRow(e.RowIndex);
                return;
            }

            if (!Enum.TryParse<TeacherStateEnum>(stateStr, out var stateEnum))
            {
                MessageBox.Show($"第{e.RowIndex + 1}行 状态无效", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RevertRow(e.RowIndex);
                return;
            }
            if (teachersPreview.Where((_, idx) => idx != e.RowIndex).Any(t => t.Username == username))
            {
                MessageBox.Show($"第{e.RowIndex + 1}行：用户名已存在于列表中", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RevertRow(e.RowIndex);
                return;
            }
            // 所有验证通过后再赋值
            var teacher = teachersPreview[e.RowIndex];
            teacher.Name = name;
            teacher.Username = username;
            teacher.TeacherNumber = teacherNumber;
            teacher.State = (int)stateEnum;
        }

        private void RevertRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= teachersPreview.Count) return;
            var teacher = teachersPreview[rowIndex];
            var row = dataGridView_preview.Rows[rowIndex];
            row.Cells["姓名"].Value = teacher.Name;
            row.Cells["用户名"].Value = teacher.Username;
            row.Cells["工号"].Value = teacher.TeacherNumber;
            row.Cells["状态"].Value = ((TeacherStateEnum)teacher.State).ToString();
        }

        private async void dataGridView_exams_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var grid = sender as DataGridView;

            if (grid.Columns[e.ColumnIndex].Name == "操作")
            {
                if (e.RowIndex >= teacherDisplay.Count) return;
                var result = MessageBox.Show("确定要删除这条教师信息吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    var teacher = teacherDisplay[e.RowIndex];
                    var t = new Teacher
                    {
                        TeacherNumber = teacher.TeacherNumber,
                        Name = teacher.Name,
                        State = teacher.State
                    };
                    bool isSuccess = await service.DeleteTeacher(t);
                    if (isSuccess)
                    {
                        MessageBox.Show("删除成功","提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    }
                    DataLoad();
                }
            }
        }
    }
}
