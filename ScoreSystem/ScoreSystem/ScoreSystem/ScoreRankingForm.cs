using ScoreSystem.Data;
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
    public partial class ScoreRankingForm : Form
    {
        private FormAutoScaler autoScaler;
        private ClassRankingControlForm classRankingControl;
        private GradeRankingControlForm gradeRankingControl;

        public ScoreRankingForm()
        {
            InitializeComponent();
            autoScaler = new FormAutoScaler(this);
        }

        private void ScoreRankingForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 排行榜";
        }

        private void LoadUserControl(int mode)
        {
            panel_show.Controls.Clear();
            UserControl control = null;
            if (mode == 1)
            {
                if (classRankingControl == null)
                    classRankingControl = new ClassRankingControlForm();
                control = classRankingControl;
            }
            else
            {
                if (gradeRankingControl == null)
                    gradeRankingControl = new GradeRankingControlForm();
                control = gradeRankingControl;
            }
            control.Dock = DockStyle.Fill;
            panel_show.Controls.Add(control);
        }

        private void button_class_Click(object sender, EventArgs e)
        {
            LoadUserControl(1);
        }

        private void button_grade_Click(object sender, EventArgs e)
        {
            LoadUserControl(2);
        }
    }
}
