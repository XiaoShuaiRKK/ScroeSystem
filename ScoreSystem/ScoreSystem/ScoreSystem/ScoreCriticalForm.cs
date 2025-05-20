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
    public partial class ScoreCriticalForm : Form
    {
        public ScoreCriticalForm()
        {
            InitializeComponent();
        }

        private void ScoreCriticalForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 临界生管理";
        }

        private void menu_critical_config_Click(object sender, EventArgs e)
        {
            new ScoreCriticalConfigForm().ShowDialog();
        }
    }
}
