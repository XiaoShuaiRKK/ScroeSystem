namespace ScoreSystem
{
    partial class ScoreLoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_username = new System.Windows.Forms.TextBox();
            this.textBox_password = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button_login = new System.Windows.Forms.Button();
            this.linkLabel_forget_password = new System.Windows.Forms.LinkLabel();
            this.button_register = new System.Windows.Forms.Button();
            this.linkLabel_server = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 16F);
            this.label1.Location = new System.Drawing.Point(121, 98);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "用户名:";
            // 
            // textBox_username
            // 
            this.textBox_username.Font = new System.Drawing.Font("宋体", 14F);
            this.textBox_username.Location = new System.Drawing.Point(229, 97);
            this.textBox_username.Name = "textBox_username";
            this.textBox_username.Size = new System.Drawing.Size(373, 29);
            this.textBox_username.TabIndex = 1;
            // 
            // textBox_password
            // 
            this.textBox_password.Font = new System.Drawing.Font("宋体", 14F);
            this.textBox_password.Location = new System.Drawing.Point(229, 186);
            this.textBox_password.Name = "textBox_password";
            this.textBox_password.PasswordChar = '·';
            this.textBox_password.Size = new System.Drawing.Size(373, 29);
            this.textBox_password.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 16F);
            this.label2.Location = new System.Drawing.Point(121, 187);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 22);
            this.label2.TabIndex = 2;
            this.label2.Text = "密码:";
            // 
            // button_login
            // 
            this.button_login.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_login.Location = new System.Drawing.Point(437, 321);
            this.button_login.Name = "button_login";
            this.button_login.Size = new System.Drawing.Size(106, 44);
            this.button_login.TabIndex = 4;
            this.button_login.Text = "登录";
            this.button_login.UseVisualStyleBackColor = true;
            this.button_login.Click += new System.EventHandler(this.button_login_Click);
            // 
            // linkLabel_forget_password
            // 
            this.linkLabel_forget_password.AutoSize = true;
            this.linkLabel_forget_password.Location = new System.Drawing.Point(537, 265);
            this.linkLabel_forget_password.Name = "linkLabel_forget_password";
            this.linkLabel_forget_password.Size = new System.Drawing.Size(65, 12);
            this.linkLabel_forget_password.TabIndex = 6;
            this.linkLabel_forget_password.TabStop = true;
            this.linkLabel_forget_password.Text = "忘记密码？";
            // 
            // button_register
            // 
            this.button_register.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_register.Location = new System.Drawing.Point(260, 321);
            this.button_register.Name = "button_register";
            this.button_register.Size = new System.Drawing.Size(106, 44);
            this.button_register.TabIndex = 7;
            this.button_register.Text = "注册";
            this.button_register.UseVisualStyleBackColor = true;
            this.button_register.Click += new System.EventHandler(this.button_register_Click);
            // 
            // linkLabel_server
            // 
            this.linkLabel_server.AutoSize = true;
            this.linkLabel_server.Location = new System.Drawing.Point(143, 265);
            this.linkLabel_server.Name = "linkLabel_server";
            this.linkLabel_server.Size = new System.Drawing.Size(65, 12);
            this.linkLabel_server.TabIndex = 8;
            this.linkLabel_server.TabStop = true;
            this.linkLabel_server.Text = "设置服务器";
            this.linkLabel_server.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_server_LinkClicked);
            // 
            // ScoreLoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(826, 476);
            this.Controls.Add(this.linkLabel_server);
            this.Controls.Add(this.button_register);
            this.Controls.Add(this.linkLabel_forget_password);
            this.Controls.Add(this.button_login);
            this.Controls.Add(this.textBox_password);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_username);
            this.Controls.Add(this.label1);
            this.Name = "ScoreLoginForm";
            this.Load += new System.EventHandler(this.ScoreLoginForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_username;
        private System.Windows.Forms.TextBox textBox_password;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_login;
        private System.Windows.Forms.LinkLabel linkLabel_forget_password;
        private System.Windows.Forms.Button button_register;
        private System.Windows.Forms.LinkLabel linkLabel_server;
    }
}