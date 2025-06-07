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
        private FormAutoScaler autoScaler;
        private CriticalService criticalService = new CriticalService();
        private RankingService rankingService = RankingService.GetIntance();
        private ScoreService scoreService = ScoreService.GetIntance();
        private TeacherService teacherService = TeacherService.GetIntance();
        private List<StudentRanking> studentRankings;
        private List<CriticalConfig> criticalConfigs;
        private List<Exam> exams;
        private Dictionary<string, Color> studentRowColors = new Dictionary<string, Color>();
        private bool isLoaded = false;

        private ScoreCriticalConfigForm scoreCriticalConfigForm;

        public ScoreCriticalForm()
        {
            InitializeComponent();
            autoScaler = new FormAutoScaler(this);
            dataGridView_critical.DataBindingComplete += dataGridView_critical_DataBindingComplete;
        }

        private void ScoreCriticalForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 临界生管理";
            this.comboBox_exam.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_subject_group.DropDownStyle = ComboBoxStyle.DropDownList;
            this.dataGridView_critical.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
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
            if(scoreCriticalConfigForm == null)
            {
                scoreCriticalConfigForm = new ScoreCriticalConfigForm();
                scoreCriticalConfigForm.Show();
                scoreCriticalConfigForm.FormClosed += (s, ex) =>
                {
                    scoreCriticalConfigForm = null;
                };
            }
            else
            {
                scoreCriticalConfigForm.BringToFront();
            }
            LoadData();
        }

        

        private async void LoadData()
        {
            if (!isLoaded) return;

            int examId = (int)comboBox_exam.SelectedValue;
            int subjectGroupId = (int)comboBox_subject_group.SelectedValue;

            studentRankings = await rankingService.GetStudentGradeSubjectGroupRanking(examId, subjectGroupId);
            criticalConfigs = await criticalService.GetCriticalConfigByGrade(examId);

            var configs = criticalConfigs
                .Where(c => c.SubjectGroupId == subjectGroupId && !c.Deleted)
                .OrderBy(c => c.UniversityLevel)
                .ToList();

            if (!configs.Any())
            {
                MessageBox.Show("未找到该分组的临界配置");
                return;
            }

            // 构建等级区间
            var levelThresholds = new List<(int upperBound, UniversityLevelEnum level)>();
            int currentThreshold = 0;

            foreach (var config in configs)
            {
                currentThreshold += config.TargetCount;
                levelThresholds.Add((currentThreshold, (UniversityLevelEnum)config.UniversityLevel));
            }

            int maxThreshold = currentThreshold + 10;

            var filteredStudents = studentRankings
                .Select(s =>
                {
                    var rankInfo = s.Ranks.FirstOrDefault();
                    return new
                    {
                        Student = s,
                        RankInfo = rankInfo
                    };
                })
                .Where(x => x.RankInfo != null && x.RankInfo.Rank <= maxThreshold)
                .OrderBy(x => x.RankInfo.Rank)
                .ToList();

            UniversityLevelEnum GetUniversityLevelByRank(int rank)
            {
                foreach (var threshold in levelThresholds)
                {
                    if (rank <= threshold.upperBound)
                        return threshold.level;
                }
                return UniversityLevelEnum.全部;
            }

            // 设置背景色映射
            studentRowColors.Clear();

            foreach (var config in configs)
            {
                int target = config.TargetCount;
                int up = config.FloatUpCount;
                int down = config.FloatDownCount;
                var level = (UniversityLevelEnum)config.UniversityLevel;

                var levelStudents = filteredStudents
                    .Where(x => GetUniversityLevelByRank(x.RankInfo.Rank) == level)
                    .OrderBy(x => x.RankInfo.Rank)
                    .ToList();

                // 上浮绿色
                foreach (var s in levelStudents.Take(up))
                {
                    studentRowColors[s.Student.StudentNumber] = Color.LightGreen;
                }

                // 下浮黄色，从末尾向前数 down 个
                foreach (var s in levelStudents.Skip(Math.Max(0, levelStudents.Count - down)))
                {
                    studentRowColors[s.Student.StudentNumber] = Color.LightYellow;
                }
            }

            // 构造 DataSource 数据
            var viewData = filteredStudents.Select(s => new
            {
                学号 = s.Student.StudentNumber,
                姓名 = s.Student.StudentName,
                三加一加二总分 = s.RankInfo.Score,
                排名 = $"{s.RankInfo.Rank}/{s.RankInfo.Total}",
                可考大学等级 = GetUniversityLevelByRank(s.RankInfo.Rank) == UniversityLevelEnum.全部
                    ? ""
                    : GetUniversityLevelByRank(s.RankInfo.Rank).ToString()
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

        private void dataGridView_critical_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView_critical.Rows)
            {
                if (row.Cells["学号"].Value is string studentNumber)
                {
                    if (studentRowColors.TryGetValue(studentNumber, out var color))
                    {
                        row.DefaultCellStyle.BackColor = color;
                    }
                }
            }
        }

        private async void dataGridView_critical_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridView_critical.CurrentRow != null && dataGridView_critical.CurrentRow.Index >= 0)
            {
                var selectedRow = dataGridView_critical.CurrentRow;

                // 确保“学号”列存在且值为字符串
                if (selectedRow.Cells["学号"].Value is string studentNumber)
                {
                    Student student = await teacherService.GetStudent(studentNumber);
                    ScoreTrendFormManage.ShowTrendForm(student);
                }
            }
        }
    }
}
