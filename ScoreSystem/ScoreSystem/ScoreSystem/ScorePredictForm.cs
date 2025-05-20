using ScoreSystem.Data;
using ScoreSystem.Model;
using ScoreSystem.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScoreSystem
{
    public partial class ScorePredictForm : Form
    {
        private bool isLoaded = false;
        private List<GradeThresholdPredictionResult> results;
        private UniversitySevice universitySevice = UniversitySevice.GetIntance();
        // 打印相关字段
        private PrintDocument printDocument = new PrintDocument();
        private int currentRowIndex = 0;
        private int rowHeight = 30;
        public ScorePredictForm()
        {
            InitializeComponent();
            printDocument.PrintPage += PrintDocument_PrintPage;
        }

        private void ScorePredictForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 上线预测";
            this.comboBox_grade.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_university_level.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBoxLoad();
        }

        private void ComboBoxLoad()
        {
            // 年级下拉
            comboBox_grade.DataSource = Enum.GetValues(typeof(GradeEnum))
                .Cast<GradeEnum>()
                .Select(g => new { Name = g.ToString(), Value = (int)g })
                .ToList();
            comboBox_grade.DisplayMember = "Name";
            comboBox_grade.ValueMember = "Value";

            // 大学等级下拉
            comboBox_university_level.DataSource = Enum.GetValues(typeof(UniversityLevelEnum))
                .Cast<UniversityLevelEnum>()
                .Where(u => u != UniversityLevelEnum.全部) // 去掉“全部”
                .Select(u => new { Name = u.ToString(), Value = (int)u })
                .ToList();
            comboBox_university_level.DisplayMember = "Name";
            comboBox_university_level.ValueMember = "Value";
            isLoaded = true;
            DataLoad();
        }

        private async void DataLoad()
        {
            if (!isLoaded) return;
            int grade = (int)comboBox_grade.SelectedValue;
            int selectedLevel = (int)comboBox_university_level.SelectedValue;
            results = await universitySevice.GetGradeThresholdPredictionResults(grade, selectedLevel);
            // 展示当前大学等级的预测数据
            var displayList = results.SelectMany(student => student.PredictionResults
                .Select(p => new
                {
                    学号 = student.StudentNumber,
                    姓名 = student.StudentName,
                    等级 = p.Level,
                    总考试次数 = p.TotalExams,
                    达标次数 = p.QualifiedExams,
                    概率 = Math.Round(p.Probability * 100, 2) + "%" // 格式化为百分比
                })
            ).ToList();

            dataGridView_predict.DataSource = null;
            dataGridView_predict.DataSource = displayList;
        }

        private void comboBox_university_level_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataLoad();
        }

        private void comboBox_grade_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataLoad();
        }

        private void menu_print_Click(object sender, EventArgs e)
        {
            if (dataGridView_predict.DataSource == null)
            {
                MessageBox.Show("暂无数据可打印！");
                return;
            }

            currentRowIndex = 0; // 重置行索引
            PrintPreviewDialog previewDialog = new PrintPreviewDialog
            {
                Document = printDocument
            };
            previewDialog.ShowDialog();
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            int topMargin = e.MarginBounds.Top;
            int leftMargin = e.MarginBounds.Left;
            int y = topMargin;
            int rowHeightLocal = 25;

            Font font = new Font("Arial", 8);
            Brush brush = Brushes.Black;
            Pen pen = Pens.Black;

            int colCount = dataGridView_predict.Columns.Count;

            // 1. 获取原始列宽总宽度
            int totalGridWidth = dataGridView_predict.Columns.Cast<DataGridViewColumn>().Sum(c => c.Width);

            // 2. 缩放每列宽度
            Dictionary<int, int> scaledColWidths = new Dictionary<int, int>();
            int printAreaWidth = e.MarginBounds.Width;
            foreach (DataGridViewColumn col in dataGridView_predict.Columns)
            {
                float colRatio = (float)col.Width / totalGridWidth;
                scaledColWidths[col.Index] = (int)(colRatio * printAreaWidth);
            }

            // 打印表头
            int x = leftMargin;
            foreach (DataGridViewColumn col in dataGridView_predict.Columns)
            {
                int colWidth = scaledColWidths[col.Index];
                Rectangle rect = new Rectangle(x, y, colWidth, rowHeightLocal);
                e.Graphics.DrawRectangle(pen, rect);

                StringFormat format = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                e.Graphics.DrawString(col.HeaderText, font, brush, rect, format);
                x += colWidth;
            }

            y += rowHeightLocal;

            // 打印行数据
            while (currentRowIndex < dataGridView_predict.Rows.Count)
            {
                DataGridViewRow row = dataGridView_predict.Rows[currentRowIndex];
                if (row.IsNewRow) break;

                x = leftMargin;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    int colWidth = scaledColWidths[cell.ColumnIndex];
                    Rectangle rect = new Rectangle(x, y, colWidth, rowHeightLocal);
                    e.Graphics.DrawRectangle(pen, rect);

                    string value = cell.Value?.ToString() ?? "";
                    StringFormat format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    e.Graphics.DrawString(value, font, brush, rect, format);

                    x += colWidth;
                }

                currentRowIndex++;
                y += rowHeightLocal;

                if (y + rowHeightLocal > e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }
            }

            e.HasMorePages = false;
        }

    }
}
