using ScoreSystem.Data;
using ScoreSystem.Model;
using ScoreSystem.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScoreSystem
{
    public partial class ScoreCriticalForm : Form
    {
        private CriticalService criticalService = new CriticalService();
        private List<CriticalStudentLog> logs;
        private bool isLoaded = false;
        public ScoreCriticalForm()
        {
            InitializeComponent();
        }

        private void ScoreCriticalForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 临界生管理";
            this.comboBox_grade.DropDownStyle = ComboBoxStyle.DropDownList;
            dtp_year.Format = DateTimePickerFormat.Custom;
            dtp_year.CustomFormat = "yyyy";
            dtp_year.ShowUpDown = true;
            dtp_year.MaxDate = new DateTime(DateTime.Now.Year, 12, 31);
            ControlsLoad();
        }

        private void ControlsLoad()
        {
            // 年级下拉
            comboBox_grade.DataSource = Enum.GetValues(typeof(GradeEnum))
                .Cast<GradeEnum>()
                .Select(g => new { Name = g.ToString(), Value = (int)g })
                .ToList();
            comboBox_grade.DisplayMember = "Name";
            comboBox_grade.ValueMember = "Value";
            isLoaded = true;
        }

        private void menu_critical_config_Click(object sender, EventArgs e)
        {
            new ScoreCriticalConfigForm().ShowDialog();
        }

        private void comboBox_grade_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dtp_year_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private async void LoadData()
        {
            if (!isLoaded) return;
            int grade = (int)comboBox_grade.SelectedValue;
            int year = dtp_year.Value.Year;
            try
            {
                logs = await criticalService.GetCriticalByGrade(grade, year);

                if (logs == null || logs.Count == 0)
                {
                    MessageBox.Show("未找到符合条件的临界生数据。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dataGridView_critical.DataSource = null;
                    return;
                }

                var displayLogs = logs.Select(log => new
                {
                    编号 = log.Id,
                    考试ID = log.ExamId,
                    学号 = log.StudentNumber,
                    姓名 = log.StudentName,
                    大学等级 = log.UniversityLevel,
                    成绩排名 = log.ScoreRank,
                    目标排名 = log.TargetRank,
                    差距 = log.Gap,
                    分数 = log.Score,
                    分科ID = log.SubjectGroupId
                }).ToList();

                dataGridView_critical.DataSource = displayLogs;
                dataGridView_critical.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载临界生数据失败。\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
