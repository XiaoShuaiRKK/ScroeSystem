namespace ScoreSystem
{
    partial class ScoreThresholdForm
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
            this.comboBox_exam = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView_exam = new System.Windows.Forms.DataGridView();
            this.comboBox_add_exam = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.num_threshold = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_course = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dataGridView_preview = new System.Windows.Forms.DataGridView();
            this.button_add = new System.Windows.Forms.Button();
            this.button_template = new System.Windows.Forms.Button();
            this.button_import = new System.Windows.Forms.Button();
            this.button_save = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_exam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_threshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_preview)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox_exam
            // 
            this.comboBox_exam.FormattingEnabled = true;
            this.comboBox_exam.Location = new System.Drawing.Point(402, 29);
            this.comboBox_exam.Name = "comboBox_exam";
            this.comboBox_exam.Size = new System.Drawing.Size(353, 20);
            this.comboBox_exam.TabIndex = 3;
            this.comboBox_exam.SelectedIndexChanged += new System.EventHandler(this.comboBox_exam_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F);
            this.label1.Location = new System.Drawing.Point(357, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "考试";
            // 
            // dataGridView_exam
            // 
            this.dataGridView_exam.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_exam.Location = new System.Drawing.Point(359, 73);
            this.dataGridView_exam.Name = "dataGridView_exam";
            this.dataGridView_exam.RowTemplate.Height = 23;
            this.dataGridView_exam.Size = new System.Drawing.Size(416, 426);
            this.dataGridView_exam.TabIndex = 4;
            // 
            // comboBox_add_exam
            // 
            this.comboBox_add_exam.FormattingEnabled = true;
            this.comboBox_add_exam.Location = new System.Drawing.Point(78, 73);
            this.comboBox_add_exam.Name = "comboBox_add_exam";
            this.comboBox_add_exam.Size = new System.Drawing.Size(232, 20);
            this.comboBox_add_exam.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F);
            this.label2.Location = new System.Drawing.Point(33, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "考试";
            // 
            // num_threshold
            // 
            this.num_threshold.Location = new System.Drawing.Point(92, 160);
            this.num_threshold.Name = "num_threshold";
            this.num_threshold.Size = new System.Drawing.Size(217, 21);
            this.num_threshold.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F);
            this.label3.Location = new System.Drawing.Point(31, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 16);
            this.label3.TabIndex = 8;
            this.label3.Text = "达标分";
            // 
            // comboBox_course
            // 
            this.comboBox_course.FormattingEnabled = true;
            this.comboBox_course.Location = new System.Drawing.Point(77, 117);
            this.comboBox_course.Name = "comboBox_course";
            this.comboBox_course.Size = new System.Drawing.Size(232, 20);
            this.comboBox_course.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F);
            this.label4.Location = new System.Drawing.Point(32, 121);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 16);
            this.label4.TabIndex = 9;
            this.label4.Text = "科目";
            // 
            // dataGridView_preview
            // 
            this.dataGridView_preview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_preview.Location = new System.Drawing.Point(23, 341);
            this.dataGridView_preview.Name = "dataGridView_preview";
            this.dataGridView_preview.RowTemplate.Height = 23;
            this.dataGridView_preview.Size = new System.Drawing.Size(330, 158);
            this.dataGridView_preview.TabIndex = 11;
            // 
            // button_add
            // 
            this.button_add.Location = new System.Drawing.Point(22, 201);
            this.button_add.Name = "button_add";
            this.button_add.Size = new System.Drawing.Size(320, 23);
            this.button_add.TabIndex = 12;
            this.button_add.Text = "添加";
            this.button_add.UseVisualStyleBackColor = true;
            this.button_add.Click += new System.EventHandler(this.button_add_Click);
            // 
            // button_template
            // 
            this.button_template.Location = new System.Drawing.Point(23, 230);
            this.button_template.Name = "button_template";
            this.button_template.Size = new System.Drawing.Size(320, 23);
            this.button_template.TabIndex = 13;
            this.button_template.Text = "下载达标分设置模板";
            this.button_template.UseVisualStyleBackColor = true;
            this.button_template.Click += new System.EventHandler(this.button_template_Click);
            // 
            // button_import
            // 
            this.button_import.Location = new System.Drawing.Point(24, 259);
            this.button_import.Name = "button_import";
            this.button_import.Size = new System.Drawing.Size(320, 23);
            this.button_import.TabIndex = 14;
            this.button_import.Text = "导入";
            this.button_import.UseVisualStyleBackColor = true;
            this.button_import.Click += new System.EventHandler(this.button_import_Click);
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(24, 288);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(320, 23);
            this.button_save.TabIndex = 15;
            this.button_save.Text = "保存";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F);
            this.label5.Location = new System.Drawing.Point(31, 322);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 16);
            this.label5.TabIndex = 16;
            this.label5.Text = "预览";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 8F);
            this.label6.Location = new System.Drawing.Point(34, 187);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(176, 11);
            this.label6.TabIndex = 17;
            this.label6.Text = "模板导入 不受上面三个筛选的影响";
            // 
            // ScoreThresholdForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 523);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_import);
            this.Controls.Add(this.button_template);
            this.Controls.Add(this.button_add);
            this.Controls.Add(this.dataGridView_preview);
            this.Controls.Add(this.comboBox_course);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.num_threshold);
            this.Controls.Add(this.comboBox_add_exam);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridView_exam);
            this.Controls.Add(this.comboBox_exam);
            this.Controls.Add(this.label1);
            this.Name = "ScoreThresholdForm";
            this.Text = "ScoreThresholdForm";
            this.Load += new System.EventHandler(this.ScoreThresholdForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_exam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_threshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_preview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_exam;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView_exam;
        private System.Windows.Forms.ComboBox comboBox_add_exam;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown num_threshold;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_course;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView dataGridView_preview;
        private System.Windows.Forms.Button button_add;
        private System.Windows.Forms.Button button_template;
        private System.Windows.Forms.Button button_import;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}