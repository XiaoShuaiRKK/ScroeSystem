namespace ScoreSystem
{
    partial class ScoreUpGradeForm
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
            this.comboBox_grade = new System.Windows.Forms.ComboBox();
            this.button_to_update = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F);
            this.label1.Location = new System.Drawing.Point(47, 102);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "年级:";
            // 
            // comboBox_grade
            // 
            this.comboBox_grade.FormattingEnabled = true;
            this.comboBox_grade.Location = new System.Drawing.Point(114, 98);
            this.comboBox_grade.Name = "comboBox_grade";
            this.comboBox_grade.Size = new System.Drawing.Size(263, 20);
            this.comboBox_grade.TabIndex = 1;
            // 
            // button_to_update
            // 
            this.button_to_update.Location = new System.Drawing.Point(178, 202);
            this.button_to_update.Name = "button_to_update";
            this.button_to_update.Size = new System.Drawing.Size(75, 23);
            this.button_to_update.TabIndex = 2;
            this.button_to_update.Text = "升班";
            this.button_to_update.UseVisualStyleBackColor = true;
            this.button_to_update.Click += new System.EventHandler(this.button_to_update_Click);
            // 
            // ScoreUpGradeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(446, 265);
            this.Controls.Add(this.button_to_update);
            this.Controls.Add(this.comboBox_grade);
            this.Controls.Add(this.label1);
            this.Name = "ScoreUpGradeForm";
            this.Text = "ScoreUpGradeForm";
            this.Load += new System.EventHandler(this.ScoreUpGradeForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_grade;
        private System.Windows.Forms.Button button_to_update;
    }
}