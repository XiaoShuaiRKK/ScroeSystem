namespace ScoreSystem
{
    partial class ClassRankingControlForm
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
            this.dataGridView_rank = new System.Windows.Forms.DataGridView();
            this.comboBox_class = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_exam = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_mode = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_rank)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView_rank
            // 
            this.dataGridView_rank.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_rank.Location = new System.Drawing.Point(32, 102);
            this.dataGridView_rank.Name = "dataGridView_rank";
            this.dataGridView_rank.RowTemplate.Height = 23;
            this.dataGridView_rank.Size = new System.Drawing.Size(612, 371);
            this.dataGridView_rank.TabIndex = 0;
            // 
            // comboBox_class
            // 
            this.comboBox_class.FormattingEnabled = true;
            this.comboBox_class.Location = new System.Drawing.Point(351, 65);
            this.comboBox_class.Name = "comboBox_class";
            this.comboBox_class.Size = new System.Drawing.Size(121, 20);
            this.comboBox_class.TabIndex = 7;
            this.comboBox_class.SelectedIndexChanged += new System.EventHandler(this.comboBox_class_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F);
            this.label2.Location = new System.Drawing.Point(306, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 16);
            this.label2.TabIndex = 6;
            this.label2.Text = "班级";
            // 
            // comboBox_exam
            // 
            this.comboBox_exam.FormattingEnabled = true;
            this.comboBox_exam.Location = new System.Drawing.Point(75, 65);
            this.comboBox_exam.Name = "comboBox_exam";
            this.comboBox_exam.Size = new System.Drawing.Size(225, 20);
            this.comboBox_exam.TabIndex = 5;
            this.comboBox_exam.SelectedIndexChanged += new System.EventHandler(this.comboBox_exam_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F);
            this.label1.Location = new System.Drawing.Point(30, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "考试";
            // 
            // comboBox_mode
            // 
            this.comboBox_mode.FormattingEnabled = true;
            this.comboBox_mode.Location = new System.Drawing.Point(520, 65);
            this.comboBox_mode.Name = "comboBox_mode";
            this.comboBox_mode.Size = new System.Drawing.Size(121, 20);
            this.comboBox_mode.TabIndex = 9;
            this.comboBox_mode.SelectedIndexChanged += new System.EventHandler(this.comboBox_mode_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F);
            this.label3.Location = new System.Drawing.Point(475, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 16);
            this.label3.TabIndex = 8;
            this.label3.Text = "模式";
            // 
            // ClassRankingControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comboBox_mode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox_class);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox_exam);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView_rank);
            this.Name = "ClassRankingControlForm";
            this.Size = new System.Drawing.Size(681, 494);
            this.Load += new System.EventHandler(this.ClassRankingControlForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_rank)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView_rank;
        private System.Windows.Forms.ComboBox comboBox_class;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_exam;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_mode;
        private System.Windows.Forms.Label label3;
    }
}
