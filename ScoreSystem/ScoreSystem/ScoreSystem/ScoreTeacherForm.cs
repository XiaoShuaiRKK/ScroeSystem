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
        private List<Teacher> teachersPreview = new List<Teacher>();
        private List<TeacherVO> teacherDisplay;
        private TeacherService service = new TeacherService();
        public ScoreTeacherForm()
        {
            InitializeComponent();
        }

        private void ScoreTeacherForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 老师管理";
            this.comboBox_state.DropDownStyle = ComboBoxStyle.DropDownList;
            this.dataGridView_preview.ReadOnly = true;
            this.dataGridView_exams.ReadOnly = true;
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
                .Select(t => new
                {
                    姓名 = t.Name,
                    用户名 = t.Username,
                    工号 = t.TeacherNumber,
                    状态 = ((TeacherStateEnum)t.State).ToString()
                })
                .ToList();

            dataGridView_preview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
        }
    }
}
