namespace ScoreSystem
{
    partial class ScoreScoreForm
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
            this.comboBox_exam = new System.Windows.Forms.ComboBox();
            this.comboBox_class = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridView_score = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menu_import = new System.Windows.Forms.ToolStripMenuItem();
            this.button_back = new System.Windows.Forms.Button();
            this.menu_rank = new System.Windows.Forms.ToolStripMenuItem();
            this.label_threshold = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_score)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F);
            this.label1.Location = new System.Drawing.Point(215, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "考试";
            // 
            // comboBox_exam
            // 
            this.comboBox_exam.FormattingEnabled = true;
            this.comboBox_exam.Location = new System.Drawing.Point(260, 67);
            this.comboBox_exam.Name = "comboBox_exam";
            this.comboBox_exam.Size = new System.Drawing.Size(297, 20);
            this.comboBox_exam.TabIndex = 1;
            this.comboBox_exam.SelectedIndexChanged += new System.EventHandler(this.comboBox_exam_SelectedIndexChanged);
            // 
            // comboBox_class
            // 
            this.comboBox_class.FormattingEnabled = true;
            this.comboBox_class.Location = new System.Drawing.Point(608, 67);
            this.comboBox_class.Name = "comboBox_class";
            this.comboBox_class.Size = new System.Drawing.Size(121, 20);
            this.comboBox_class.TabIndex = 3;
            this.comboBox_class.SelectedIndexChanged += new System.EventHandler(this.comboBox_class_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F);
            this.label2.Location = new System.Drawing.Point(563, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "班级";
            // 
            // dataGridView_score
            // 
            this.dataGridView_score.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_score.Location = new System.Drawing.Point(41, 103);
            this.dataGridView_score.Name = "dataGridView_score";
            this.dataGridView_score.RowTemplate.Height = 23;
            this.dataGridView_score.Size = new System.Drawing.Size(713, 335);
            this.dataGridView_score.TabIndex = 4;
            this.dataGridView_score.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView_score_CellFormatting);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_import,
            this.menu_rank});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 25);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menu_import
            // 
            this.menu_import.Name = "menu_import";
            this.menu_import.Size = new System.Drawing.Size(68, 21);
            this.menu_import.Text = "分数导入";
            this.menu_import.Click += new System.EventHandler(this.menu_import_Click);
            // 
            // button_back
            // 
            this.button_back.Location = new System.Drawing.Point(41, 39);
            this.button_back.Name = "button_back";
            this.button_back.Size = new System.Drawing.Size(75, 23);
            this.button_back.TabIndex = 6;
            this.button_back.Text = "<  返回";
            this.button_back.UseVisualStyleBackColor = true;
            this.button_back.Click += new System.EventHandler(this.button_back_Click);
            // 
            // menu_rank
            // 
            this.menu_rank.Name = "menu_rank";
            this.menu_rank.Size = new System.Drawing.Size(44, 21);
            this.menu_rank.Text = "排行";
            this.menu_rank.Click += new System.EventHandler(this.menu_rank_Click);
            // 
            // label_threshold
            // 
            this.label_threshold.AutoSize = true;
            this.label_threshold.Font = new System.Drawing.Font("宋体", 9F);
            this.label_threshold.Location = new System.Drawing.Point(38, 454);
            this.label_threshold.Name = "label_threshold";
            this.label_threshold.Size = new System.Drawing.Size(41, 12);
            this.label_threshold.TabIndex = 7;
            this.label_threshold.Text = "达标线";
            // 
            // ScoreScoreForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 496);
            this.Controls.Add(this.label_threshold);
            this.Controls.Add(this.button_back);
            this.Controls.Add(this.dataGridView_score);
            this.Controls.Add(this.comboBox_class);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox_exam);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ScoreScoreForm";
            this.Text = "ScoreScoreForm";
            this.Load += new System.EventHandler(this.ScoreScoreForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_score)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_exam;
        private System.Windows.Forms.ComboBox comboBox_class;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dataGridView_score;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menu_import;
        private System.Windows.Forms.Button button_back;
        private System.Windows.Forms.ToolStripMenuItem menu_rank;
        private System.Windows.Forms.Label label_threshold;
    }
}