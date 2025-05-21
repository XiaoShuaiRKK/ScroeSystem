namespace ScoreSystem
{
    partial class ScoreCriticalForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menu_critical_config = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridView_critical = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox_grade = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.dtp_year = new System.Windows.Forms.DateTimePicker();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_critical)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_critical_config});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menu_critical_config
            // 
            this.menu_critical_config.Name = "menu_critical_config";
            this.menu_critical_config.Size = new System.Drawing.Size(68, 21);
            this.menu_critical_config.Text = "临界配置";
            this.menu_critical_config.Click += new System.EventHandler(this.menu_critical_config_Click);
            // 
            // dataGridView_critical
            // 
            this.dataGridView_critical.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_critical.Location = new System.Drawing.Point(12, 97);
            this.dataGridView_critical.Name = "dataGridView_critical";
            this.dataGridView_critical.RowTemplate.Height = 23;
            this.dataGridView_critical.Size = new System.Drawing.Size(776, 262);
            this.dataGridView_critical.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(314, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 26;
            this.label4.Text = "年级";
            // 
            // comboBox_grade
            // 
            this.comboBox_grade.FormattingEnabled = true;
            this.comboBox_grade.Location = new System.Drawing.Point(349, 62);
            this.comboBox_grade.Name = "comboBox_grade";
            this.comboBox_grade.Size = new System.Drawing.Size(173, 20);
            this.comboBox_grade.TabIndex = 25;
            this.comboBox_grade.SelectedIndexChanged += new System.EventHandler(this.comboBox_grade_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(528, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 28;
            this.label5.Text = "年份";
            // 
            // dtp_year
            // 
            this.dtp_year.Location = new System.Drawing.Point(563, 62);
            this.dtp_year.Name = "dtp_year";
            this.dtp_year.Size = new System.Drawing.Size(216, 21);
            this.dtp_year.TabIndex = 27;
            this.dtp_year.ValueChanged += new System.EventHandler(this.dtp_year_ValueChanged);
            // 
            // ScoreCriticalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 382);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dtp_year);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.comboBox_grade);
            this.Controls.Add(this.dataGridView_critical);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ScoreCriticalForm";
            this.Text = "ScoreCriticalForm";
            this.Load += new System.EventHandler(this.ScoreCriticalForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_critical)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menu_critical_config;
        private System.Windows.Forms.DataGridView dataGridView_critical;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox_grade;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dtp_year;
    }
}