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
    public partial class ScoreRegistionForm : Form
    {
        private ScoreLoginForm loginForm;
        public ScoreRegistionForm(ScoreLoginForm loginForm)
        {
            this.loginForm = loginForm;
            InitializeComponent();
        }

        private void ScoreRegistionForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 注册";
            comboBox_role.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            loginForm.Show();
            this.Dispose();
        }

        private void button_register_Click(object sender, EventArgs e)
        {

        }
    }
}
