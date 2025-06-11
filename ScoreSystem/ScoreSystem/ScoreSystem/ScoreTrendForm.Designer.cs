namespace ScoreSystem
{
    partial class ScoreTrendForm
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart_trend = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.label_name = new System.Windows.Forms.Label();
            this.label_student_number = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_subject = new System.Windows.Forms.ComboBox();
            this.comboBox_grade = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_mode = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dataGridView_history = new System.Windows.Forms.DataGridView();
            this.label_course = new System.Windows.Forms.Label();
            this.label_join_date = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menu_edit = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.chart_trend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_history)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart_trend
            // 
            chartArea2.Name = "ChartArea1";
            this.chart_trend.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart_trend.Legends.Add(legend2);
            this.chart_trend.Location = new System.Drawing.Point(417, 161);
            this.chart_trend.Name = "chart_trend";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart_trend.Series.Add(series2);
            this.chart_trend.Size = new System.Drawing.Size(535, 406);
            this.chart_trend.TabIndex = 1;
            this.chart_trend.Text = "chart2";
            // 
            // label_name
            // 
            this.label_name.AutoSize = true;
            this.label_name.Location = new System.Drawing.Point(30, 25);
            this.label_name.Name = "label_name";
            this.label_name.Size = new System.Drawing.Size(29, 12);
            this.label_name.TabIndex = 2;
            this.label_name.Text = "Name";
            // 
            // label_student_number
            // 
            this.label_student_number.AutoSize = true;
            this.label_student_number.Location = new System.Drawing.Point(30, 63);
            this.label_student_number.Name = "label_student_number";
            this.label_student_number.Size = new System.Drawing.Size(83, 12);
            this.label_student_number.TabIndex = 3;
            this.label_student_number.Text = "StudentNumber";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(784, 123);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "科目";
            // 
            // comboBox_subject
            // 
            this.comboBox_subject.FormattingEnabled = true;
            this.comboBox_subject.Location = new System.Drawing.Point(831, 120);
            this.comboBox_subject.Name = "comboBox_subject";
            this.comboBox_subject.Size = new System.Drawing.Size(121, 20);
            this.comboBox_subject.TabIndex = 5;
            this.comboBox_subject.SelectedIndexChanged += new System.EventHandler(this.comboBox_subject_SelectedIndexChanged);
            // 
            // comboBox_grade
            // 
            this.comboBox_grade.FormattingEnabled = true;
            this.comboBox_grade.Location = new System.Drawing.Point(645, 120);
            this.comboBox_grade.Name = "comboBox_grade";
            this.comboBox_grade.Size = new System.Drawing.Size(121, 20);
            this.comboBox_grade.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(598, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "学期";
            // 
            // comboBox_mode
            // 
            this.comboBox_mode.FormattingEnabled = true;
            this.comboBox_mode.Location = new System.Drawing.Point(462, 120);
            this.comboBox_mode.Name = "comboBox_mode";
            this.comboBox_mode.Size = new System.Drawing.Size(121, 20);
            this.comboBox_mode.TabIndex = 9;
            this.comboBox_mode.SelectedIndexChanged += new System.EventHandler(this.comboBox_mode_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(415, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "模式";
            // 
            // dataGridView_history
            // 
            this.dataGridView_history.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_history.Location = new System.Drawing.Point(32, 161);
            this.dataGridView_history.Name = "dataGridView_history";
            this.dataGridView_history.RowTemplate.Height = 23;
            this.dataGridView_history.Size = new System.Drawing.Size(356, 406);
            this.dataGridView_history.TabIndex = 10;
            // 
            // label_course
            // 
            this.label_course.AutoSize = true;
            this.label_course.Location = new System.Drawing.Point(30, 103);
            this.label_course.Name = "label_course";
            this.label_course.Size = new System.Drawing.Size(41, 12);
            this.label_course.TabIndex = 11;
            this.label_course.Text = "Course";
            // 
            // label_join_date
            // 
            this.label_join_date.AutoSize = true;
            this.label_join_date.Location = new System.Drawing.Point(340, 63);
            this.label_join_date.Name = "label_join_date";
            this.label_join_date.Size = new System.Drawing.Size(53, 12);
            this.label_join_date.TabIndex = 12;
            this.label_join_date.Text = "JoinDate";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_edit});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(987, 25);
            this.menuStrip1.TabIndex = 13;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menu_edit
            // 
            this.menu_edit.Name = "menu_edit";
            this.menu_edit.Size = new System.Drawing.Size(44, 21);
            this.menu_edit.Text = "转班";
            this.menu_edit.Click += new System.EventHandler(this.menu_edit_Click);
            // 
            // ScoreTrendForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(987, 595);
            this.Controls.Add(this.label_join_date);
            this.Controls.Add(this.label_course);
            this.Controls.Add(this.dataGridView_history);
            this.Controls.Add(this.comboBox_mode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox_grade);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox_subject);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label_student_number);
            this.Controls.Add(this.label_name);
            this.Controls.Add(this.chart_trend);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ScoreTrendForm";
            this.Text = "ScoreTrendForm";
            this.Load += new System.EventHandler(this.ScoreTrendForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart_trend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_history)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart_trend;
        private System.Windows.Forms.Label label_name;
        private System.Windows.Forms.Label label_student_number;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_subject;
        private System.Windows.Forms.ComboBox comboBox_grade;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_mode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dataGridView_history;
        private System.Windows.Forms.Label label_course;
        private System.Windows.Forms.Label label_join_date;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menu_edit;
    }
}