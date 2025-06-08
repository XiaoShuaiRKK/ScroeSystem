namespace ScoreSystem
{
    partial class ScoreRateForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_exam = new System.Windows.Forms.ComboBox();
            this.comboBox_subjectGroup = new System.Windows.Forms.ComboBox();
            this.dataGridView_stat = new System.Windows.Forms.DataGridView();
            this.button_generate = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_stat)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(237, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "考试:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(552, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "分科:";
            // 
            // comboBox_exam
            // 
            this.comboBox_exam.FormattingEnabled = true;
            this.comboBox_exam.Location = new System.Drawing.Point(269, 64);
            this.comboBox_exam.Name = "comboBox_exam";
            this.comboBox_exam.Size = new System.Drawing.Size(268, 20);
            this.comboBox_exam.TabIndex = 2;
            this.comboBox_exam.SelectedIndexChanged += new System.EventHandler(this.comboBox_exam_SelectedIndexChanged);
            // 
            // comboBox_subjectGroup
            // 
            this.comboBox_subjectGroup.FormattingEnabled = true;
            this.comboBox_subjectGroup.Location = new System.Drawing.Point(593, 67);
            this.comboBox_subjectGroup.Name = "comboBox_subjectGroup";
            this.comboBox_subjectGroup.Size = new System.Drawing.Size(154, 20);
            this.comboBox_subjectGroup.TabIndex = 3;
            this.comboBox_subjectGroup.SelectedIndexChanged += new System.EventHandler(this.comboBox_subjectGroup_SelectedIndexChanged);
            // 
            // dataGridView_stat
            // 
            this.dataGridView_stat.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_stat.Location = new System.Drawing.Point(32, 101);
            this.dataGridView_stat.Name = "dataGridView_stat";
            this.dataGridView_stat.RowTemplate.Height = 23;
            this.dataGridView_stat.Size = new System.Drawing.Size(744, 364);
            this.dataGridView_stat.TabIndex = 4;
            // 
            // button_generate
            // 
            this.button_generate.Location = new System.Drawing.Point(690, 477);
            this.button_generate.Name = "button_generate";
            this.button_generate.Size = new System.Drawing.Size(75, 23);
            this.button_generate.TabIndex = 5;
            this.button_generate.Text = "生成";
            this.button_generate.UseVisualStyleBackColor = true;
            this.button_generate.Click += new System.EventHandler(this.button_generate_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 7F);
            this.label3.Location = new System.Drawing.Point(40, 477);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(585, 10);
            this.label3.TabIndex = 6;
            this.label3.Text = "如果没有数据则可能没有生成数据，请选择对应的考试然后点击右边的生成按钮(如果还是没数据可能是这个考试暂未录入对应成绩)";
            // 
            // ScoreRateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 512);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button_generate);
            this.Controls.Add(this.dataGridView_stat);
            this.Controls.Add(this.comboBox_subjectGroup);
            this.Controls.Add(this.comboBox_exam);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ScoreRateForm";
            this.Text = "ScoreRateForm";
            this.Load += new System.EventHandler(this.ScoreRateForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_stat)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_exam;
        private System.Windows.Forms.ComboBox comboBox_subjectGroup;
        private System.Windows.Forms.DataGridView dataGridView_stat;
        private System.Windows.Forms.Button button_generate;
        private System.Windows.Forms.Label label3;
    }
}