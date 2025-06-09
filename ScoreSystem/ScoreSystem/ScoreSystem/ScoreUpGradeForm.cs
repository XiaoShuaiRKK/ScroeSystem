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
    public partial class ScoreUpGradeForm : Form
    {
        private List<ClassEntity> classEntities;
        private ClassService classService = ClassService.GetIntance();
        public ScoreUpGradeForm()
        {
            InitializeComponent();
        }

        private void ScoreUpGradeForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 升班";
            this.comboBox_grade.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBoxLoad();
        }

        private async void ComboBoxLoad()
        {
            classEntities = await classService.GetAllClasses();

            // 获取班级中的所有 grade，并去重
            var availableGrades = classEntities
                .Select(c => c.Grade)
                .Distinct()
                .OrderBy(g => g) // 按年级顺序排列
                .ToList();

            // 创建绑定项：显示年级名称，值为年级ID
            var gradeItems = availableGrades
                .Select(g => new
                {
                    GradeValue = g,
                    GradeName = ((GradeEnum)g).ToString()
                })
                .ToList();

            comboBox_grade.DataSource = gradeItems;
            comboBox_grade.DisplayMember = "GradeName";
            comboBox_grade.ValueMember = "GradeValue";
        }

        private async void button_to_update_Click(object sender, EventArgs e)
        {
            int grade = (int)comboBox_grade.SelectedValue;
            // 第一次确认
            var confirmResult = MessageBox.Show(
                $"确定要对年级为『{(GradeEnum)grade}』的所有班级进行升班操作吗？",
                "升班确认",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmResult != DialogResult.Yes)
                return;

            // 第二次确认
            var doubleCheck = MessageBox.Show(
                "请再次确认是否执行升班操作？该操作不可撤销！",
                "最终确认",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (doubleCheck != DialogResult.Yes)
                return;
            using (var loading = new LoadForm())
            {
                loading.Show();
                await Task.Delay(100);
                bool isSuccess = await classService.UpGradeClass(grade);
                loading.Close();
            }
        }
    }
}
