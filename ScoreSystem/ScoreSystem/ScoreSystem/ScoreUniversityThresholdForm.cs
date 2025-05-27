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
    public partial class ScoreUniversityThresholdForm : Form
    {
        private ScoreMainForm mainForm;
        private UniversitySevice universitySevice = UniversitySevice.GetIntance();
        private ScoreService scoreService = ScoreService.GetIntance();

        private ThresholdRankingResult thresholdRanking;
        private bool isLoaded = false;

        public ScoreUniversityThresholdForm(ScoreMainForm mainForm)
        {
            this.mainForm = mainForm;
            InitializeComponent();
        }

        private void ScoreUniversityThresholdForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 上线管理";
            this.FormClosed += (s, ex) =>
            {
                this.mainForm.Show();
                this.Dispose();
            };
            comboBox_grade.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_exam.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_university_level.DropDownStyle = ComboBoxStyle.DropDownList;
            LoadCombobox();
        }

        private async void LoadCombobox()
        {
            // 年级下拉
            comboBox_grade.DataSource = Enum.GetValues(typeof(GradeEnum))
                .Cast<GradeEnum>()
                .Select(g => new { Name = g.ToString(), Value = (int)g })
                .ToList();
            comboBox_grade.DisplayMember = "Name";
            comboBox_grade.ValueMember = "Value";

            // 大学等级下拉
            comboBox_university_level.DataSource = Enum.GetValues(typeof(UniversityLevelEnum))
                .Cast<UniversityLevelEnum>()
                .Where(u => u != UniversityLevelEnum.全部) // 去掉“全部”
                .Select(u => new { Name = u.ToString(), Value = (int)u })
                .ToList();
            comboBox_university_level.DisplayMember = "Name";
            comboBox_university_level.ValueMember = "Value";

            // 考试下拉
            var exams = await scoreService.GetExams();
            comboBox_exam.DataSource = exams;
            comboBox_exam.DisplayMember = "Name";
            comboBox_exam.ValueMember = "Id";

            if (exams.Any())
            {
                isLoaded = true;
            }

            LoadThreshold();
        }

        private async void LoadThreshold()
        {
            if (!isLoaded) return;

            int grade = (int)comboBox_grade.SelectedValue;
            int examId = (int)comboBox_exam.SelectedValue;

            thresholdRanking = await universitySevice.GetGradeThresholdRanking(grade, examId);

            FilterThresholdResults(); // 加载初始等级对应数据
        }

        private void comboBox_grade_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoaded) LoadThreshold();
        }

        private void comboBox_exam_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoaded) LoadThreshold();
        }

        private void comboBox_university_level_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoaded) FilterThresholdResults();
        }

        private void FilterThresholdResults()
        {
            if (thresholdRanking == null || thresholdRanking.LevelStudentList == null)
                return;

            var selectedLevel = (UniversityLevelEnum)comboBox_university_level.SelectedValue;
            string levelKey = selectedLevel.ToString();

            if (thresholdRanking.LevelStudentList.TryGetValue(levelKey, out var studentList))
            {
                var displayList = studentList.SelectMany(s => s.Ranks.Select(r => new
                {
                    学号 = s.StudentNumber,
                    姓名 = s.StudentName,
                    课程编号 = r.CourseId,
                    课程名称 = r.CourseName,
                    成绩 = r.Score,
                    排名 = r.Rank,
                    总人数 = r.Total,
                    排名范围 = r.Scope
                })).ToList();

                dataGridView_threshold.DataSource = null;
                dataGridView_threshold.DataSource = displayList;
            }
            else
            {
                dataGridView_threshold.DataSource = null;
                MessageBox.Show("该等级下暂无学生数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void button_back_Click(object sender, EventArgs e)
        {
            this.mainForm.Show();
            this.Dispose();
        }

        private void menu_university_Click(object sender, EventArgs e)
        {
            new ScoreUniversityForm().ShowDialog();
        }

        private void menu_predict_Click(object sender, EventArgs e)
        {
            new ScorePredictForm().ShowDialog();
        }
    }
}
