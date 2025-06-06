using EnumsNET;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScoreSystem
{
    public partial class ScoreUniversityForm : Form
    {
        private FormAutoScaler autoScaler;
        private UniversitySevice universityService = UniversitySevice.GetIntance();
        private List<University> previewUniversities = new List<University>();
        private List<University> universities;
        private bool isTemplateImport = false;

        public ScoreUniversityForm()
        {
            InitializeComponent();
            autoScaler = new FormAutoScaler(this);
        }

        private void ScoreUniversityForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 大学信息管理";

            dtp_year.Format = DateTimePickerFormat.Custom;
            dtp_year.CustomFormat = "yyyy";
            dtp_year.ShowUpDown = true;
            dtp_year.MaxDate = new DateTime(DateTime.Now.Year, 12, 31);

            num_art_score_line.DecimalPlaces = 1;
            num_art_score_line.Increment = 0.5M;
            num_art_score_line.Minimum = 0;

            num_scient_score_line.DecimalPlaces = 1;
            num_scient_score_line.Increment = 0.5M;
            num_scient_score_line.Minimum = 0;

            comboBox_level.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_level.DataSource = Enum.GetValues(typeof(UniversityLevelEnum))
                .Cast<UniversityLevelEnum>()
                .Where(x => x != UniversityLevelEnum.全部)
                .Select(x => new { Name = x.ToString(), Value = (int)x }).ToList();
            comboBox_level.DisplayMember = "Name";
            comboBox_level.ValueMember = "Value";

            dataGridView_preview.ReadOnly = true;
            dataGridView_preview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView_university.ReadOnly = true;
            dataGridView_university.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // TODO: Load real data if needed.
            LoadUniversites();
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            string name = textBox_university_name.Text.Trim();
            int year = dtp_year.Value.Year;
            long level = (long)comboBox_level.SelectedValue;
            double sci = (double)num_scient_score_line.Value;
            double art = (double)num_art_score_line.Value;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("大学名称不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (name.Length > 50 || Regex.IsMatch(name, @"[^a-zA-Z0-9\u4e00-\u9fa5\s\-_()（）【】]"))
            {
                MessageBox.Show("大学名称过长或包含非法字符", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (previewUniversities.Any(u => u.Name == name && u.Year == year))
            {
                MessageBox.Show("已存在相同名称与年份的大学记录", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            previewUniversities.Add(new University
            {
                Name = name,
                Year = year,
                UniversityLevel = level,
                ScienceScoreLine = sci,
                ArtScoreLine = art
            });

            LoadPreview();
        }

        private void LoadPreview()
        {
            dataGridView_preview.DataSource = null;
            var list = previewUniversities.Select(u => new
            {
                名称 = u.Name,
                年份 = u.Year,
                层次 = ((UniversityLevelEnum)u.UniversityLevel).ToString(),
                理科线 = u.ScienceScoreLine,
                文科线 = u.ArtScoreLine
            }).ToList();
            dataGridView_preview.DataSource = list;

            if (isTemplateImport)
            {
                button_add.Enabled = false;
            }
        }
        private async void LoadUniversites()
        {
            this.dataGridView_university.DataSource = null;
            universities = await universityService.GetUniversities();
            var list = universities.Select(u => new
            {
                名称 = u.Name,
                年份 = u.Year,
                层次 = ((UniversityLevelEnum)u.UniversityLevel).ToString(),
                理科线 = u.ScienceScoreLine,
                文科线 = u.ArtScoreLine
            }).ToList();
            this.dataGridView_university.DataSource = list;
        }

        private void button_template_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel 文件 (*.xlsx)|*.xlsx",
                FileName = "大学信息模板.xlsx"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("大学信息");

                IRow tipRow = sheet.CreateRow(0);
                tipRow.CreateCell(0).SetCellValue("请按照模板填写大学信息，层次如 九八五 双一流(不能填写数字985和211) 优投 本科；年份仅填写年份（如2024）；此行禁止删除！");
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 4));

                IRow header = sheet.CreateRow(1);
                header.CreateCell(0).SetCellValue("大学名称");
                header.CreateCell(1).SetCellValue("年份");
                header.CreateCell(2).SetCellValue("大学层次");
                header.CreateCell(3).SetCellValue("理科分数线");
                header.CreateCell(4).SetCellValue("文科分数线");

                using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fs);
                }

                MessageBox.Show("模板生成成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button_import_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Excel 文件 (*.xlsx)|*.xlsx"
            };

            if (ofd.ShowDialog() != DialogResult.OK) return;

            try
            {
                using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook = new XSSFWorkbook(fs);
                    ISheet sheet = workbook.GetSheetAt(0);

                    if (sheet == null || sheet.LastRowNum < 2)
                    {
                        MessageBox.Show("表格无内容或格式错误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    previewUniversities.Clear();

                    for (int i = 2; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;

                        string name = row.GetCell(0)?.ToString()?.Trim();
                        string yearStr = row.GetCell(1)?.ToString()?.Trim();
                        string levelStr = row.GetCell(2)?.ToString()?.Trim();
                        string sciStr = row.GetCell(3)?.ToString()?.Trim();
                        string artStr = row.GetCell(4)?.ToString()?.Trim();

                        if (string.IsNullOrWhiteSpace(name) || name.Length > 50 || Regex.IsMatch(name, @"[^a-zA-Z0-9\u4e00-\u9fa5\s\-_()（）【】]"))
                        {
                            MessageBox.Show($"第{i + 1}行，大学名称无效", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (!int.TryParse(yearStr, out int year) || year > DateTime.Now.Year)
                        {
                            MessageBox.Show($"第{i + 1}行，年份格式无效", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        if (levelStr.Equals("985") || levelStr.Equals("211"))
                        {
                            MessageBox.Show($"第{i + 1}行，大学层次格式错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        if (!Enum.TryParse<UniversityLevelEnum>(levelStr,out UniversityLevelEnum levelObj))
                        {
                            MessageBox.Show($"第{i + 1}行，大学层次格式错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (!double.TryParse(sciStr, out double sci) || !double.TryParse(artStr, out double art))
                        {
                            MessageBox.Show($"第{i + 1}行，分数线格式错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        previewUniversities.Add(new University
                        {
                            Name = name,
                            Year = year,
                            UniversityLevel = (int)levelObj,
                            ScienceScoreLine = sci,
                            ArtScoreLine = art
                        });
                    }

                    isTemplateImport = true;
                    LoadPreview();
                    MessageBox.Show("导入成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导入失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button_save_Click(object sender, EventArgs e)
        {
            if (previewUniversities == null || !previewUniversities.Any())
            {
                MessageBox.Show("没有可保存的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //try
            //{
                bool result = await universityService.AddUniversities(previewUniversities);
                if (result)
                {
                    MessageBox.Show("保存成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearData();
                    LoadUniversites();
                }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("保存异常：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private void ClearData()
        {
            previewUniversities = new List<University>();
            dataGridView_preview.DataSource = null;
            button_add.Enabled = true;
        }
    }
}
