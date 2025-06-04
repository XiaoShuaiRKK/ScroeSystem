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
        private RankingService rankingService = RankingService.GetIntance();
        private ScoreService scoreService = ScoreService.GetIntance();
        private List<StudentRanking> studentRankings;
        private List<CriticalConfig> criticalConfigs;
        private List<Exam> exams;
        private bool isLoaded = false;
        public ScoreCriticalForm()
        {
            InitializeComponent();
        }

        private void ScoreCriticalForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 临界生管理";
            this.comboBox_exam.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_subject_group.DropDownStyle = ComboBoxStyle.DropDownList;
            ControlsLoad();
        }

        private async void ControlsLoad()
        {
            // 年级下拉
            comboBox_subject_group.DataSource = Enum.GetValues(typeof(SubjectGroupEnum))
                .Cast<SubjectGroupEnum>()
                .Where(c => c != SubjectGroupEnum.未分组)
                .Select(g => new { Name = g.ToString(), Value = (int)g })
                .ToList();
            comboBox_subject_group.DisplayMember = "Name";
            comboBox_subject_group.ValueMember = "Value";
            exams = await scoreService.GetExams();
            comboBox_exam.DataSource = exams;
            comboBox_exam.DisplayMember = "Name";
            comboBox_exam.ValueMember = "Id";
            isLoaded = true;
            LoadData();
        }

        private void menu_critical_config_Click(object sender, EventArgs e)
        {
            new ScoreCriticalConfigForm().ShowDialog();
        }

        

        private async void LoadData()
        {
            if (!isLoaded) return;
            int examId = (int)comboBox_exam.SelectedValue;
            int subjectGroupId = (int)comboBox_subject_group.SelectedValue;
            studentRankings = await rankingService.GetStudentGradeSubjectGroupRanking(examId,subjectGroupId);
            criticalConfigs = await criticalService.GetCriticalConfigByGrade();
            var viewData = studentRankings.Select(s =>
            {
                var r = s.Ranks.FirstOrDefault(); // 只取唯一的一项“3+1+2”数据
                return new
                {
                    学号 = s.StudentNumber,
                    姓名 = s.StudentName,
                    三加一加二总分 = r?.Score ?? 0,
                    排名 = r != null ? $"{r.Rank}/{r.Total}" : ""
                };
            }).ToList();

            dataGridView_critical.DataSource = viewData;
            dataGridView_critical.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void comboBox_exam_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void comboBox_subject_group_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
