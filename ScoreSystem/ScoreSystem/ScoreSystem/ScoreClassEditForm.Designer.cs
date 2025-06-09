namespace ScoreSystem
{
    partial class ScoreClassEditForm
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
            this.label_class_name = new System.Windows.Forms.Label();
            this.label_class_teacher_name = new System.Windows.Forms.Label();
            this.textBox_class_name = new System.Windows.Forms.TextBox();
            this.textBox_taecher_name = new System.Windows.Forms.TextBox();
            this.dataGridView_students = new System.Windows.Forms.DataGridView();
            this.button_cancle = new System.Windows.Forms.Button();
            this.button_edit = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_teacher = new System.Windows.Forms.ComboBox();
            this.comboBox_subjectGroup = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_subjectGroup = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_students)).BeginInit();
            this.SuspendLayout();
            // 
            // label_class_name
            // 
            this.label_class_name.AutoSize = true;
            this.label_class_name.Location = new System.Drawing.Point(39, 53);
            this.label_class_name.Name = "label_class_name";
            this.label_class_name.Size = new System.Drawing.Size(35, 12);
            this.label_class_name.TabIndex = 0;
            this.label_class_name.Text = "班级:";
            // 
            // label_class_teacher_name
            // 
            this.label_class_teacher_name.AutoSize = true;
            this.label_class_teacher_name.Location = new System.Drawing.Point(39, 107);
            this.label_class_teacher_name.Name = "label_class_teacher_name";
            this.label_class_teacher_name.Size = new System.Drawing.Size(53, 12);
            this.label_class_teacher_name.TabIndex = 1;
            this.label_class_teacher_name.Text = "班主任：";
            // 
            // textBox_class_name
            // 
            this.textBox_class_name.Location = new System.Drawing.Point(80, 50);
            this.textBox_class_name.Name = "textBox_class_name";
            this.textBox_class_name.Size = new System.Drawing.Size(374, 21);
            this.textBox_class_name.TabIndex = 2;
            // 
            // textBox_taecher_name
            // 
            this.textBox_taecher_name.Location = new System.Drawing.Point(98, 104);
            this.textBox_taecher_name.Name = "textBox_taecher_name";
            this.textBox_taecher_name.Size = new System.Drawing.Size(149, 21);
            this.textBox_taecher_name.TabIndex = 3;
            // 
            // dataGridView_students
            // 
            this.dataGridView_students.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_students.Location = new System.Drawing.Point(27, 250);
            this.dataGridView_students.Name = "dataGridView_students";
            this.dataGridView_students.RowTemplate.Height = 23;
            this.dataGridView_students.Size = new System.Drawing.Size(724, 216);
            this.dataGridView_students.TabIndex = 4;
            this.dataGridView_students.DoubleClick += new System.EventHandler(this.dataGridView_students_DoubleClick);
            // 
            // button_cancle
            // 
            this.button_cancle.Location = new System.Drawing.Point(242, 484);
            this.button_cancle.Name = "button_cancle";
            this.button_cancle.Size = new System.Drawing.Size(75, 23);
            this.button_cancle.TabIndex = 6;
            this.button_cancle.Text = "取消";
            this.button_cancle.UseVisualStyleBackColor = true;
            // 
            // button_edit
            // 
            this.button_edit.Location = new System.Drawing.Point(433, 484);
            this.button_edit.Name = "button_edit";
            this.button_edit.Size = new System.Drawing.Size(75, 23);
            this.button_edit.TabIndex = 7;
            this.button_edit.Text = "修改";
            this.button_edit.UseVisualStyleBackColor = true;
            this.button_edit.Click += new System.EventHandler(this.button_edit_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 223);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "所属学生";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(335, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "新班主任：";
            // 
            // comboBox_teacher
            // 
            this.comboBox_teacher.FormattingEnabled = true;
            this.comboBox_teacher.Location = new System.Drawing.Point(406, 110);
            this.comboBox_teacher.Name = "comboBox_teacher";
            this.comboBox_teacher.Size = new System.Drawing.Size(180, 20);
            this.comboBox_teacher.TabIndex = 11;
            // 
            // comboBox_subjectGroup
            // 
            this.comboBox_subjectGroup.FormattingEnabled = true;
            this.comboBox_subjectGroup.Location = new System.Drawing.Point(406, 165);
            this.comboBox_subjectGroup.Name = "comboBox_subjectGroup";
            this.comboBox_subjectGroup.Size = new System.Drawing.Size(180, 20);
            this.comboBox_subjectGroup.TabIndex = 13;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(346, 168);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "新分科：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(39, 168);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 14;
            this.label4.Text = "旧分科：";
            // 
            // textBox_subjectGroup
            // 
            this.textBox_subjectGroup.Location = new System.Drawing.Point(98, 165);
            this.textBox_subjectGroup.Name = "textBox_subjectGroup";
            this.textBox_subjectGroup.Size = new System.Drawing.Size(149, 21);
            this.textBox_subjectGroup.TabIndex = 15;
            // 
            // ScoreClassEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 542);
            this.Controls.Add(this.textBox_subjectGroup);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBox_subjectGroup);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox_teacher);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button_edit);
            this.Controls.Add(this.button_cancle);
            this.Controls.Add(this.dataGridView_students);
            this.Controls.Add(this.textBox_taecher_name);
            this.Controls.Add(this.textBox_class_name);
            this.Controls.Add(this.label_class_teacher_name);
            this.Controls.Add(this.label_class_name);
            this.Name = "ScoreClassEditForm";
            this.Text = "ScoreClassEditForm";
            this.Load += new System.EventHandler(this.ScoreClassEditForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_students)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_class_name;
        private System.Windows.Forms.Label label_class_teacher_name;
        private System.Windows.Forms.TextBox textBox_class_name;
        private System.Windows.Forms.TextBox textBox_taecher_name;
        private System.Windows.Forms.DataGridView dataGridView_students;
        private System.Windows.Forms.Button button_cancle;
        private System.Windows.Forms.Button button_edit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_teacher;
        private System.Windows.Forms.ComboBox comboBox_subjectGroup;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_subjectGroup;
    }
}