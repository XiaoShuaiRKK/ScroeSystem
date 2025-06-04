namespace ScoreSystem
{
    partial class ScoreMainForm
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
            this.components = new System.ComponentModel.Container();
            this.label_hello = new System.Windows.Forms.Label();
            this.label_time = new System.Windows.Forms.Label();
            this.timer_now_time = new System.Windows.Forms.Timer(this.components);
            this.comboBox_class = new System.Windows.Forms.ComboBox();
            this.label_class = new System.Windows.Forms.Label();
            this.dataGridView_students = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menu_logout = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_exam = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_score = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_class_or_student = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_university = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_trend = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_critical = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_teacher = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_class = new System.Windows.Forms.ToolStripMenuItem();
            this.button_edit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_students)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_hello
            // 
            this.label_hello.AutoSize = true;
            this.label_hello.Location = new System.Drawing.Point(31, 32);
            this.label_hello.Name = "label_hello";
            this.label_hello.Size = new System.Drawing.Size(29, 12);
            this.label_hello.TabIndex = 0;
            this.label_hello.Text = "你好";
            // 
            // label_time
            // 
            this.label_time.AutoSize = true;
            this.label_time.Location = new System.Drawing.Point(550, 32);
            this.label_time.Name = "label_time";
            this.label_time.Size = new System.Drawing.Size(59, 12);
            this.label_time.TabIndex = 1;
            this.label_time.Text = "当前时间:";
            // 
            // comboBox_class
            // 
            this.comboBox_class.FormattingEnabled = true;
            this.comboBox_class.Location = new System.Drawing.Point(75, 76);
            this.comboBox_class.Name = "comboBox_class";
            this.comboBox_class.Size = new System.Drawing.Size(121, 20);
            this.comboBox_class.TabIndex = 2;
            this.comboBox_class.SelectedIndexChanged += new System.EventHandler(this.comboBox_class_SelectedIndexChanged);
            // 
            // label_class
            // 
            this.label_class.AutoSize = true;
            this.label_class.Location = new System.Drawing.Point(31, 79);
            this.label_class.Name = "label_class";
            this.label_class.Size = new System.Drawing.Size(35, 12);
            this.label_class.TabIndex = 3;
            this.label_class.Text = "班级:";
            // 
            // dataGridView_students
            // 
            this.dataGridView_students.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_students.Location = new System.Drawing.Point(33, 114);
            this.dataGridView_students.Name = "dataGridView_students";
            this.dataGridView_students.RowTemplate.Height = 23;
            this.dataGridView_students.Size = new System.Drawing.Size(725, 324);
            this.dataGridView_students.TabIndex = 4;
            this.dataGridView_students.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_students_CellContentClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_logout,
            this.menu_exam,
            this.menu_score,
            this.menu_class_or_student,
            this.menu_university,
            this.menu_trend,
            this.menu_critical,
            this.menu_class,
            this.menu_teacher});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 25);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menu_logout
            // 
            this.menu_logout.Name = "menu_logout";
            this.menu_logout.Size = new System.Drawing.Size(68, 21);
            this.menu_logout.Text = "退出登录";
            this.menu_logout.Click += new System.EventHandler(this.menu_logout_Click);
            // 
            // menu_exam
            // 
            this.menu_exam.Name = "menu_exam";
            this.menu_exam.Size = new System.Drawing.Size(68, 21);
            this.menu_exam.Text = "考试管理";
            this.menu_exam.Click += new System.EventHandler(this.menu_exam_Click);
            // 
            // menu_score
            // 
            this.menu_score.Name = "menu_score";
            this.menu_score.Size = new System.Drawing.Size(68, 21);
            this.menu_score.Text = "分数管理";
            this.menu_score.Click += new System.EventHandler(this.menu_score_Click);
            // 
            // menu_class_or_student
            // 
            this.menu_class_or_student.Name = "menu_class_or_student";
            this.menu_class_or_student.Size = new System.Drawing.Size(68, 21);
            this.menu_class_or_student.Text = "学生导入";
            this.menu_class_or_student.Click += new System.EventHandler(this.menu_class_or_student_Click);
            // 
            // menu_university
            // 
            this.menu_university.Name = "menu_university";
            this.menu_university.Size = new System.Drawing.Size(68, 21);
            this.menu_university.Text = "上线情况";
            this.menu_university.Click += new System.EventHandler(this.menu_university_Click);
            // 
            // menu_trend
            // 
            this.menu_trend.Name = "menu_trend";
            this.menu_trend.Size = new System.Drawing.Size(68, 21);
            this.menu_trend.Text = "学情跟踪";
            this.menu_trend.Click += new System.EventHandler(this.menu_trend_Click);
            // 
            // menu_critical
            // 
            this.menu_critical.Name = "menu_critical";
            this.menu_critical.Size = new System.Drawing.Size(80, 21);
            this.menu_critical.Text = "临界生管理";
            this.menu_critical.Click += new System.EventHandler(this.menu_critical_Click);
            // 
            // menu_teacher
            // 
            this.menu_teacher.Name = "menu_teacher";
            this.menu_teacher.Size = new System.Drawing.Size(68, 21);
            this.menu_teacher.Text = "老师管理";
            this.menu_teacher.Click += new System.EventHandler(this.menu_teacher_Click);
            // 
            // menu_class
            // 
            this.menu_class.Name = "menu_class";
            this.menu_class.Size = new System.Drawing.Size(68, 21);
            this.menu_class.Text = "班级管理";
            this.menu_class.Click += new System.EventHandler(this.menu_class_Click);
            // 
            // button_edit
            // 
            this.button_edit.Location = new System.Drawing.Point(650, 79);
            this.button_edit.Name = "button_edit";
            this.button_edit.Size = new System.Drawing.Size(75, 23);
            this.button_edit.TabIndex = 6;
            this.button_edit.Text = "修改";
            this.button_edit.UseVisualStyleBackColor = true;
            this.button_edit.Click += new System.EventHandler(this.button_edit_Click);
            // 
            // ScoreMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button_edit);
            this.Controls.Add(this.dataGridView_students);
            this.Controls.Add(this.label_class);
            this.Controls.Add(this.comboBox_class);
            this.Controls.Add(this.label_time);
            this.Controls.Add(this.label_hello);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ScoreMainForm";
            this.Text = "ScoreMainForm";
            this.Load += new System.EventHandler(this.ScoreMainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_students)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_hello;
        private System.Windows.Forms.Label label_time;
        private System.Windows.Forms.Timer timer_now_time;
        private System.Windows.Forms.ComboBox comboBox_class;
        private System.Windows.Forms.Label label_class;
        private System.Windows.Forms.DataGridView dataGridView_students;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menu_logout;
        private System.Windows.Forms.ToolStripMenuItem menu_exam;
        private System.Windows.Forms.ToolStripMenuItem menu_score;
        private System.Windows.Forms.ToolStripMenuItem menu_class_or_student;
        private System.Windows.Forms.ToolStripMenuItem menu_university;
        private System.Windows.Forms.ToolStripMenuItem menu_trend;
        private System.Windows.Forms.ToolStripMenuItem menu_critical;
        private System.Windows.Forms.ToolStripMenuItem menu_teacher;
        private System.Windows.Forms.ToolStripMenuItem menu_class;
        private System.Windows.Forms.Button button_edit;
    }
}