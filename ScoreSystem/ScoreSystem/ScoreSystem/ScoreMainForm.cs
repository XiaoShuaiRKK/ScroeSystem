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
    public partial class ScoreMainForm : Form
    {
        private UserService userService = UserService.GetIntance();
        private ClassService classService = ClassService.GetIntance();
        private TeacherService teacherService = new TeacherService();
        private ScoreLoginForm loginForm;
        private User user;
        private List<Student> students;
        private bool isLoadClass = false;
        private ScoreTrendForm trendForm = null;
        private string trendFormStudentNumber = null;
        public ScoreMainForm(ScoreLoginForm loginForm)
        {
            this.loginForm = loginForm;
            InitializeComponent();
        }

        private void ScoreMainForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 首页";
            comboBox_class.DropDownStyle = ComboBoxStyle.DropDownList;
            dataGridView_students.ReadOnly = true;
            dataGridView_students.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView_students.MultiSelect = false;
            dataGridView_students.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.FormClosed += (s, ev) =>
            {
                Environment.Exit(0);
            };
            TimerInit();
            UserInfoInit();
            ClassInfoInit();
        }

        private void TimerInit()
        {
            timer_now_time.Interval = 1000;
            timer_now_time.Tick += (sender, e) =>
            {
                label_time.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            };
            timer_now_time.Start();
        }

        private void menu_logout_Click(object sender, EventArgs e)
        {
            this.loginForm.Show();
            this.Dispose();
        }

        private void UserInfoInit()
        {
            this.user = userService.GetUser();
            label_hello.Text = $"你好 {user.Name} {user.Role}";
        }

        private async void ClassInfoInit()
        {
            List<ClassEntity> classEntities = await classService.GetAllClasses();
            comboBox_class.DataSource = classEntities;
            comboBox_class.DisplayMember = "Name";
            comboBox_class.ValueMember = "Id";
            isLoadClass = true;
            StudentDataInit();
        }

        private void comboBox_class_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoadClass)
            {
                StudentDataInit();
            }
        }

        private async void StudentDataInit()
        {
            int selectedId = (int)comboBox_class.SelectedValue;
            students = await teacherService.GetStudentByClassId(selectedId);
            dataGridView_students.Columns.Clear();
            var displayStudents = students.Select(s => new
            {
                学号 = s.StudentNumber,
                姓名 = s.Name,
                选科一 = Enum.GetName(typeof(CourseEnum), s.ElectiveCourse1Id - 1),
                选科二 = Enum.GetName(typeof(CourseEnum), s.ElectiveCourse2Id - 1),
                入学时间 = s.EnrollmentDate
            }).ToList();
            dataGridView_students.DataSource = displayStudents;
        }

        private void menu_class_or_student_Click(object sender, EventArgs e)
        {
            new ScoreClassOrStudentOperateForm().ShowDialog();
            StudentDataInit();
        }

        private void menu_score_Click(object sender, EventArgs e)
        {
            new ScoreScoreForm(this).Show();
            this.Hide();
        }

        private void menu_exam_Click(object sender, EventArgs e)
        {
            new ScoreExamForm().ShowDialog();
        }

        private void menu_university_Click(object sender, EventArgs e)
        {
            this.Hide();
            new ScoreUniversityThresholdForm(this).Show();
        }

        private void menu_trend_Click(object sender, EventArgs e)
        {
            if (dataGridView_students.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择一名学生进行走势分析。");
                return;
            }

            var selectedRow = dataGridView_students.SelectedRows[0];
            string studentNumber = selectedRow.Cells["学号"].Value.ToString();

            // 找到完整学生对象
            var student = students.FirstOrDefault(s => s.StudentNumber == studentNumber);
            if (student == null)
            {
                MessageBox.Show("未能获取到学生详细信息。");
                return;
            }

            // 判断是否已有窗口打开，且是同一位学生
            if (trendForm != null && !trendForm.IsDisposed)
            {
                if (trendFormStudentNumber == studentNumber)
                {
                    trendForm.BringToFront();  // 已打开，前置
                    return;
                }
                else
                {
                    trendForm.Close();  // 不是同一学生，关闭旧窗体
                }
            }

            trendForm = new ScoreTrendForm(student);
            trendFormStudentNumber = studentNumber;
            trendForm.FormClosed += (s, args) =>
            {
                trendForm = null;
                trendFormStudentNumber = null;
            };
            trendForm.Show();
        }

        private void menu_critical_Click(object sender, EventArgs e)
        {
            new ScoreCriticalForm().ShowDialog();
        }

        private void menu_teacher_Click(object sender, EventArgs e)
        {
            new ScoreTeacherForm().ShowDialog();
        }


    }
}
