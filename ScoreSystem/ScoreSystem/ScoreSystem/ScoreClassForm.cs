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
        private ScoreUpGradeForm gradeForm;
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
            AddDeleteButtonColumn(); // 添加删除按钮列
        }

        private void AddDeleteButtonColumn()
        {
            if (!dataGridView_class.Columns.Contains("DeleteLinkColumn"))
            {
                DataGridViewLinkColumn linkColumn = new DataGridViewLinkColumn();
                linkColumn.Name = "DeleteLinkColumn";
                linkColumn.HeaderText = "操作";
                linkColumn.Text = "删除";
                linkColumn.UseColumnTextForLinkValue = true;
                linkColumn.LinkBehavior = LinkBehavior.HoverUnderline;
                linkColumn.Width = 60;
                dataGridView_class.Columns.Add(linkColumn);
            }
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

        private async void dataGridView_class_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 点击的是“删除”按钮列
            if (dataGridView_class.Columns[e.ColumnIndex].Name == "DeleteColumn" && e.RowIndex >= 0)
            {
                string className = dataGridView_class.Rows[e.RowIndex].Cells["班级名称"].Value.ToString();
                var result = MessageBox.Show($"确定要删除班级【{className}】吗？", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    var classEntity = classEntities[e.RowIndex];
                    bool success = await classService.DeleteClass(classEntity);
                    if (success)
                    {
                        MessageBox.Show("删除成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("删除失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void menu_up_grade_Click(object sender, EventArgs e)
        {
            if (gradeForm == null)
            {
                gradeForm = new ScoreUpGradeForm();
                gradeForm.Show();
                gradeForm.FormClosed += (s, ex) =>
                {
                    gradeForm = null;
                    LoadData();
                };
            }
            else
            {
                gradeForm.BringToFront();
            }
        }
    }
}
