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
    public partial class ScoreCriticalConfigForm : Form
    {
        private List<CriticalConfig> previewConfigs = new List<CriticalConfig>();
        private List<CriticalConfig> criticalConfigs;
        private CriticalService criticalService = new CriticalService();

        public ScoreCriticalConfigForm()
        {
            InitializeComponent();
        }

        private void ScoreCriticalConfigForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 临界配置";
            comboBox_grade.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_subject_group.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_university_level.DropDownStyle = ComboBoxStyle.DropDownList;
            dataGridView_exams.ReadOnly = true;
            dataGridView_preview.ReadOnly = true;
            ComboBoxLoad();
            LoadPreView();
            CriticalConfigLoad();
        }

        private void ComboBoxLoad()
        {
            dtp_year.Format = DateTimePickerFormat.Custom;
            dtp_year.CustomFormat = "yyyy";
            dtp_year.ShowUpDown = true;
            dtp_year.MaxDate = new DateTime(DateTime.Now.Year, 12, 31);
            // 大学等级下拉
            comboBox_university_level.DataSource = Enum.GetValues(typeof(UniversityLevelEnum))
                .Cast<UniversityLevelEnum>()
                .Where(u => u != UniversityLevelEnum.全部) // 去掉“全部”
                .Select(u => new { Name = u.ToString(), Value = (int)u })
                .ToList();
            comboBox_university_level.DisplayMember = "Name";
            comboBox_university_level.ValueMember = "Value";

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

            num_target_count.Minimum = 1;
            num_critical_ratio.Minimum = 0.1M;
            num_critical_ratio.DecimalPlaces = 1;
            num_critical_ratio.Increment = 0.5M;

            this.dataGridView_preview.DataSource = null;
            this.dataGridView_exams.DataSource = null;
        }

        private void LoadPreView()
        {
            dataGridView_preview.DataSource = null;

            dataGridView_preview.DataSource = previewConfigs
                .Select(c => new
                {
                    年级 = ((GradeEnum)c.Grade).ToString(),
                    年份 = c.Year,
                    大学等级 = ((UniversityLevelEnum)c.UniversityLevel).ToString(),
                    目标人数 = c.TargetCount,
                    临界比例 = c.CriticalRatio,
                    科目组合 = ((SubjectGroupEnum)c.SubjectGroupId).ToString()
                })
                .ToList();

            dataGridView_preview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView_preview.AllowUserToAddRows = false;
            dataGridView_preview.ReadOnly = true;
        }

        private async void CriticalConfigLoad()
        {
            try
            {
                this.criticalConfigs = await criticalService.GetCriticalConfigs();

                dataGridView_exams.DataSource = null;

                dataGridView_exams.DataSource = criticalConfigs
                    .Select(c => new
                    {
                        年级 = ((GradeEnum)c.Grade).ToString(),
                        年份 = c.Year,
                        大学等级 = ((UniversityLevelEnum)c.UniversityLevel).ToString(),
                        目标人数 = c.TargetCount,
                        临界比例 = c.CriticalRatio,
                        科目组合 = ((SubjectGroupEnum)c.SubjectGroupId).ToString()
                    })
                    .ToList();

                dataGridView_exams.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView_exams.AllowUserToAddRows = false;
                dataGridView_exams.ReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载已有配置失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button_template_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel文件(*.xlsx)|*.xlsx";
            saveFileDialog.FileName = "临界配置模板.xlsx";
            if (saveFileDialog.ShowDialog() != DialogResult.OK) return;

            IWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("临界配置");

            IRow tipRow = sheet.CreateRow(0);
            tipRow.HeightInPoints = 60;
            tipRow.CreateCell(0).SetCellValue("请按照本模板填写，删除或修改列顺序将导致导入失败。年级如“高一上学期”，大学等级如“双一流”，组合如“理科”");

            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, 5));
            IFont tipFont = workbook.CreateFont();
            tipFont.IsItalic = true;
            tipFont.Color = IndexedColors.Grey40Percent.Index;
            
            ICellStyle tipStyle = workbook.CreateCellStyle();
            tipStyle.SetFont(tipFont);
            tipRow.GetCell(0).CellStyle = tipStyle;

            IRow header = sheet.CreateRow(1);
            header.CreateCell(0).SetCellValue("年级");
            header.CreateCell(1).SetCellValue("年份");
            header.CreateCell(2).SetCellValue("大学等级");
            header.CreateCell(3).SetCellValue("目标人数");
            header.CreateCell(4).SetCellValue("临界比例");
            header.CreateCell(5).SetCellValue("科目组合");

            using (var fs = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }

            MessageBox.Show("模板导出成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            var config = new CriticalConfig
            {
                Grade = (int)comboBox_grade.SelectedValue,
                Year = dtp_year.Value.Year,
                UniversityLevel = (int)comboBox_university_level.SelectedValue,
                TargetCount = (int)num_target_count.Value,
                CriticalRatio = (double)num_critical_ratio.Value,
                SubjectGroupId = (int)comboBox_subject_group.SelectedValue
            };

            // 检查重复
            if (previewConfigs.Any(c =>
                c.Grade == config.Grade &&
                c.Year == config.Year &&
                c.UniversityLevel == config.UniversityLevel &&
                c.SubjectGroupId == config.SubjectGroupId))
            {
                MessageBox.Show("该配置已存在！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            previewConfigs.Add(config);
            LoadPreView();
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
                    previewConfigs.Clear();

                    for (int i = 2; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue;

                        string gradeStr = row.GetCell(0)?.ToString()?.Trim();
                        string yearStr = row.GetCell(1)?.ToString()?.Trim();
                        string levelStr = row.GetCell(2)?.ToString()?.Trim();
                        string targetStr = row.GetCell(3)?.ToString()?.Trim();
                        string ratioStr = row.GetCell(4)?.ToString()?.Trim();
                        string groupStr = row.GetCell(5)?.ToString()?.Trim();

                        if (!Enum.TryParse<GradeEnum>(gradeStr, out var grade))
                            throw new Exception($"第{i + 1}行 年级解析失败");

                        if (!int.TryParse(yearStr, out var year))
                            throw new Exception($"第{i + 1}行 年份解析失败");

                        if (!Enum.TryParse<UniversityLevelEnum>(levelStr, out var level))
                            throw new Exception($"第{i + 1}行 大学等级解析失败");

                        if (!int.TryParse(targetStr, out var targetCount) || targetCount < 1)
                            throw new Exception($"第{i + 1}行 目标人数无效");

                        if (!double.TryParse(ratioStr, out var ratio) || ratio < 0 || ratio > 100)
                            throw new Exception($"第{i + 1}行 临界比例无效");

                        if (!Enum.TryParse<SubjectGroupEnum>(groupStr, out var group))
                            throw new Exception($"第{i + 1}行 科目组合解析失败");

                        previewConfigs.Add(new CriticalConfig
                        {
                            Grade = (int)grade,
                            Year = year,
                            UniversityLevel = (int)level,
                            TargetCount = targetCount,
                            CriticalRatio = ratio,
                            SubjectGroupId = (int)group
                        });
                    }
                    LoadPreView();
                    MessageBox.Show("导入成功，共导入 " + previewConfigs.Count + " 条", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导入失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private async void button_save_Click(object sender, EventArgs e)
        {
            if (!previewConfigs.Any())
            {
                MessageBox.Show("没有可保存的数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var loading = new LoadForm())
            {
                loading.Show();
                await Task.Delay(100); // 显示窗口

                try
                {
                    bool success = await criticalService.AddScore(previewConfigs); // 你需实现该接口
                    if (success)
                    {
                        MessageBox.Show("保存成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        previewConfigs.Clear();
                    }
                    else
                    {
                        MessageBox.Show("保存失败，请重试", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
