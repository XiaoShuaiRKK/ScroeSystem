namespace ScoreSystem
{
    partial class GradeRankingControlForm
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBox_mode = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_exam = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView_rank = new System.Windows.Forms.DataGridView();
            this.comboBox_subject_group = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_rank)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox_mode
            // 
            this.comboBox_mode.FormattingEnabled = true;
            this.comboBox_mode.Location = new System.Drawing.Point(524, 47);
            this.comboBox_mode.Name = "comboBox_mode";
            this.comboBox_mode.Size = new System.Drawing.Size(121, 20);
            this.comboBox_mode.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F);
            this.label3.Location = new System.Drawing.Point(479, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 16);
            this.label3.TabIndex = 13;
            this.label3.Text = "模式";
            // 
            // comboBox_exam
            // 
            this.comboBox_exam.FormattingEnabled = true;
            this.comboBox_exam.Location = new System.Drawing.Point(76, 51);
            this.comboBox_exam.Name = "comboBox_exam";
            this.comboBox_exam.Size = new System.Drawing.Size(255, 20);
            this.comboBox_exam.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F);
            this.label1.Location = new System.Drawing.Point(31, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 16);
            this.label1.TabIndex = 11;
            this.label1.Text = "考试";
            // 
            // dataGridView_rank
            // 
            this.dataGridView_rank.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_rank.Location = new System.Drawing.Point(36, 84);
            this.dataGridView_rank.Name = "dataGridView_rank";
            this.dataGridView_rank.RowTemplate.Height = 23;
            this.dataGridView_rank.Size = new System.Drawing.Size(612, 371);
            this.dataGridView_rank.TabIndex = 10;
            // 
            // comboBox_subject_group
            // 
            this.comboBox_subject_group.FormattingEnabled = true;
            this.comboBox_subject_group.Location = new System.Drawing.Point(387, 51);
            this.comboBox_subject_group.Name = "comboBox_subject_group";
            this.comboBox_subject_group.Size = new System.Drawing.Size(86, 20);
            this.comboBox_subject_group.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F);
            this.label2.Location = new System.Drawing.Point(342, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 16);
            this.label2.TabIndex = 15;
            this.label2.Text = "分科";
            // 
            // GradeRankingControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboBox_subject_group);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox_mode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox_exam);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView_rank);
            this.Name = "GradeRankingControlForm";
            this.Size = new System.Drawing.Size(690, 494);
            this.Load += new System.EventHandler(this.GradeRankingControlForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_rank)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_mode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_exam;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView_rank;
        private System.Windows.Forms.ComboBox comboBox_subject_group;
        private System.Windows.Forms.Label label2;
    }
}
