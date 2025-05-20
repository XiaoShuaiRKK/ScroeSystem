namespace ScoreSystem
{
    partial class ScoreCriticalConfigForm
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
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox_grade = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView_exams = new System.Windows.Forms.DataGridView();
            this.dtp_year = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_university_level = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.num_target_count = new System.Windows.Forms.NumericUpDown();
            this.num_critical_ratio = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBox_subject_group = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_preview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_exams)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_target_count)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_critical_ratio)).BeginInit();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(39, 459);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 31;
            this.label6.Text = "所有配置";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(39, 198);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 30;
            this.label5.Text = "预览";
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(23, 424);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(729, 23);
            this.button_save.TabIndex = 29;
            this.button_save.Text = "保存";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_import
            // 
            this.button_import.Location = new System.Drawing.Point(494, 154);
            this.button_import.Name = "button_import";
            this.button_import.Size = new System.Drawing.Size(197, 23);
            this.button_import.TabIndex = 28;
            this.button_import.Text = "导入";
            this.button_import.UseVisualStyleBackColor = true;
            this.button_import.Click += new System.EventHandler(this.button_import_Click);
            // 
            // button_template
            // 
            this.button_template.Location = new System.Drawing.Point(247, 154);
            this.button_template.Name = "button_template";
            this.button_template.Size = new System.Drawing.Size(216, 23);
            this.button_template.TabIndex = 27;
            this.button_template.Text = "下载临界设置导入模板";
            this.button_template.UseVisualStyleBackColor = true;
            this.button_template.Click += new System.EventHandler(this.button_template_Click);
            // 
            // dataGridView_preview
            // 
            this.dataGridView_preview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_preview.Location = new System.Drawing.Point(23, 223);
            this.dataGridView_preview.Name = "dataGridView_preview";
            this.dataGridView_preview.RowTemplate.Height = 23;
            this.dataGridView_preview.Size = new System.Drawing.Size(729, 180);
            this.dataGridView_preview.TabIndex = 26;
            // 
            // button_add
            // 
            this.button_add.Location = new System.Drawing.Point(41, 154);
            this.button_add.Name = "button_add";
            this.button_add.Size = new System.Drawing.Size(184, 23);
            this.button_add.TabIndex = 25;
            this.button_add.Text = "添加";
            this.button_add.UseVisualStyleBackColor = true;
            this.button_add.Click += new System.EventHandler(this.button_add_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(383, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 24;
            this.label4.Text = "年级";
            // 
            // comboBox_grade
            // 
            this.comboBox_grade.FormattingEnabled = true;
            this.comboBox_grade.Location = new System.Drawing.Point(418, 20);
            this.comboBox_grade.Name = "comboBox_grade";
            this.comboBox_grade.Size = new System.Drawing.Size(200, 20);
            this.comboBox_grade.TabIndex = 23;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 18;
            this.label1.Text = "年份";
            // 
            // dataGridView_exams
            // 
            this.dataGridView_exams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_exams.Location = new System.Drawing.Point(23, 485);
            this.dataGridView_exams.Name = "dataGridView_exams";
            this.dataGridView_exams.RowTemplate.Height = 23;
            this.dataGridView_exams.Size = new System.Drawing.Size(729, 212);
            this.dataGridView_exams.TabIndex = 16;
            // 
            // dtp_year
            // 
            this.dtp_year.Location = new System.Drawing.Point(85, 19);
            this.dtp_year.Name = "dtp_year";
            this.dtp_year.Size = new System.Drawing.Size(216, 21);
            this.dtp_year.TabIndex = 32;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 33;
            this.label2.Text = "大学等级";
            // 
            // comboBox_university_level
            // 
            this.comboBox_university_level.FormattingEnabled = true;
            this.comboBox_university_level.Location = new System.Drawing.Point(98, 62);
            this.comboBox_university_level.Name = "comboBox_university_level";
            this.comboBox_university_level.Size = new System.Drawing.Size(216, 20);
            this.comboBox_university_level.TabIndex = 34;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 36;
            this.label3.Text = "人数";
            // 
            // num_target_count
            // 
            this.num_target_count.Location = new System.Drawing.Point(98, 104);
            this.num_target_count.Name = "num_target_count";
            this.num_target_count.Size = new System.Drawing.Size(97, 21);
            this.num_target_count.TabIndex = 37;
            // 
            // num_critical_ratio
            // 
            this.num_critical_ratio.Location = new System.Drawing.Point(315, 106);
            this.num_critical_ratio.Name = "num_critical_ratio";
            this.num_critical_ratio.Size = new System.Drawing.Size(97, 21);
            this.num_critical_ratio.TabIndex = 39;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(256, 108);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 38;
            this.label7.Text = "比例";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(475, 113);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 12);
            this.label8.TabIndex = 40;
            this.label8.Text = "分科";
            // 
            // comboBox_subject_group
            // 
            this.comboBox_subject_group.FormattingEnabled = true;
            this.comboBox_subject_group.Location = new System.Drawing.Point(510, 108);
            this.comboBox_subject_group.Name = "comboBox_subject_group";
            this.comboBox_subject_group.Size = new System.Drawing.Size(108, 20);
            this.comboBox_subject_group.TabIndex = 41;
            // 
            // ScoreCriticalConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 722);
            this.Controls.Add(this.comboBox_subject_group);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.num_critical_ratio);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.num_target_count);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox_university_level);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dtp_year);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_import);
            this.Controls.Add(this.button_template);
            this.Controls.Add(this.dataGridView_preview);
            this.Controls.Add(this.button_add);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBox_grade);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView_exams);
            this.Name = "ScoreCriticalConfigForm";
            this.Text = "ScoreCriticalConfigForm";
            this.Load += new System.EventHandler(this.ScoreCriticalConfigForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_preview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_exams)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_target_count)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_critical_ratio)).EndInit();
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
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox_grade;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView_exams;
        private System.Windows.Forms.DateTimePicker dtp_year;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_university_level;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown num_target_count;
        private System.Windows.Forms.NumericUpDown num_critical_ratio;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBox_subject_group;
    }
}