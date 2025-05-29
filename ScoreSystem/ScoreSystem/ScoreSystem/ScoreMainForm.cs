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
        private StudentService studentService = StudentService.GetIntance();
        private ScoreLoginForm loginForm;
        private User user;
        private List<Student> students;
        private bool isLoadClass = false;
        private ScoreTrendForm trendForm = null;
        private string trendFormStudentNumber = null;
        private bool isEdit = false;
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
            ExitEditModeIfNeeded();
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
            if (classEntities.Any())
            {
                isLoadClass = true;
            }
            StudentDataInit();
        }

        private void comboBox_class_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoadClass)
            {
                ExitEditModeIfNeeded();
                StudentDataInit();
            }
        }

        private async void StudentDataInit()
        {
            if (!isLoadClass) return;
            int selectedId = (int)comboBox_class.SelectedValue;
            students = await teacherService.GetStudentByClassId(selectedId);
            if (students == null || !students.Any()) return;

            dataGridView_students.DataSource = null;
            dataGridView_students.Columns.Clear();
            dataGridView_students.DataSource = students;

            // 设置列头名
            dataGridView_students.Columns["StudentNumber"].HeaderText = "学号";
            dataGridView_students.Columns["Name"].HeaderText = "姓名";
            dataGridView_students.Columns["ElectiveCourse1Id"].HeaderText = "选科一";
            dataGridView_students.Columns["ElectiveCourse2Id"].HeaderText = "选科二";
            dataGridView_students.Columns["EnrollmentDate"].HeaderText = "入学时间";

            // 隐藏不需要显示的列（如主键等）
            foreach (DataGridViewColumn col in dataGridView_students.Columns)
            {
                if (col.Name != "StudentNumber" && col.Name != "Name" &&
                    col.Name != "ElectiveCourse1Id" && col.Name != "ElectiveCourse2Id" &&
                    col.Name != "EnrollmentDate")
                {
                    col.Visible = false;
                }
            }

            // 格式化课程 ID 为名称
            dataGridView_students.CellFormatting += (s, e) =>
            {
                string colName = dataGridView_students.Columns[e.ColumnIndex].Name;
                if ((colName == "ElectiveCourse1Id" || colName == "ElectiveCourse2Id") &&
                    e.Value is int courseId)
                {
                    e.Value = CourseHelper.GetCourseNameById(courseId);
                    e.FormattingApplied = true;
                }
            };

            // 添加删除按钮列
            if (!dataGridView_students.Columns.Contains("操作"))
            {
                DataGridViewLinkColumn deleteColumn = new DataGridViewLinkColumn
                {
                    Name = "操作",
                    HeaderText = "操作",
                    Text = "删除",
                    UseColumnTextForLinkValue = true
                };
                dataGridView_students.Columns.Add(deleteColumn);
            }
        }

        private void menu_class_or_student_Click(object sender, EventArgs e)
        {
            ExitEditModeIfNeeded();
            new ScoreClassOrStudentOperateForm().ShowDialog();
            StudentDataInit();
        }

        private void menu_score_Click(object sender, EventArgs e)
        {
            ExitEditModeIfNeeded();
            new ScoreScoreForm(this).Show();
            this.Hide();
        }

        private void menu_exam_Click(object sender, EventArgs e)
        {
            ExitEditModeIfNeeded();
            new ScoreExamForm().ShowDialog();
        }

        private void menu_university_Click(object sender, EventArgs e)
        {
            ExitEditModeIfNeeded();
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
            ExitEditModeIfNeeded();
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
            ExitEditModeIfNeeded();
            new ScoreCriticalForm().ShowDialog();
        }

        private void menu_teacher_Click(object sender, EventArgs e)
        {
            ExitEditModeIfNeeded();
            new ScoreTeacherForm().ShowDialog();
        }

        private void menu_class_Click(object sender, EventArgs e)
        {
            ExitEditModeIfNeeded();
            new ScoreClassForm(this).Show();
            this.Hide();
        }

        private async void button_edit_Click(object sender, EventArgs e)
        {
            if (!isEdit)
            {
                dataGridView_students.ReadOnly = false;
                dataGridView_students.SelectionMode = DataGridViewSelectionMode.CellSelect;
                button_edit.Text = "保存";

                var courseNames = CourseHelper.GetAllCourseNames();

                ReplaceColumn(dataGridView_students, "ElectiveCourse1Id", new DataGridViewComboBoxColumn
                {
                    Name = "ElectiveCourse1Id",
                    HeaderText = "选科一",
                    DataSource = courseNames,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
                });

                ReplaceColumn(dataGridView_students, "ElectiveCourse2Id", new DataGridViewComboBoxColumn
                {
                    Name = "ElectiveCourse2Id",
                    HeaderText = "选科二",
                    DataSource = courseNames,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox
                });
            }
            else
            {
                button_edit.Enabled = false;
                // 保存逻辑
                var updatedStudents = new List<Student>();
                for (int i = 0; i < dataGridView_students.Rows.Count; i++)
                {
                    var row = dataGridView_students.Rows[i];

                    string studentNumber = row.Cells["StudentNumber"].Value?.ToString();
                    var originalStudent = students.FirstOrDefault(s => s.StudentNumber == studentNumber);
                    if (originalStudent == null) continue;

                    string name = row.Cells["Name"].Value?.ToString();
                    string electiveCourse1Name = row.Cells["ElectiveCourse1Id"].Value?.ToString();
                    string electiveCourse2Name = row.Cells["ElectiveCourse2Id"].Value?.ToString();
                    DateTime.TryParse(row.Cells["EnrollmentDate"].Value?.ToString(), out DateTime enrollmentDate);

                    int electiveCourse1Id = CourseHelper.GetCourseIdByName(electiveCourse1Name);
                    int electiveCourse2Id = CourseHelper.GetCourseIdByName(electiveCourse2Name);

                    if (originalStudent.Name != name ||
                        originalStudent.ElectiveCourse1Id != electiveCourse1Id ||
                        originalStudent.ElectiveCourse2Id != electiveCourse2Id ||
                        originalStudent.EnrollmentDate != enrollmentDate)
                    {
                        originalStudent.Name = name;
                        originalStudent.ElectiveCourse1Id = electiveCourse1Id;
                        originalStudent.ElectiveCourse2Id = electiveCourse2Id;
                        originalStudent.EnrollmentDate = enrollmentDate;
                        updatedStudents.Add(originalStudent);
                    }
                }

                if (updatedStudents.Any())
                {
                    bool success = false;
                    using (var loading = new LoadForm())
                    {
                        loading.Show();
                        await Task.Delay(100);
                        success = await studentService.BatchUpdateStudent(students);
                        loading.Close();
                    }
                    if (success)
                    {
                        MessageBox.Show("更新完成", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                dataGridView_students.ReadOnly = true;
                dataGridView_students.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                button_edit.Text = "修改";
                button_edit.Enabled = true;
                StudentDataInit(); // 刷新数据
            }

            isEdit = !isEdit;
        }

        private async void dataGridView_students_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var grid = sender as DataGridView;
            if (grid.Columns[e.ColumnIndex].Name == "操作")
            {
                // 获取选中行的学号
                string studentNumber = grid.Rows[e.RowIndex].Cells["学号"].Value?.ToString();

                if (string.IsNullOrEmpty(studentNumber)) return;

                var confirm = MessageBox.Show("确定要删除该学生吗？", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    using (var loading = new LoadForm())
                    {
                        loading.Show();
                        await Task.Delay(100);
                        var studentToDelete = students.FirstOrDefault(s => s.StudentNumber == studentNumber);
                        loading.Close();
                        if (studentToDelete != null)
                        {
                            bool success = await studentService.DeleteStudent(studentToDelete); // 你需要实现此方法
                            if (success)
                            {
                                MessageBox.Show("删除成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                students.Remove(studentToDelete);
                                StudentDataInit(); // 重新加载数据
                            }
                            else
                            {
                                MessageBox.Show("删除失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                    }
                }
            }
        }

        private void ReplaceColumn(DataGridView dgv, string columnName, DataGridViewComboBoxColumn newColumn)
        {
            int index = dgv.Columns[columnName].Index;
            newColumn.Width = dgv.Columns[columnName].Width;

            List<object> oldValues = new List<object>();
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                oldValues.Add(dgv.Rows[i].Cells[columnName].Value);
            }

            dgv.Columns.RemoveAt(index);
            dgv.Columns.Insert(index, newColumn);

            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                dgv.Rows[i].Cells[index].Value = CourseHelper.GetCourseNameById(Convert.ToInt32(oldValues[i]));
            }
        }

        private void ExitEditModeIfNeeded()
        {
            if (isEdit)
            {
                dataGridView_students.ReadOnly = true;
                dataGridView_students.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                button_edit.Text = "修改";
                button_edit.Enabled = true;
                isEdit = false;
                StudentDataInit(); // 刷新数据回到正常状态
            }
        }
    }
}
