namespace ScoreSystem
{
    partial class ScoreCriticalConfigForm
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
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button_save = new System.Windows.Forms.Button();
            this.button_import = new System.Windows.Forms.Button();
            this.button_template = new System.Windows.Forms.Button();
            this.dataGridView_preview = new System.Windows.Forms.DataGridView();
            this.dataGridView_exams = new System.Windows.Forms.DataGridView();
            this.button_edit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_preview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_exams)).BeginInit();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(39, 394);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 31;
            this.label6.Text = "所有配置";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(39, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 30;
            this.label5.Text = "预览";
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(23, 352);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(729, 23);
            this.button_save.TabIndex = 29;
            this.button_save.Text = "保存";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_import
            // 
            this.button_import.Location = new System.Drawing.Point(403, 30);
            this.button_import.Name = "button_import";
            this.button_import.Size = new System.Drawing.Size(197, 23);
            this.button_import.TabIndex = 28;
            this.button_import.Text = "导入";
            this.button_import.UseVisualStyleBackColor = true;
            this.button_import.Click += new System.EventHandler(this.button_import_Click);
            // 
            // button_template
            // 
            this.button_template.Location = new System.Drawing.Point(156, 30);
            this.button_template.Name = "button_template";
            this.button_template.Size = new System.Drawing.Size(216, 23);
            this.button_template.TabIndex = 27;
            this.button_template.Text = "下载临界设置导入模板";
            this.button_template.UseVisualStyleBackColor = true;
            this.button_template.Click += new System.EventHandler(this.button_template_Click);
            // 
            // dataGridView_preview
            // 
            this.dataGridView_preview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_preview.Location = new System.Drawing.Point(23, 94);
            this.dataGridView_preview.Name = "dataGridView_preview";
            this.dataGridView_preview.RowTemplate.Height = 23;
            this.dataGridView_preview.Size = new System.Drawing.Size(729, 239);
            this.dataGridView_preview.TabIndex = 26;
            this.dataGridView_preview.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_preview_CellContentClick);
            this.dataGridView_preview.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_preview_DataError);
            this.dataGridView_preview.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridView_preview_DefaultValuesNeeded);
            this.dataGridView_preview.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView_preview_RowValidating);
            // 
            // dataGridView_exams
            // 
            this.dataGridView_exams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_exams.Location = new System.Drawing.Point(23, 425);
            this.dataGridView_exams.Name = "dataGridView_exams";
            this.dataGridView_exams.RowTemplate.Height = 23;
            this.dataGridView_exams.Size = new System.Drawing.Size(729, 272);
            this.dataGridView_exams.TabIndex = 16;
            this.dataGridView_exams.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_exams_CellContentClick);
            this.dataGridView_exams.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_exams_DataError);
            // 
            // button_edit
            // 
            this.button_edit.Location = new System.Drawing.Point(643, 396);
            this.button_edit.Name = "button_edit";
            this.button_edit.Size = new System.Drawing.Size(109, 23);
            this.button_edit.TabIndex = 32;
            this.button_edit.Text = "修改";
            this.button_edit.UseVisualStyleBackColor = true;
            this.button_edit.Click += new System.EventHandler(this.button_edit_Click);
            // 
            // ScoreCriticalConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 722);
            this.Controls.Add(this.button_edit);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.button_import);
            this.Controls.Add(this.button_template);
            this.Controls.Add(this.dataGridView_preview);
            this.Controls.Add(this.dataGridView_exams);
            this.Name = "ScoreCriticalConfigForm";
            this.Text = "ScoreCriticalConfigForm";
            this.Load += new System.EventHandler(this.ScoreCriticalConfigForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_preview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_exams)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Button button_import;
        private System.Windows.Forms.Button button_template;
        private System.Windows.Forms.DataGridView dataGridView_preview;
        private System.Windows.Forms.DataGridView dataGridView_exams;
        private System.Windows.Forms.Button button_edit;
    }
}