namespace ScoreSystem
{
    partial class ScoreExamForm
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
            this.dataGridView_exams = new System.Windows.Forms.DataGridView();
            this.dataGridView_preview = new System.Windows.Forms.DataGridView();
            this.button_template = new System.Windows.Forms.Button();
            this.button_import = new System.Windows.Forms.Button();
            this.button_save = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menu_threshold = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_exams)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_preview)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView_exams
            // 
            this.dataGridView_exams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_exams.Location = new System.Drawing.Point(27, 470);
            this.dataGridView_exams.Name = "dataGridView_exams";
            this.dataGridView_exams.RowTemplate.Height = 23;
            this.dataGridView_exams.Size = new System.Drawing.Size(729, 246);
            this.dataGridView_exams.TabIndex = 0;
            this.dataGridView_exams.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_exams_CellContentClick);
            // 
            // dataGridView_preview
            // 
            this.dataGridView_preview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_preview.Location = new System.Drawing.Point(27, 115);
            this.dataGridView_preview.Name = "dataGridView_preview";
            this.dataGridView_preview.RowTemplate.Height = 23;
            this.dataGridView_preview.Size = new System.Drawing.Size(729, 265);
            this.dataGridView_preview.TabIndex = 10;
            this.dataGridView_preview.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_preview_CellContentClick);
            this.dataGridView_preview.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_preview_CellValueChanged);
            this.dataGridView_preview.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_preview_DataError);
            this.dataGridView_preview.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView_preview_DefaultValuesNeeded);
            this.dataGridView_preview.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView_preview_RowValidating);
            // 
            // button_template
            // 
            this.button_template.Location = new System.Drawing.Point(166, 59);
            this.button_template.Name = "button_template";
            this.button_template.Size = new System.Drawing.Size(216, 23);
            this.button_template.TabIndex = 11;
            this.button_template.Text = "下载考试导入模板";
            this.button_template.UseVisualStyleBackColor = true;
            this.button_template.Click += new System.EventHandler(this.button_template_Click);
            // 
            // button_import
            // 
            this.button_import.Location = new System.Drawing.Point(434, 59);
            this.button_import.Name = "button_import";
            this.button_import.Size = new System.Drawing.Size(197, 23);
            this.button_import.TabIndex = 12;
            this.button_import.Text = "导入";
            this.button_import.UseVisualStyleBackColor = true;
            this.button_import.Click += new System.EventHandler(this.button_import_Click);
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(27, 399);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(729, 23);
            this.button_save.TabIndex = 13;
            this.button_save.Text = "保存";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(43, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 14;
            this.label5.Text = "预览";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(43, 441);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 15;
            this.label6.Text = "所有考试";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_threshold});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 25);
            this.menuStrip1.TabIndex = 16;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menu_threshold
            // 
            this.menu_threshold.Name = "menu_threshold";
            this.menu_threshold.Size = new System.Drawing.Size(80, 21);
            this.menu_threshold.Text = "设置达标线";
            this.menu_threshold.Click += new System.EventHandler(this.menu_threshold_Click);
            // 
            // ScoreExamForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 739);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_import);
            this.Controls.Add(this.button_template);
            this.Controls.Add(this.dataGridView_preview);
            this.Controls.Add(this.dataGridView_exams);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ScoreExamForm";
            this.Text = "ScoreExamForm";
            this.Load += new System.EventHandler(this.ScoreExamForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_exams)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_preview)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView_exams;
        private System.Windows.Forms.DataGridView dataGridView_preview;
        private System.Windows.Forms.Button button_template;
        private System.Windows.Forms.Button button_import;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menu_threshold;
    }
}