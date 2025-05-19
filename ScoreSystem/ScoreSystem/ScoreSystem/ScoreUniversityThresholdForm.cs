using ScoreSystem.Data;
using ScoreSystem.Model;
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
    public partial class ScoreUniversityThresholdForm : Form
    {
        private ScoreMainForm mainForm;
        private List<GradeThresholdPredictionResult> gradeThresholdPredictionResults;
        private bool isLoaded = false;
        public ScoreUniversityThresholdForm(ScoreMainForm mainForm)
        {
            this.mainForm = mainForm;
            InitializeComponent();
        }

        private void ScoreUniversityThresholdForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 上线管理";
            LoadCombobox();
        }

        private void comboBox_exam_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox_grade_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void menu_university_Click(object sender, EventArgs e)
        {
            new ScoreUniversityForm().ShowDialog();
        }

        private void button_back_Click(object sender, EventArgs e)
        {
            this.mainForm.Show();
            this.Dispose();
        }

        private void LoadCombobox()
        {
            this.comboBox_grade.DataSource = Enum.GetValues(typeof(GradeEnum))
                .Cast<GradeEnum>()
                .Select(g => new
                {
                    Name = g.ToString(),
                    Value = (int)g
                }).ToList();
            this.comboBox_exam.DisplayMember = "Name";
            this.comboBox_exam.ValueMember = "Value";
            isLoaded = true;
            LoadThreshold();
        }

        private void LoadThreshold()
        {
            
        }
    }
}
