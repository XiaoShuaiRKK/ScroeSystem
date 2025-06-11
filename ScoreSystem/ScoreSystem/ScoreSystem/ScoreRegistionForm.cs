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
    public partial class ScoreRegistionForm : Form
    {
        private FormAutoScaler autoScaler;
        private ScoreLoginForm loginForm;
        private UserService userService = UserService.GetIntance();
        public ScoreRegistionForm(ScoreLoginForm loginForm)
        {
            this.loginForm = loginForm;
            InitializeComponent();
            autoScaler = new FormAutoScaler(this);
        }

        private void ScoreRegistionForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 注册";
            this.FormClosed += (s, ex) =>
            {
                loginForm.Show();
                this.Dispose();
            };
            comboBox_role.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_role.DataSource = Enum.GetValues(typeof(UserRoleEnum))
                .Cast<UserRoleEnum>()
                .Where(r => r == UserRoleEnum.管理员 || r == UserRoleEnum.主任)
                .Select(r => new
                {
                    Name = r.ToString(),
                    Value = (int)r
                }).ToList();
            comboBox_role.DisplayMember = "Name";
            comboBox_role.ValueMember = "Value";
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            loginForm.Show();
            this.Dispose();
        }

        private async void button_register_Click(object sender, EventArgs e)
        {
            using(var loading = new LoadForm())
            {
                await Task.Delay(100);
                loading.Show();
                string name = textBox_name.Text.Trim();
                string username = textBox_username.Text.Trim();
                string password = textBox_password.Text.Trim();
                string checkPassword = textBox_password.Text.Trim();
                int role;

                // 校验：姓名不能为空
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show("姓名不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 校验：用户名不能为空
                if (string.IsNullOrWhiteSpace(username))
                {
                    MessageBox.Show("用户名不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 校验：密码不能为空
                if (string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show("密码不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 校验：密码长度
                if (password.Length < 6)
                {
                    MessageBox.Show("密码长度不能少于6位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 校验：两次密码一致
                if (password != checkPassword)
                {
                    MessageBox.Show("两次密码输入不一致", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 校验：角色是否选择
                if (comboBox_role.SelectedItem == null || !int.TryParse(comboBox_role.SelectedValue.ToString(), out role))
                {
                    MessageBox.Show("请选择角色", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                RegisterRequest register = new RegisterRequest
                {
                    Name = name,
                    Password = password,
                    Username = username,
                    Level = 1,
                    Role = role
                };

                bool isSuccess = await userService.Register(register);
                if (isSuccess)
                {
                    MessageBox.Show("注册成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    loading.Close();
                    loginForm.Show();
                    this.Dispose();
                    return;
                }

                loading.Close();
            }
            
        }
    }
}
