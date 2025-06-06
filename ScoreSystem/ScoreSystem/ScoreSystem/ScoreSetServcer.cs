using ScoreSystem.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScoreSystem
{
    public partial class ScoreSetServcer : Form
    {
        private string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
        private FormAutoScaler autoScaler;

        public ScoreSetServcer()
        {
            InitializeComponent();
            autoScaler = new FormAutoScaler(this);
        }

        private void ScoreSetServcer_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 设置服务器";

            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");

            if (File.Exists(configPath))
            {
                try
                {
                    string serverAddr = File.ReadAllText(configPath).Trim(); // 例如：192.168.1.100:8080
                    if (!string.IsNullOrWhiteSpace(serverAddr))
                    {
                        textBox_server.Text = serverAddr;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("读取服务器地址失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close(); // 或 this.Dispose();
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            string input = textBox_server.Text.Trim();
            if (!string.IsNullOrEmpty(input))
            {
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
                File.WriteAllText(configPath, input);
                HttpUtil.LoadBaseUrl(); // 重新加载新地址
                MessageBox.Show("服务器地址已保存", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("请输入服务器地址", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
