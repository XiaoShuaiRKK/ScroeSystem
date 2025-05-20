namespace ScoreSystem
{
    partial class ScoreClassForm
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
            this.dataGridView_class = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menu_class_import = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_class_edit = new System.Windows.Forms.ToolStripMenuItem();
            this.button_back = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_class)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView_class
            // 
            this.dataGridView_class.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_class.Location = new System.Drawing.Point(29, 68);
            this.dataGridView_class.Name = "dataGridView_class";
            this.dataGridView_class.RowTemplate.Height = 23;
            this.dataGridView_class.Size = new System.Drawing.Size(744, 355);
            this.dataGridView_class.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_class_import,
            this.menu_class_edit});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menu_class_import
            // 
            this.menu_class_import.Name = "menu_class_import";
            this.menu_class_import.Size = new System.Drawing.Size(68, 21);
            this.menu_class_import.Text = "班级导入";
            this.menu_class_import.Click += new System.EventHandler(this.menu_class_import_Click);
            // 
            // menu_class_edit
            // 
            this.menu_class_edit.Name = "menu_class_edit";
            this.menu_class_edit.Size = new System.Drawing.Size(68, 21);
            this.menu_class_edit.Text = "设置班级";
            // 
            // button_back
            // 
            this.button_back.Location = new System.Drawing.Point(29, 39);
            this.button_back.Name = "button_back";
            this.button_back.Size = new System.Drawing.Size(75, 23);
            this.button_back.TabIndex = 7;
            this.button_back.Text = "<  返回";
            this.button_back.UseVisualStyleBackColor = true;
            this.button_back.Click += new System.EventHandler(this.button_back_Click);
            // 
            // ScoreClassForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button_back);
            this.Controls.Add(this.dataGridView_class);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ScoreClassForm";
            this.Text = "ScoreClassForm";
            this.Load += new System.EventHandler(this.ScoreClassForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_class)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView_class;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menu_class_import;
        private System.Windows.Forms.ToolStripMenuItem menu_class_edit;
        private System.Windows.Forms.Button button_back;
    }
}