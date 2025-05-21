using ScoreSystem.Data;
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
    public partial class ScoreLoginForm : Form
    {
        private UserService userService = UserService.GetIntance();
        public ScoreLoginForm()
        {
            InitializeComponent();
        }

        private void ScoreLoginForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 登录";
            this.AcceptButton = button_login;
        }

        private void button_register_Click(object sender, EventArgs e)
        {
            this.Hide();
            new ScoreRegistionForm(this).ShowDialog();
        }

        private async void button_login_Click(object sender, EventArgs e)
        {
            this.button_login.Enabled = false;
            string username = textBox_username.Text;
            string password = textBox_password.Text;
            bool isSuccess = await userService.Login(username, password);
            if (isSuccess)
            {
                new ScoreMainForm(this).Show();
                this.Hide();
            }
            this.button_login.Enabled = true;
        }

        private void linkLabel_server_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new ScoreSetServcer().ShowDialog();
        }
    }
}
