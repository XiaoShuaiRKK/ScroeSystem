using ScoreSystem.Data;
using ScoreSystem.Model;
using ScoreSystem.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScoreSystem
{
    public partial class ScoreRateForm : Form
    {
        private ScoreService scoreService = ScoreService.GetIntance();
        private StatService statService = StatService.GetIntance();
        private List<Exam> exams;
        private List<ExamClassSubjectStat> examClassSubjectStats;
        private bool isLoaded = false;
        public ScoreRateForm()
        {
            InitializeComponent();
        }

        private void ScoreRateForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 数率查看";
            comboBox_exam.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_subjectGroup.DropDownStyle = ComboBoxStyle.DropDownList;
            dataGridView_stat.ReadOnly = true;
            dataGridView_stat.SelectionMode = DataGridViewSelectionMode.CellSelect;
            ComboBoxInit();
            LoadData();
        }

        private async void ComboBoxInit()
        {
            exams = await scoreService.GetExams();
            comboBox_exam.DataSource = exams;
            comboBox_exam.DisplayMember = "Name";
            comboBox_exam.ValueMember = "Id";
            comboBox_subjectGroup.DataSource = Enum.GetValues(typeof(SubjectGroupEnum))
                .Cast<SubjectGroupEnum>()
                .Where(s => s != SubjectGroupEnum.未分组)
                .Select(g => new { Name = g.ToString(), Value = (int)g })
                .ToList();
            comboBox_subjectGroup.DisplayMember = "Name";
            comboBox_subjectGroup.ValueMember = "Value";
            isLoaded = true;
            LoadData();
        }

        private async void LoadData()
        {
            if (!isLoaded) return;

            using (var loading = new LoadForm())
            {
                loading.Show();
                await Task.Delay(100);
                int examId = (int)comboBox_exam.SelectedValue;
                int subjectGroupId = (int)comboBox_subjectGroup.SelectedValue;

                examClassSubjectStats = await statService.GetStats(examId, subjectGroupId);

                if (examClassSubjectStats == null || examClassSubjectStats.Count == 0)
                {
                    dataGridView_stat.DataSource = null;
                    return;
                }

                // 所有科目（包括 CourseId == 100，即 3+1+2 总分）
                var courseIdNameMap = examClassSubjectStats
                    .Select(s => s.CourseId)
                    .Distinct()
                    .ToDictionary(
                        id => id,
                        id => id == 100 ? "3+1+2" :
                               Enum.IsDefined(typeof(CourseEnum), (int)id - 1) ? Enum.GetName(typeof(CourseEnum), (int)id - 1) : $"未知科目({id})"
                    );

                // 所有大学等级
                var universityLevels = examClassSubjectStats
                    .Select(s => s.UniversityLevel)
                    .Distinct()
                    .OrderBy(l => l)
                    .ToList();

                // 所有班级名称
                var classNames = examClassSubjectStats
                    .Select(s => s.ClassName)
                    .Distinct()
                    .ToList();

                // 构建表头
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("班级");

                var columnKeys = new List<(long courseId, int level)>();
                foreach (var courseId in courseIdNameMap.Keys)
                {
                    foreach (var level in universityLevels)
                    {
                        string subject = courseIdNameMap[courseId];
                        dataTable.Columns.Add($"{subject}({(UniversityLevelEnum)level})平均分");
                        dataTable.Columns.Add($"{subject}({(UniversityLevelEnum)level})协同率");
                        dataTable.Columns.Add($"{subject}({(UniversityLevelEnum)level})协同人数");
                        dataTable.Columns.Add($"{subject}({(UniversityLevelEnum)level})贡献率");
                        dataTable.Columns.Add($"{subject}({(UniversityLevelEnum)level})贡献人数");

                        columnKeys.Add((courseId, level));
                    }
                }

                // 构建数据行
                foreach (var className in classNames)
                {
                    var row = dataTable.NewRow();
                    row["班级"] = className;

                    foreach (var (courseId, level) in columnKeys)
                    {
                        var stat = examClassSubjectStats.FirstOrDefault(s =>
                            s.ClassName == className && s.CourseId == courseId && s.UniversityLevel == level);

                        if (stat != null)
                        {
                            string subject = courseIdNameMap[courseId];
                            row[$"{subject}({(UniversityLevelEnum)level})平均分"] = stat.AvgScore.ToString("F2");
                            row[$"{subject}({(UniversityLevelEnum)level})协同率"] = (stat.SynergyRate * 100).ToString("F2") + "%";
                            row[$"{subject}({(UniversityLevelEnum)level})协同人数"] = stat.SynergyCount;
                            row[$"{subject}({(UniversityLevelEnum)level})贡献率"] = (stat.ContributionRate * 100).ToString("F2") + "%";
                            row[$"{subject}({(UniversityLevelEnum)level})贡献人数"] = stat.ContributionCount;
                        }
                    }

                    dataTable.Rows.Add(row);
                }

                dataGridView_stat.DataSource = dataTable;
                loading.Close();
            }

        }

        private string GetCourseName(long courseId)
        {
            try
            {
                return Enum.GetName(typeof(CourseEnum), (int)courseId - 1) ?? $"未知({courseId})";
            }
            catch
            {
                return $"未知({courseId})";
            }
        }

        private string GetUniversityLevelName(int level)
        {
            // 如果值合法，直接返回对应中文名
            if (Enum.IsDefined(typeof(UniversityLevelEnum), level))
            {
                return Enum.GetName(typeof(UniversityLevelEnum), level);
            }
            return $"未知({level})";
        }




        private void comboBox_exam_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void comboBox_subjectGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private async void button_generate_Click(object sender, EventArgs e)
        {
            try
            {
                using (var loading = new LoadForm())
                {
                    loading.Show();
                    await Task.Delay(100);
                    int examId = (int)comboBox_exam.SelectedValue;
                    bool isSuccess = await statService.GenerateStat(examId);
                    if (isSuccess)
                    {
                        MessageBox.Show("生成成功","提示" , MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    loading.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
    }
}
