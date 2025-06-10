using NPOI.OpenXmlFormats.Spreadsheet;
using ScoreSystem.Data;
using ScoreSystem.Model;
using ScoreSystem.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScoreSystem
{
    public partial class ScoreClassEditForm : Form
    {
        private FormAutoScaler autoScaler;
        private ClassEntity classEntity;
        private ClassEntity needUpdateClass;
        private List<TeacherVO> teachers;
        private TeacherService teacherService = TeacherService.GetIntance();
        private ClassService classService = ClassService.GetIntance();
        private List<Student> students;
        public ScoreClassEditForm(ClassEntity classEntity)
        {
            this.classEntity = classEntity;
            InitializeComponent();
            autoScaler = new FormAutoScaler(this);
        }

        private void ScoreClassEditForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 设置班级";
            this.textBox_taecher_name.Enabled = false;
            this.textBox_taecher_name.Text = classEntity.TeacherName;
            this.textBox_subjectGroup.Enabled = false;
            this.textBox_subjectGroup.Text = ((SubjectGroupEnum)classEntity.SubjectGroupId).ToString();
            this.textBox_class_name.Text = classEntity.Name;
            this.comboBox_teacher.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_subjectGroup.DropDownStyle = ComboBoxStyle.DropDownList;
            this.dataGridView_students.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView_students.ReadOnly = true;
            this.dataGridView_students.MultiSelect = false;
            ControlsLoad();
            LoadData();
        }

        private async void LoadData()
        {
            students = await teacherService.GetStudentByClassId(classEntity.Id);
            var displayStudent = students.Select(s => new
            {
                学号 = s.StudentNumber,
                姓名 = s.Name
            }).ToList();
            this.dataGridView_students.DataSource = displayStudent;
        }

        private async void ControlsLoad()
        {
            teachers = await teacherService.GetTeachers();
            // 过滤掉当前班主任
            var filteredTeachers = teachers
                .Where(t => t.Id != classEntity.HeadTeacherId)
                .ToList();

            comboBox_teacher.DataSource = filteredTeachers;
            comboBox_teacher.DisplayMember = "Name";
            comboBox_teacher.ValueMember = "Id";

            comboBox_subjectGroup.DataSource = Enum.GetValues(typeof(SubjectGroupEnum))
                .Cast<SubjectGroupEnum>()
                .Where(s => s != SubjectGroupEnum.未分组)
                .Select(g => new { Name = g.ToString(), Value = (int)g })
                .ToList();
            comboBox_subjectGroup.DisplayMember = "Name";
            comboBox_subjectGroup.ValueMember = "Value";
        }

        private async void button_edit_Click(object sender, EventArgs e)
        {
            string name = textBox_class_name.Text.Trim();
            long teacherId = (long)comboBox_teacher.SelectedValue;
            int subjectGroupId = (int)comboBox_subjectGroup.SelectedValue;

            // 如果没有修改任何内容，就直接关闭窗口
            if (classEntity.HeadTeacherId == teacherId
                && classEntity.SubjectGroupId == subjectGroupId
                && classEntity.Name.Equals(name, StringComparison.Ordinal))
            {
                this.Dispose();
                return;
            }

            using (var loading = new LoadForm())
            {
                try
                {
                    loading.Show();
                    await Task.Delay(100); // 确保加载窗体显示

                    // 直接修改 classEntity 的属性
                    classEntity.HeadTeacherId = teacherId;
                    classEntity.SubjectGroupId = subjectGroupId;
                    classEntity.Name = name;

                    bool isSuccess = await classService.UpdateClass(classEntity);

                    if (isSuccess)
                    {
                        MessageBox.Show("更新成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Dispose();
                    }
                    else
                    {
                        MessageBox.Show("更新失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                finally
                {
                    loading.Close();
                }
            }

        }

        private async void dataGridView_students_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridView_students.CurrentRow != null && dataGridView_students.CurrentRow.Index >= 0)
            {
                var selectedRow = dataGridView_students.CurrentRow;

                // 确保“学号”列存在且值为字符串
                if (selectedRow.Cells["学号"].Value is string studentNumber)
                {
                    Student student = await teacherService.GetStudent(studentNumber);
                    ScoreTrendFormManage.ShowTrendForm(student);
                }
            }
        }

        private void button_cancle_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
