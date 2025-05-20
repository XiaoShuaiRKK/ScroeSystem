namespace ScoreSystem
{
    partial class ScoreUniversityThresholdForm
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
            this.dataGridView_threshold = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menu_university = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_predict = new System.Windows.Forms.ToolStripMenuItem();
            this.button_back = new System.Windows.Forms.Button();
            this.comboBox_grade = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox_exam = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_university_level = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_threshold)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView_threshold
            // 
            this.dataGridView_threshold.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_threshold.Location = new System.Drawing.Point(26, 100);
            this.dataGridView_threshold.Name = "dataGridView_threshold";
            this.dataGridView_threshold.RowTemplate.Height = 23;
            this.dataGridView_threshold.Size = new System.Drawing.Size(739, 338);
            this.dataGridView_threshold.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_university,
            this.menu_predict});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menu_university
            // 
            this.menu_university.Name = "menu_university";
            this.menu_university.Size = new System.Drawing.Size(92, 21);
            this.menu_university.Text = "大学信息管理";
            this.menu_university.Click += new System.EventHandler(this.menu_university_Click);
            // 
            // menu_predict
            // 
            this.menu_predict.Name = "menu_predict";
            this.menu_predict.Size = new System.Drawing.Size(68, 21);
            this.menu_predict.Text = "上线预测";
            this.menu_predict.Click += new System.EventHandler(this.menu_predict_Click);
            // 
            // button_back
            // 
            this.button_back.Location = new System.Drawing.Point(26, 40);
            this.button_back.Name = "button_back";
            this.button_back.Size = new System.Drawing.Size(75, 23);
            this.button_back.TabIndex = 7;
            this.button_back.Text = "<  返回";
            this.button_back.UseVisualStyleBackColor = true;
            this.button_back.Click += new System.EventHandler(this.button_back_Click);
            // 
            // comboBox_grade
            // 
            this.comboBox_grade.FormattingEnabled = true;
            this.comboBox_grade.Location = new System.Drawing.Point(663, 64);
            this.comboBox_grade.Name = "comboBox_grade";
            this.comboBox_grade.Size = new System.Drawing.Size(86, 20);
            this.comboBox_grade.TabIndex = 22;
            this.comboBox_grade.SelectedIndexChanged += new System.EventHandler(this.comboBox_grade_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F);
            this.label4.Location = new System.Drawing.Point(618, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 16);
            this.label4.TabIndex = 21;
            this.label4.Text = "年级";
            // 
            // comboBox_exam
            // 
            this.comboBox_exam.FormattingEnabled = true;
            this.comboBox_exam.Location = new System.Drawing.Point(489, 64);
            this.comboBox_exam.Name = "comboBox_exam";
            this.comboBox_exam.Size = new System.Drawing.Size(114, 20);
            this.comboBox_exam.TabIndex = 20;
            this.comboBox_exam.SelectedIndexChanged += new System.EventHandler(this.comboBox_exam_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F);
            this.label1.Location = new System.Drawing.Point(444, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 16);
            this.label1.TabIndex = 19;
            this.label1.Text = "考试";
            // 
            // comboBox_university_level
            // 
            this.comboBox_university_level.FormattingEnabled = true;
            this.comboBox_university_level.Location = new System.Drawing.Point(293, 65);
            this.comboBox_university_level.Name = "comboBox_university_level";
            this.comboBox_university_level.Size = new System.Drawing.Size(134, 20);
            this.comboBox_university_level.TabIndex = 24;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F);
            this.label2.Location = new System.Drawing.Point(216, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 16);
            this.label2.TabIndex = 23;
            this.label2.Text = "大学等级";
            // 
            // ScoreUniversityThresholdForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.comboBox_university_level);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox_grade);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBox_exam);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_back);
            this.Controls.Add(this.dataGridView_threshold);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ScoreUniversityThresholdForm";
            this.Text = "ScoreUniversityThresholdForm";
            this.Load += new System.EventHandler(this.ScoreUniversityThresholdForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_threshold)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView_threshold;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menu_university;
        private System.Windows.Forms.Button button_back;
        private System.Windows.Forms.ComboBox comboBox_grade;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox_exam;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem menu_predict;
        private System.Windows.Forms.ComboBox comboBox_university_level;
        private System.Windows.Forms.Label label2;
    }
}