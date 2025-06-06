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
    public partial class ScoreClassEditForm : Form
    {
        private FormAutoScaler autoScaler;
        private ClassEntity classEntity;
        private ClassEntity needUpdateClass;
        private List<TeacherVO> teachers;
        private TeacherService teacherService = TeacherService.GetIntance();
        private ClassService classService = ClassService.GetIntance();
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
            this.textBox_class_name.Text = classEntity.Name;
            this.comboBox_teacher.DropDownStyle = ComboBoxStyle.DropDownList;
            ControlsLoad();
        }

        private async void ControlsLoad()
        {
            teachers = await teacherService.GetTeachers();
            // 过滤掉当前班主任
            var filteredTeachers = teachers
                .ToList();

            comboBox_teacher.DataSource = filteredTeachers;
            comboBox_teacher.DisplayMember = "Name";
            comboBox_teacher.ValueMember = "Id";
        }

        private async void button_edit_Click(object sender, EventArgs e)
        {
            int id = (int)comboBox_teacher.SelectedValue;
            needUpdateClass = classEntity;
            needUpdateClass.HeadTeacherId = id;
            bool isSuccess = await classService.UpdateClass(classEntity);
            if (isSuccess)
            {
                MessageBox.Show("更新成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Dispose();
            }
        }
    }
}
