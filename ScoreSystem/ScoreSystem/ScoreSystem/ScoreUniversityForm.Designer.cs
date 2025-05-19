namespace ScoreSystem
{
    partial class ScoreUniversityForm
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
            this.dataGridView_preview = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_university_name = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_level = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.num_scient_score_line = new System.Windows.Forms.NumericUpDown();
            this.num_art_score_line = new System.Windows.Forms.NumericUpDown();
            this.dtp_year = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.dataGridView_university = new System.Windows.Forms.DataGridView();
            this.button_import = new System.Windows.Forms.Button();
            this.button_template = new System.Windows.Forms.Button();
            this.button_add = new System.Windows.Forms.Button();
            this.button_save = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_preview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_scient_score_line)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_art_score_line)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_university)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView_preview
            // 
            this.dataGridView_preview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_preview.Location = new System.Drawing.Point(22, 229);
            this.dataGridView_preview.Name = "dataGridView_preview";
            this.dataGridView_preview.RowTemplate.Height = 23;
            this.dataGridView_preview.Size = new System.Drawing.Size(746, 144);
            this.dataGridView_preview.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "大学名称";
            // 
            // textBox_university_name
            // 
            this.textBox_university_name.Location = new System.Drawing.Point(97, 34);
            this.textBox_university_name.Name = "textBox_university_name";
            this.textBox_university_name.Size = new System.Drawing.Size(608, 21);
            this.textBox_university_name.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "大学等级";
            // 
            // comboBox_level
            // 
            this.comboBox_level.FormattingEnabled = true;
            this.comboBox_level.Location = new System.Drawing.Point(97, 65);
            this.comboBox_level.Name = "comboBox_level";
            this.comboBox_level.Size = new System.Drawing.Size(228, 20);
            this.comboBox_level.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "理科分数线";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(367, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "文科分数线";
            // 
            // num_scient_score_line
            // 
            this.num_scient_score_line.Location = new System.Drawing.Point(109, 101);
            this.num_scient_score_line.Name = "num_scient_score_line";
            this.num_scient_score_line.Size = new System.Drawing.Size(216, 21);
            this.num_scient_score_line.TabIndex = 8;
            // 
            // num_art_score_line
            // 
            this.num_art_score_line.Location = new System.Drawing.Point(438, 103);
            this.num_art_score_line.Name = "num_art_score_line";
            this.num_art_score_line.Size = new System.Drawing.Size(216, 21);
            this.num_art_score_line.TabIndex = 9;
            // 
            // dtp_year
            // 
            this.dtp_year.Location = new System.Drawing.Point(109, 135);
            this.dtp_year.Name = "dtp_year";
            this.dtp_year.Size = new System.Drawing.Size(216, 21);
            this.dtp_year.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 141);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 11;
            this.label5.Text = "年份";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(29, 205);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "预览";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(38, 423);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 13;
            this.label7.Text = "所有信息";
            // 
            // dataGridView_university
            // 
            this.dataGridView_university.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_university.Location = new System.Drawing.Point(22, 447);
            this.dataGridView_university.Name = "dataGridView_university";
            this.dataGridView_university.RowTemplate.Height = 23;
            this.dataGridView_university.Size = new System.Drawing.Size(746, 144);
            this.dataGridView_university.TabIndex = 14;
            // 
            // button_import
            // 
            this.button_import.Location = new System.Drawing.Point(508, 175);
            this.button_import.Name = "button_import";
            this.button_import.Size = new System.Drawing.Size(197, 23);
            this.button_import.TabIndex = 17;
            this.button_import.Text = "导入";
            this.button_import.UseVisualStyleBackColor = true;
            this.button_import.Click += new System.EventHandler(this.button_import_Click);
            // 
            // button_template
            // 
            this.button_template.Location = new System.Drawing.Point(261, 175);
            this.button_template.Name = "button_template";
            this.button_template.Size = new System.Drawing.Size(216, 23);
            this.button_template.TabIndex = 16;
            this.button_template.Text = "下载大学信息导入模板";
            this.button_template.UseVisualStyleBackColor = true;
            this.button_template.Click += new System.EventHandler(this.button_template_Click);
            // 
            // button_add
            // 
            this.button_add.Location = new System.Drawing.Point(55, 175);
            this.button_add.Name = "button_add";
            this.button_add.Size = new System.Drawing.Size(184, 23);
            this.button_add.TabIndex = 15;
            this.button_add.Text = "添加";
            this.button_add.UseVisualStyleBackColor = true;
            this.button_add.Click += new System.EventHandler(this.button_add_Click);
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(22, 388);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(746, 23);
            this.button_save.TabIndex = 18;
            this.button_save.Text = "保存";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // ScoreUniversityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 640);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_import);
            this.Controls.Add(this.button_template);
            this.Controls.Add(this.button_add);
            this.Controls.Add(this.dataGridView_university);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dtp_year);
            this.Controls.Add(this.num_art_score_line);
            this.Controls.Add(this.num_scient_score_line);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox_level);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox_university_name);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView_preview);
            this.Name = "ScoreUniversityForm";
            this.Text = "ScoreUniversityForm";
            this.Load += new System.EventHandler(this.ScoreUniversityForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_preview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_scient_score_line)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_art_score_line)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_university)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView_preview;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_university_name;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_level;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown num_scient_score_line;
        private System.Windows.Forms.NumericUpDown num_art_score_line;
        private System.Windows.Forms.DateTimePicker dtp_year;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridView dataGridView_university;
        private System.Windows.Forms.Button button_import;
        private System.Windows.Forms.Button button_template;
        private System.Windows.Forms.Button button_add;
        private System.Windows.Forms.Button button_save;
    }
}