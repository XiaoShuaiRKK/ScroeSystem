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
    public partial class GradeRankingControlForm : UserControl
    {
        private ScoreService scoreService = ScoreService.GetIntance();
        private List<Exam> exams;
        private bool isLoaded = false;
        private RankingService rankingService = RankingService.GetIntance();
        public GradeRankingControlForm()
        {
            InitializeComponent();
        }

        private void GradeRankingControlForm_Load(object sender, EventArgs e)
        {
            this.comboBox_exam.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_mode.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_subject_group.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_grade.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBoxInit();
        }

        private async void ComboBoxInit()
        {
            comboBox_exam.DataSource = null;
            exams = await scoreService.GetExams();
            var examDisplayList = exams.Select(e => new
            {
                Id = e.Id,
                Name = $"{e.Name}（{(Enum.IsDefined(typeof(GradeEnum), e.Grade) ? ((GradeEnum)e.Grade).ToString() : "未知年级")}）"
            }).ToList();
            comboBox_exam.DataSource = examDisplayList;
            comboBox_exam.DisplayMember = "Name";
            comboBox_exam.ValueMember = "Id";

            comboBox_subject_group.DataSource = Enum.GetValues(typeof(SubjectGroupEnum))
                .Cast<SubjectGroupEnum>()
                .Select(s => new
                {
                    Value = (int)s,
                    Name = s.ToString()
                }).ToList();
            comboBox_subject_group.DisplayMember = "Name";
            comboBox_subject_group.ValueMember = "Value";

            // RankModeEnum绑定
            comboBox_mode.DataSource = Enum.GetValues(typeof(RankModeEnum))
                .Cast<RankModeEnum>()
                .Select(e => new
                {
                    Value = (int)e,
                    Name = e.ToString()
                })
                .ToList();
            comboBox_mode.DisplayMember = "Name";
            comboBox_mode.ValueMember = "Value";

            comboBox_grade.DataSource = Enum.GetValues(typeof(GradeEnum))
                .Cast<GradeEnum>()
                .Select(e => new
                {
                    Value = (int)e,
                    Name = e.ToString()
                }).ToList();
            comboBox_grade.DisplayMember = "Name";
            comboBox_grade.ValueMember = "Value";
            if (exams.Any())
            {
                isLoaded = true;
            }
            RankInit();
        }

        private async void RankInit()
        {
            if (!isLoaded) return;

            int examId = (int)comboBox_exam.SelectedValue;
            int subjectGroupId = (int)comboBox_subject_group.SelectedValue;
            int rankModeValue = (int)comboBox_mode.SelectedValue;
            int grade = (int)comboBox_grade.SelectedValue;
            RankModeEnum rankMode = (RankModeEnum)rankModeValue;

            // 获取排名数据
            List<StudentRanking> studentRankings = await rankingService.GetGradeRanking(examId, subjectGroupId,grade, rankMode);

            DataTable dt = new DataTable();
            dt.Columns.Add("姓名");
            dt.Columns.Add("学号");

            List<CourseEnum> displayCourses = new List<CourseEnum>();
            bool includeTotal = false;

            if (rankMode == RankModeEnum.总分)
            {
                dt.Columns.Add("总分");
                dt.Columns.Add("排名");
            }
            else if (rankMode == RankModeEnum.各科)
            {
                displayCourses = Enum.GetValues(typeof(CourseEnum)).Cast<CourseEnum>().ToList();
            }
            else if (rankMode == RankModeEnum.三科总分)
            {
                displayCourses = new List<CourseEnum> { CourseEnum.语文, CourseEnum.数学, CourseEnum.英语 };
                includeTotal = true;
            }
            else if (rankMode == RankModeEnum.三加一总分 || rankMode == RankModeEnum.三加一加二总分)
            {
                var firstThree = new List<CourseEnum> { CourseEnum.语文, CourseEnum.数学, CourseEnum.英语 };

                var allCoursesInData = studentRankings
                    .SelectMany(s => s.Ranks)
                    .Where(r => r.CourseId != 0) // 0是总分
                    .Select(r => (CourseEnum)(r.CourseId - 1)) // 映射偏移
                    .Distinct()
                    .ToList();

                var others = allCoursesInData.Except(firstThree)
                                .Take(rankMode == RankModeEnum.三加一总分 ? 1 : 2)
                                .ToList();

                displayCourses = firstThree.Concat(others).ToList();
                includeTotal = true;
            }

            if (rankMode != RankModeEnum.总分)
            {
                foreach (var course in displayCourses)
                {
                    dt.Columns.Add($"{course}分数");
                    dt.Columns.Add($"{course}排名");
                }

                if (includeTotal)
                {
                    dt.Columns.Add("总分");
                    dt.Columns.Add("排名");
                }
            }

            foreach (var student in studentRankings)
            {
                DataRow row = dt.NewRow();
                row["姓名"] = student.StudentName;
                row["学号"] = student.StudentNumber;

                foreach (var rank in student.Ranks)
                {
                    if (rank.CourseId == 0)
                    {
                        if (dt.Columns.Contains("总分"))
                        {
                            row["总分"] = rank.Score;
                            row["排名"] = $"{rank.Rank}/{rank.Total}";
                        }
                        continue;
                    }

                    CourseEnum course = (CourseEnum)(rank.CourseId - 1); // ⭐ 关键点：CourseId 对应 CourseEnum

                    if (!displayCourses.Contains(course)) continue;

                    string scoreCol = $"{course}分数";
                    string rankCol = $"{course}排名";

                    if (dt.Columns.Contains(scoreCol))
                        row[scoreCol] = rank.Score;
                    if (dt.Columns.Contains(rankCol))
                        row[rankCol] = $"{rank.Rank}/{rank.Total}";
                }

                dt.Rows.Add(row);
            }

            dataGridView_rank.DataSource = null;
            dataGridView_rank.DataSource = dt;
        }

        private void comboBox_exam_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoaded)
            {
                RankInit();
            }
        }

        private void comboBox_grade_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoaded)
            {
                RankInit();
            }
        }

        private void comboBox_subject_group_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoaded)
            {
                RankInit();
            }
        }

        private void comboBox_mode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoaded)
            {
                RankInit();
            }
        }
    }
}
