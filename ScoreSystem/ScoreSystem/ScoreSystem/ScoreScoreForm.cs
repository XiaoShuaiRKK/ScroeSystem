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
    public partial class ScoreScoreForm : Form
    {
        private ScoreMainForm scoreMainForm;
        private List<Exam> exams;
        private List<ClassEntity> classEntities;
        private List<StudentScore> studentScores;
        private ScoreService scoreService = ScoreService.GetIntance();
        private ClassService classService = ClassService.GetIntance();
        private bool isLoaded = false;

        public ScoreScoreForm(ScoreMainForm scoreMainForm)
        {
            this.scoreMainForm = scoreMainForm;
            InitializeComponent();
        }

        private void ScoreScoreForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 分数管理";
            comboBox_class.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_exam.DropDownStyle = ComboBoxStyle.DropDownList;
            dataGridView_score.ReadOnly = true;
            this.FormClosed += (s, ev) =>
            {
                Environment.Exit(0);
            };
            ComboxDataInit();
        }

        private async void ComboxDataInit()
        {
            comboBox_exam.DataSource = null;
            exams = await scoreService.GetExams();
            comboBox_exam.DataSource = exams;
            comboBox_exam.DisplayMember = "Name";
            comboBox_exam.ValueMember = "Id";
            comboBox_class.DataSource = null;
            classEntities = await classService.GetAllClasses();
            comboBox_class.DataSource = classEntities;
            comboBox_class.DisplayMember = "Name";
            comboBox_class.ValueMember = "Id";
            isLoaded = true;
            ScoreInit();
        }

        private async void ScoreInit()
        {
            if (isLoaded)
            {
                int examId = (int)comboBox_exam.SelectedValue;
                int classId = (int)comboBox_class.SelectedValue;
                studentScores = await scoreService.GetScoresByClass(examId, classId);
                var allCourseNames = Enum.GetNames(typeof(CourseEnum));
                //构建DataTable
                DataTable dt = new DataTable();
                dt.Columns.Add("姓名");
                dt.Columns.Add("学号");
                dt.Columns.Add("班级");
                foreach (var courseName in allCourseNames)
                {
                    dt.Columns.Add(courseName);
                }
                // 添加每个学生数据
                foreach (var student in studentScores)
                {
                    DataRow row = dt.NewRow();
                    row["姓名"] = student.Name;
                    row["学号"] = student.StudentNumber;
                    row["班级"] = student.ClassName;

                    // 填充已有成绩
                    foreach (var score in student.Scores)
                    {
                        string courseName = ((CourseEnum)score.CourseId).ToString();
                        if (dt.Columns.Contains(courseName))
                        {
                            row[courseName] = score.Score;
                        }
                    }
                    dt.Rows.Add(row);
                }
                dataGridView_score.DataSource = null;
                dataGridView_score.DataSource = dt;
            }
        }

        private void comboBox_exam_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScoreInit();
        }

        private void comboBox_class_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScoreInit();
        }

        private void button_back_Click(object sender, EventArgs e)
        {
            scoreMainForm.Show();
            this.Dispose();
        }

        private void menu_import_Click(object sender, EventArgs e)
        {
            new ScoreImportForm().ShowDialog();
            ScoreInit();
        }
    }
}
