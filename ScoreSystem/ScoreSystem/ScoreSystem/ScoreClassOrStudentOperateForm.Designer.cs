namespace ScoreSystem
{
    partial class ScoreClassOrStudentOperateForm
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
            this.button_import_student = new System.Windows.Forms.Button();
            this.dataGridView_preview = new System.Windows.Forms.DataGridView();
            this.button_template = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button_save = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_preview)).BeginInit();
            this.SuspendLayout();
            // 
            // button_import_student
            // 
            this.button_import_student.Location = new System.Drawing.Point(48, 109);
            this.button_import_student.Name = "button_import_student";
            this.button_import_student.Size = new System.Drawing.Size(119, 23);
            this.button_import_student.TabIndex = 0;
            this.button_import_student.Text = "导入学生";
            this.button_import_student.UseVisualStyleBackColor = true;
            this.button_import_student.Click += new System.EventHandler(this.button_import_student_Click);
            // 
            // dataGridView_preview
            // 
            this.dataGridView_preview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_preview.Location = new System.Drawing.Point(219, 56);
            this.dataGridView_preview.Name = "dataGridView_preview";
            this.dataGridView_preview.RowTemplate.Height = 23;
            this.dataGridView_preview.Size = new System.Drawing.Size(524, 361);
            this.dataGridView_preview.TabIndex = 1;
            // 
            // button_template
            // 
            this.button_template.Location = new System.Drawing.Point(48, 56);
            this.button_template.Name = "button_template";
            this.button_template.Size = new System.Drawing.Size(119, 23);
            this.button_template.TabIndex = 2;
            this.button_template.Text = "下载学生导入模板";
            this.button_template.UseVisualStyleBackColor = true;
            this.button_template.Click += new System.EventHandler(this.button_template_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(230, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "预览";
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(48, 314);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(119, 23);
            this.button_save.TabIndex = 4;
            this.button_save.Text = "保存";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(48, 359);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(119, 23);
            this.button_cancel.TabIndex = 5;
            this.button_cancel.Text = "取消";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // ScoreClassOrStudentOperateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_template);
            this.Controls.Add(this.dataGridView_preview);
            this.Controls.Add(this.button_import_student);
            this.Name = "ScoreClassOrStudentOperateForm";
            this.Text = "ScoreClassOrStudentOperateForm";
            this.Load += new System.EventHandler(this.ScoreClassOrStudentOperateForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_preview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_import_student;
        private System.Windows.Forms.DataGridView dataGridView_preview;
        private System.Windows.Forms.Button button_template;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Button button_cancel;
    }
}