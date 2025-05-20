namespace ScoreSystem
{
    partial class ScoreTeacherForm
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
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button_save = new System.Windows.Forms.Button();
            this.button_import = new System.Windows.Forms.Button();
            this.button_template = new System.Windows.Forms.Button();
            this.dataGridView_preview = new System.Windows.Forms.DataGridView();
            this.button_add = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView_exams = new System.Windows.Forms.DataGridView();
            this.textBox_name = new System.Windows.Forms.TextBox();
            this.textBox_username = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_password = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_number = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBox_state = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_preview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_exams)).BeginInit();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(42, 466);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 52;
            this.label6.Text = "所有教师";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(42, 205);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 51;
            this.label5.Text = "预览";
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(26, 431);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(729, 23);
            this.button_save.TabIndex = 50;
            this.button_save.Text = "保存";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_import
            // 
            this.button_import.Location = new System.Drawing.Point(497, 161);
            this.button_import.Name = "button_import";
            this.button_import.Size = new System.Drawing.Size(197, 23);
            this.button_import.TabIndex = 49;
            this.button_import.Text = "导入";
            this.button_import.UseVisualStyleBackColor = true;
            this.button_import.Click += new System.EventHandler(this.button_import_Click);
            // 
            // button_template
            // 
            this.button_template.Location = new System.Drawing.Point(250, 161);
            this.button_template.Name = "button_template";
            this.button_template.Size = new System.Drawing.Size(216, 23);
            this.button_template.TabIndex = 48;
            this.button_template.Text = "下载教师导入模板";
            this.button_template.UseVisualStyleBackColor = true;
            this.button_template.Click += new System.EventHandler(this.button_template_Click);
            // 
            // dataGridView_preview
            // 
            this.dataGridView_preview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_preview.Location = new System.Drawing.Point(26, 230);
            this.dataGridView_preview.Name = "dataGridView_preview";
            this.dataGridView_preview.RowTemplate.Height = 23;
            this.dataGridView_preview.Size = new System.Drawing.Size(729, 180);
            this.dataGridView_preview.TabIndex = 47;
            // 
            // button_add
            // 
            this.button_add.Location = new System.Drawing.Point(44, 161);
            this.button_add.Name = "button_add";
            this.button_add.Size = new System.Drawing.Size(184, 23);
            this.button_add.TabIndex = 46;
            this.button_add.Text = "添加";
            this.button_add.UseVisualStyleBackColor = true;
            this.button_add.Click += new System.EventHandler(this.button_add_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 43;
            this.label1.Text = "名字";
            // 
            // dataGridView_exams
            // 
            this.dataGridView_exams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_exams.Location = new System.Drawing.Point(26, 492);
            this.dataGridView_exams.Name = "dataGridView_exams";
            this.dataGridView_exams.RowTemplate.Height = 23;
            this.dataGridView_exams.Size = new System.Drawing.Size(729, 225);
            this.dataGridView_exams.TabIndex = 42;
            // 
            // textBox_name
            // 
            this.textBox_name.Location = new System.Drawing.Point(77, 26);
            this.textBox_name.Name = "textBox_name";
            this.textBox_name.Size = new System.Drawing.Size(200, 21);
            this.textBox_name.TabIndex = 53;
            // 
            // textBox_username
            // 
            this.textBox_username.Location = new System.Drawing.Point(89, 73);
            this.textBox_username.Name = "textBox_username";
            this.textBox_username.Size = new System.Drawing.Size(282, 21);
            this.textBox_username.TabIndex = 55;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 54;
            this.label2.Text = "用户名";
            // 
            // textBox_password
            // 
            this.textBox_password.Location = new System.Drawing.Point(444, 76);
            this.textBox_password.Name = "textBox_password";
            this.textBox_password.Size = new System.Drawing.Size(282, 21);
            this.textBox_password.TabIndex = 57;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(397, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 56;
            this.label3.Text = "密码";
            // 
            // textBox_number
            // 
            this.textBox_number.Location = new System.Drawing.Point(101, 115);
            this.textBox_number.Name = "textBox_number";
            this.textBox_number.Size = new System.Drawing.Size(270, 21);
            this.textBox_number.TabIndex = 59;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(42, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 58;
            this.label4.Text = "教师编号";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(397, 124);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 60;
            this.label7.Text = "状态";
            // 
            // comboBox_state
            // 
            this.comboBox_state.FormattingEnabled = true;
            this.comboBox_state.Location = new System.Drawing.Point(444, 121);
            this.comboBox_state.Name = "comboBox_state";
            this.comboBox_state.Size = new System.Drawing.Size(282, 20);
            this.comboBox_state.TabIndex = 61;
            // 
            // ScoreTeacherForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 751);
            this.Controls.Add(this.comboBox_state);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBox_number);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox_password);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox_username);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_name);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_import);
            this.Controls.Add(this.button_template);
            this.Controls.Add(this.dataGridView_preview);
            this.Controls.Add(this.button_add);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView_exams);
            this.Name = "ScoreTeacherForm";
            this.Text = "ScoreTeacherForm";
            this.Load += new System.EventHandler(this.ScoreTeacherForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_preview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_exams)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Button button_import;
        private System.Windows.Forms.Button button_template;
        private System.Windows.Forms.DataGridView dataGridView_preview;
        private System.Windows.Forms.Button button_add;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView_exams;
        private System.Windows.Forms.TextBox textBox_name;
        private System.Windows.Forms.TextBox textBox_username;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_password;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_number;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBox_state;
    }
}