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
    public partial class ScoreStudentEditForm : Form
    {
        private Student student;
        private ClassService classService = ClassService.GetIntance();
        private StudentService studentService = StudentService.GetIntance();
        private List<ClassEntity> classEntities;
        public ScoreStudentEditForm(Student student)
        {
            this.student = student;
            InitializeComponent();
        }

        private void ScoreStudentEditForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 转班";
            textBox_name.Text = student.Name;
            textBox_number.Text = student.StudentNumber;
            textBox_name.Enabled = false;
            textBox_number.Enabled = false;
            comboBox_class.DropDownStyle = ComboBoxStyle.DropDownList; 
            ComboBoxLoad();
        }

        private async void ComboBoxLoad()
        {
            classEntities = await classService.GetAllClasses();
            comboBox_class.DataSource = classEntities;
            comboBox_class.DisplayMember = "Name";
            comboBox_class.ValueMember = "Id";
            comboBox_class.SelectedValue = student.ClassId;
        }

        private async void button_save_Click(object sender, EventArgs e)
        {
            int classId = (int)comboBox_class.SelectedValue;
            if(classId == student.ClassId)
            {
                this.Dispose();
                return;
            }
            using(var loading = new LoadForm())
            {
                await Task.Delay(100);
                loading.Show();
                Student s = new Student
                {
                    Name = student.Name,
                    StudentNumber = student.StudentNumber,
                    State = student.State,
                    SubjectGroupId = student.SubjectGroupId,
                    ElectiveCourse1Id = student.ElectiveCourse1Id,
                    ElectiveCourse2Id = student.ElectiveCourse2Id,
                    ClassId = classId
                };
                bool isSuccess = await studentService.UpdateStudent(s);
                if (isSuccess)
                {
                    MessageBox.Show("转班成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loading.Close();
                    this.Dispose();
                }
                loading.Close();
            }
                
        }
    }
}
