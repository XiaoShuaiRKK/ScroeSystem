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
    public partial class ClassRankingControlForm : UserControl
    {
        private ClassService classService = ClassService.GetIntance();
        private ScoreService scoreService = ScoreService.GetIntance();
        private RankingService rankingService = RankingService.GetIntance();
        private List<Exam> exams;
        private List<ClassEntity> classEntities;
        private List<StudentRanking> studentRankings;
        private bool isLoaded = false;
        public ClassRankingControlForm()
        {
            InitializeComponent();
        }

        private void ClassRankingControlForm_Load(object sender, EventArgs e)
        {
            this.comboBox_class.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_exam.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_mode.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboxDataInit();
        }

        private async void ComboxDataInit()
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
            comboBox_class.DataSource = null;
            classEntities = await classService.GetAllClasses();
            comboBox_class.DataSource = classEntities;
            comboBox_class.DisplayMember = "Name";
            comboBox_class.ValueMember = "Id";
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

            isLoaded = true;
            RankInit();
        }

        private async void RankInit()
        {
            if (!isLoaded) return;

            int examId = (int)comboBox_exam.SelectedValue;
            int classId = (int)comboBox_class.SelectedValue;
            int rankModeValue = (int)comboBox_mode.SelectedValue;
            RankModeEnum rankMode = (RankModeEnum)rankModeValue;

            List<StudentRanking> studentRankings = await rankingService.GetClassRanking(examId, classId, rankMode);

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
            else if (rankMode == RankModeEnum.三加一总分)
            {
                var firstThree = new List<CourseEnum> { CourseEnum.语文, CourseEnum.数学, CourseEnum.英语 };

                var others = studentRankings
                    .SelectMany(s => s.Ranks)
                    .Where(r => r.CourseId != 0)
                    .Select(r => (CourseEnum)(r.CourseId - 1))
                    .Except(firstThree)
                    .Distinct()
                    .Take(1)
                    .ToList();

                displayCourses = firstThree.Concat(others).ToList();
                includeTotal = true;
            }
            else if (rankMode == RankModeEnum.三加一加二总分)
            {
                var firstThree = new List<CourseEnum> { CourseEnum.语文, CourseEnum.数学, CourseEnum.英语 };

                var allCoursesInData = studentRankings
                    .SelectMany(s => s.Ranks)
                    .Where(r => r.CourseId != 0)
                    .Select(r => (CourseEnum)(r.CourseId - 1))
                    .Distinct()
                    .ToList();

                displayCourses = allCoursesInData
                    .Union(firstThree)
                    .Distinct()
                    .ToList();

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

                    CourseEnum course = (CourseEnum)(rank.CourseId - 1);

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

        private void comboBox_class_SelectedIndexChanged(object sender, EventArgs e)
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
