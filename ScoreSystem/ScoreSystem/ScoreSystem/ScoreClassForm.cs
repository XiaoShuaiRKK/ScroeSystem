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
    public partial class ScoreClassForm : Form
    {
        private FormAutoScaler autoScaler;
        private ScoreMainForm mainForm;
        private ClassService classService = ClassService.GetIntance();
        private List<ClassEntity> classEntities;
        public ScoreClassForm(ScoreMainForm mainForm)
        {
            this.mainForm = mainForm;
            InitializeComponent();
            autoScaler = new FormAutoScaler(this);
        }

        private void ScoreClassForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 班级管理";
            this.FormClosed += (s, ex) =>
            {
                this.mainForm.Show();
                this.Dispose();
            };
            // 设置 DataGridView 属性
            dataGridView_class.ReadOnly = true;
            dataGridView_class.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView_class.MultiSelect = false;
            dataGridView_class.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            LoadData();
        }

        private async void LoadData()
        {
            classEntities = await classService.GetAllClasses();
            this.dataGridView_class.DataSource = classEntities.Select(c => new
            {
                班级名称 = c.Name,
                年级 = ((GradeEnum)c.Grade).ToString(),
                学科组 = ((SubjectGroupEnum)c.SubjectGroupId).ToString(),
                班主任 = c.TeacherName
            }).ToList();
        }

        private void menu_class_import_Click(object sender, EventArgs e)
        {
            new ScoreClassImportForm().ShowDialog();
            LoadData();
        }

        private void button_back_Click(object sender, EventArgs e)
        {
            this.mainForm.Show();
            this.Dispose();
        }

        private void menu_class_edit_Click(object sender, EventArgs e)
        {
            if (dataGridView_class.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择一条班级记录", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int index = dataGridView_class.SelectedRows[0].Index;
            if (index < 0 || index >= classEntities.Count)
            {
                MessageBox.Show("选中行索引无效", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ClassEntity selectedClass = classEntities[index];
            new ScoreClassEditForm(selectedClass).ShowDialog();

            // 可选：编辑完后刷新数据
            LoadData();
        }
    }
}
