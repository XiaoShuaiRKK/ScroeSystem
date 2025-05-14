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
        private ClassService classService = new ClassService();
        private TeacherService teacherService = new TeacherService();
        private ScoreLoginForm loginForm;
        private User user;
        private List<Student> students;
        private bool isLoadClass = false;
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
    }
}
