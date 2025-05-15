namespace ScoreSystem
{
    partial class ScoreImportForm
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
            this.button_cancel = new System.Windows.Forms.Button();
            this.button_save = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button_template = new System.Windows.Forms.Button();
            this.dataGridView_preview = new System.Windows.Forms.DataGridView();
            this.button_import_score = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_preview)).BeginInit();
            this.SuspendLayout();
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(53, 361);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(119, 23);
            this.button_cancel.TabIndex = 11;
            this.button_cancel.Text = "取消";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(53, 316);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(119, 23);
            this.button_save.TabIndex = 10;
            this.button_save.Text = "保存";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(235, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "预览";
            // 
            // button_template
            // 
            this.button_template.Location = new System.Drawing.Point(53, 58);
            this.button_template.Name = "button_template";
            this.button_template.Size = new System.Drawing.Size(119, 23);
            this.button_template.TabIndex = 8;
            this.button_template.Text = "下载成绩导入模板";
            this.button_template.UseVisualStyleBackColor = true;
            this.button_template.Click += new System.EventHandler(this.button_template_Click);
            // 
            // dataGridView_preview
            // 
            this.dataGridView_preview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_preview.Location = new System.Drawing.Point(224, 58);
            this.dataGridView_preview.Name = "dataGridView_preview";
            this.dataGridView_preview.RowTemplate.Height = 23;
            this.dataGridView_preview.Size = new System.Drawing.Size(524, 361);
            this.dataGridView_preview.TabIndex = 7;
            // 
            // button_import_score
            // 
            this.button_import_score.Location = new System.Drawing.Point(53, 111);
            this.button_import_score.Name = "button_import_score";
            this.button_import_score.Size = new System.Drawing.Size(119, 23);
            this.button_import_score.TabIndex = 6;
            this.button_import_score.Text = "导入成绩";
            this.button_import_score.UseVisualStyleBackColor = true;
            this.button_import_score.Click += new System.EventHandler(this.button_import_score_Click);
            // 
            // ScoreImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_save);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_template);
            this.Controls.Add(this.dataGridView_preview);
            this.Controls.Add(this.button_import_score);
            this.Name = "ScoreImportForm";
            this.Text = "ScoreImportForm";
            this.Load += new System.EventHandler(this.ScoreImportForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_preview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_template;
        private System.Windows.Forms.DataGridView dataGridView_preview;
        private System.Windows.Forms.Button button_import_score;
    }
}