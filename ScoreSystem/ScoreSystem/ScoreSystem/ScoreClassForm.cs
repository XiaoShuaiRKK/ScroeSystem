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
    public partial class ScoreClassForm : Form
    {
        private ScoreMainForm mainForm;
        public ScoreClassForm(ScoreMainForm mainForm)
        {
            this.mainForm = mainForm;
            InitializeComponent();
        }

        private void ScoreClassForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 班级管理";
        }

        private void menu_class_import_Click(object sender, EventArgs e)
        {

        }

        private void button_back_Click(object sender, EventArgs e)
        {
            this.mainForm.Show();
            this.Dispose();
        }
    }
}
