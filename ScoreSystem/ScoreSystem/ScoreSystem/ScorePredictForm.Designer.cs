namespace ScoreSystem
{
    partial class ScorePredictForm
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
            this.dataGridView_predict = new System.Windows.Forms.DataGridView();
            this.comboBox_university_level = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_grade = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menu_print = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_predict)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView_predict
            // 
            this.dataGridView_predict.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_predict.Location = new System.Drawing.Point(24, 99);
            this.dataGridView_predict.Name = "dataGridView_predict";
            this.dataGridView_predict.RowTemplate.Height = 23;
            this.dataGridView_predict.Size = new System.Drawing.Size(755, 339);
            this.dataGridView_predict.TabIndex = 0;
            // 
            // comboBox_university_level
            // 
            this.comboBox_university_level.FormattingEnabled = true;
            this.comboBox_university_level.Location = new System.Drawing.Point(498, 60);
            this.comboBox_university_level.Name = "comboBox_university_level";
            this.comboBox_university_level.Size = new System.Drawing.Size(134, 20);
            this.comboBox_university_level.TabIndex = 28;
            this.comboBox_university_level.SelectedIndexChanged += new System.EventHandler(this.comboBox_university_level_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F);
            this.label2.Location = new System.Drawing.Point(421, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 16);
            this.label2.TabIndex = 27;
            this.label2.Text = "大学等级";
            // 
            // comboBox_grade
            // 
            this.comboBox_grade.FormattingEnabled = true;
            this.comboBox_grade.Location = new System.Drawing.Point(683, 60);
            this.comboBox_grade.Name = "comboBox_grade";
            this.comboBox_grade.Size = new System.Drawing.Size(86, 20);
            this.comboBox_grade.TabIndex = 26;
            this.comboBox_grade.SelectedIndexChanged += new System.EventHandler(this.comboBox_grade_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F);
            this.label4.Location = new System.Drawing.Point(638, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 16);
            this.label4.TabIndex = 25;
            this.label4.Text = "年级";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_print});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 25);
            this.menuStrip1.TabIndex = 29;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menu_print
            // 
            this.menu_print.Name = "menu_print";
            this.menu_print.Size = new System.Drawing.Size(44, 21);
            this.menu_print.Text = "打印";
            this.menu_print.Click += new System.EventHandler(this.menu_print_Click);
            // 
            // ScorePredictForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.comboBox_university_level);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox_grade);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dataGridView_predict);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ScorePredictForm";
            this.Text = "ScorePredictForm";
            this.Load += new System.EventHandler(this.ScorePredictForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_predict)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView_predict;
        private System.Windows.Forms.ComboBox comboBox_university_level;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_grade;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menu_print;
    }
}