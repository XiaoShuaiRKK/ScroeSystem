namespace ScoreSystem
{
    partial class ScoreRankingForm
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
            this.button_class = new System.Windows.Forms.Button();
            this.button_grade = new System.Windows.Forms.Button();
            this.panel_show = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // button_class
            // 
            this.button_class.Location = new System.Drawing.Point(23, 51);
            this.button_class.Name = "button_class";
            this.button_class.Size = new System.Drawing.Size(132, 53);
            this.button_class.TabIndex = 0;
            this.button_class.Text = "班级排名";
            this.button_class.UseVisualStyleBackColor = true;
            this.button_class.Click += new System.EventHandler(this.button_class_Click);
            // 
            // button_grade
            // 
            this.button_grade.Location = new System.Drawing.Point(23, 123);
            this.button_grade.Name = "button_grade";
            this.button_grade.Size = new System.Drawing.Size(132, 51);
            this.button_grade.TabIndex = 1;
            this.button_grade.Text = "年级排名";
            this.button_grade.UseVisualStyleBackColor = true;
            this.button_grade.Click += new System.EventHandler(this.button_grade_Click);
            // 
            // panel_show
            // 
            this.panel_show.Location = new System.Drawing.Point(243, 51);
            this.panel_show.Name = "panel_show";
            this.panel_show.Size = new System.Drawing.Size(717, 499);
            this.panel_show.TabIndex = 2;
            // 
            // ScoreRankingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(972, 575);
            this.Controls.Add(this.panel_show);
            this.Controls.Add(this.button_grade);
            this.Controls.Add(this.button_class);
            this.Name = "ScoreRankingForm";
            this.Text = "ScoreRankingForm";
            this.Load += new System.EventHandler(this.ScoreRankingForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_class;
        private System.Windows.Forms.Button button_grade;
        private System.Windows.Forms.Panel panel_show;
    }
}