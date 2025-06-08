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
            this.menu_rate = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridView_critical = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox_exam = new System.Windows.Forms.ComboBox();
            this.comboBox_subject_group = new System.Windows.Forms.ComboBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_critical)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_critical_config,
            this.menu_rate});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(800, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menu_critical_config
            // 
            this.menu_critical_config.Name = "menu_critical_config";
            this.menu_critical_config.Size = new System.Drawing.Size(68, 21);
            this.menu_critical_config.Text = "上线配置";
            this.menu_critical_config.Click += new System.EventHandler(this.menu_critical_config_Click);
            // 
            // menu_rate
            // 
            this.menu_rate.Name = "menu_rate";
            this.menu_rate.Size = new System.Drawing.Size(68, 21);
            this.menu_rate.Text = "数率查看";
            this.menu_rate.Click += new System.EventHandler(this.menu_rate_Click);
            // 
            // dataGridView_critical
            // 
            this.dataGridView_critical.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_critical.Location = new System.Drawing.Point(12, 97);
            this.dataGridView_critical.Name = "dataGridView_critical";
            this.dataGridView_critical.RowHeadersWidth = 51;
            this.dataGridView_critical.RowTemplate.Height = 23;
            this.dataGridView_critical.Size = new System.Drawing.Size(776, 262);
            this.dataGridView_critical.TabIndex = 1;
            this.dataGridView_critical.DoubleClick += new System.EventHandler(this.dataGridView_critical_DoubleClick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(193, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 26;
            this.label4.Text = "考试:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(522, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 12);
            this.label5.TabIndex = 28;
            this.label5.Text = "分科:";
            // 
            // comboBox_exam
            // 
            this.comboBox_exam.FormattingEnabled = true;
            this.comboBox_exam.Location = new System.Drawing.Point(234, 64);
            this.comboBox_exam.Name = "comboBox_exam";
            this.comboBox_exam.Size = new System.Drawing.Size(282, 20);
            this.comboBox_exam.TabIndex = 29;
            this.comboBox_exam.SelectedIndexChanged += new System.EventHandler(this.comboBox_exam_SelectedIndexChanged);
            // 
            // comboBox_subject_group
            // 
            this.comboBox_subject_group.FormattingEnabled = true;
            this.comboBox_subject_group.Location = new System.Drawing.Point(554, 64);
            this.comboBox_subject_group.Name = "comboBox_subject_group";
            this.comboBox_subject_group.Size = new System.Drawing.Size(234, 20);
            this.comboBox_subject_group.TabIndex = 30;
            this.comboBox_subject_group.SelectedIndexChanged += new System.EventHandler(this.comboBox_subject_group_SelectedIndexChanged);
            // 
            // ScoreCriticalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 382);
            this.Controls.Add(this.comboBox_subject_group);
            this.Controls.Add(this.comboBox_exam);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
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
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox_exam;
        private System.Windows.Forms.ComboBox comboBox_subject_group;
        private System.Windows.Forms.ToolStripMenuItem menu_rate;
    }
}