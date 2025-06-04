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
    public partial class ScoreScoreForm : Form
    {
        private ScoreMainForm scoreMainForm;
        private List<Exam> exams;
        private List<ExamSubjectThreshold> thresholds;
        private List<ClassEntity> classEntities;
        private List<StudentRanking> studentRankings;
        private ScoreService scoreService = ScoreService.GetIntance();
        private ClassService classService = ClassService.GetIntance();
        private RankingService rankingService = RankingService.GetIntance();
        private bool isLoaded = false;
        //print
        private int currentRowIndex; // 确保这是类级变量
        private PrintDocument printDocument = new PrintDocument(); // 确保这是类级变量
        private int rowHeight = 30;
        //private Bitmap dgvBitmap;




        public ScoreScoreForm(ScoreMainForm scoreMainForm)
        {
            this.scoreMainForm = scoreMainForm;
            InitializeComponent();
            printDocument.PrintPage += PrintDocument_PrintPage;
        }

        private void ScoreScoreForm_Load(object sender, EventArgs e)
        {
            this.Text = $"{ProjectSystemData.SYSTEM_NAME} - 分数管理";
            comboBox_class.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_exam.DropDownStyle = ComboBoxStyle.DropDownList;
            dataGridView_score.ReadOnly = true;
            printDocument.DefaultPageSettings.Landscape = true;
            this.FormClosed += (s, ev) =>
            {
                scoreMainForm.Show();
                this.Dispose();
            };
            ComboxDataInit();
        }

        private async void ComboxDataInit()
        {
            comboBox_exam.DataSource = null;
            exams = await scoreService.GetExams();
            var examDisplayList = exams.Select(e => new
            {
                Id = e.Id,
                Name = $"{e.Name}（{(Enum.IsDefined(typeof(GradeEnum), e.Grade) ? ((GradeEnum)e.Grade).ToString() : "未知年级")}）"
            }).ToList();
            comboBox_exam.DataSource = examDisplayList;
            comboBox_exam.DisplayMember = "Name";
            comboBox_exam.ValueMember = "Id";

            comboBox_class.DataSource = null;
            classEntities = await classService.GetAllClasses();
            comboBox_class.DataSource = classEntities;
            comboBox_class.DisplayMember = "Name";
            comboBox_class.ValueMember = "Id";

            if (exams.Any())
                isLoaded = true;

            ScoreInit();
        }

        private async void ScoreInit()
        {
            if (!isLoaded) return;

            int examId = (int)comboBox_exam.SelectedValue;
            int classId = (int)comboBox_class.SelectedValue;

            studentRankings = await rankingService.GetAllRangkingByClass(examId, classId);
            thresholds = await scoreService.GetThresholds(examId);

            StringBuilder thresholdTextBuilder = new StringBuilder("各科达标线：");
            foreach (var t in thresholds)
            {
                string courseName = GetCourseName(t.CourseId);
                thresholdTextBuilder.Append($"{courseName}: {t.ThresholdScore}  ");
            }
            label_threshold.Text = thresholdTextBuilder.ToString();

            // 自定义科目顺序
            List<int> customCourseOrder = new List<int>
            {
                (int)CourseEnum.语文,
                (int)CourseEnum.数学,
                (int)CourseEnum.英语,
                (int)CourseEnum.物理,
                (int)CourseEnum.化学,
                (int)CourseEnum.生物,
                (int)CourseEnum.政治,
                (int)CourseEnum.历史,
                (int)CourseEnum.地理,
                -2, // 三科总分
                -1, // 总分
                -3  // 3+1+2总分
            };

            var allCourseIds = customCourseOrder
                .Where(cid => studentRankings.SelectMany(s => s.Ranks.Select(r => r.CourseId)).Contains(cid))
                .ToList();

            List<string> columnNames = new List<string> { "姓名", "学号" };
            Dictionary<int, string> courseNameMap = new Dictionary<int, string>();

            foreach (var courseId in allCourseIds)
            {
                string name = GetCourseName(courseId);
                courseNameMap[courseId] = name;
                columnNames.Add(name);
                columnNames.Add($"{name}-班排");
                columnNames.Add($"{name}-年排");
            }

            DataTable dt = new DataTable();
            foreach (var col in columnNames)
            {
                dt.Columns.Add(col);
            }

            foreach (var student in studentRankings)
            {
                DataRow row = dt.NewRow();
                row["姓名"] = student.StudentName;
                row["学号"] = student.StudentNumber;

                foreach (var courseId in allCourseIds)
                {
                    string cname = courseNameMap[courseId];
                    var classRank = student.Ranks.FirstOrDefault(r => r.CourseId == courseId && r.Scope == "班级");
                    var gradeRank = student.Ranks.FirstOrDefault(r => r.CourseId == courseId && r.Scope == "年级");

                    if (classRank != null)
                        row[cname] = classRank.Score;

                    if (classRank != null)
                        row[$"{cname}-班排"] = $"{classRank.Rank}/{classRank.Total}";

                    if (gradeRank != null)
                        row[$"{cname}-年排"] = $"{gradeRank.Rank}/{gradeRank.Total}";
                }

                dt.Rows.Add(row);
            }

            dataGridView_score.DataSource = null;
            dataGridView_score.DataSource = dt;
        }

        private string GetCourseName(int courseId)
        {
            string courseName;
            switch (courseId)
            {
                case -1:
                    courseName = "总分";
                    break;
                case -2:
                    courseName = "三科总分";
                    break;
                case -3:
                    courseName = "3+1+2总分";
                    break;
                default:
                    courseName = Enum.IsDefined(typeof(CourseEnum), courseId - 1)
                        ? ((CourseEnum)courseId - 1).ToString()
                        : $"未知({courseId})";
                    break;
            }
            return courseName;
        }

        private void comboBox_exam_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScoreInit();
        }

        private void comboBox_class_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScoreInit();
        }

        private void button_back_Click(object sender, EventArgs e)
        {
            scoreMainForm.Show();
            this.Dispose();
        }

        private void menu_import_Click(object sender, EventArgs e)
        {
            new ScoreImportForm().ShowDialog();
            ScoreInit();
        }

        private void dataGridView_score_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView_score.DataSource == null || thresholds == null) return;

            // 获取当前列名（即课程名）
            string columnName = dataGridView_score.Columns[e.ColumnIndex].HeaderText;

            // 如果列名是科目名（排除“姓名”、“学号”、“班级”）
            if (Enum.TryParse<CourseEnum>(columnName, out CourseEnum courseEnumObj))
            {
                var courseEnum = courseEnumObj;
                int courseId = (int)courseEnum + 1;

                // 获取该课程的达标线
                var threshold = thresholds.FirstOrDefault(t => t.CourseId == courseId);
                if (threshold == null) return;

                // 当前单元格的值
                if (e.Value != null && double.TryParse(e.Value.ToString(), out double score))
                {
                    if (score < threshold.ThresholdScore)
                    {
                        e.CellStyle.BackColor = Color.Red;
                        e.CellStyle.ForeColor = Color.White;
                    }
                }
            }
        }

        private void menu_rank_Click(object sender, EventArgs e)
        {
            new ScoreRankingForm().ShowDialog();
        }

        private void menu_print_Click(object sender, EventArgs e)
        {
            if (dataGridView_score.DataSource == null || dataGridView_score.Rows.Count == 0)
            {
                MessageBox.Show("暂无数据可打印！");
                return;
            }

            currentRowIndex = 0;

            printDocument.PrintPage -= PrintDocument_PrintPage;
            printDocument.PrintPage += PrintDocument_PrintPage;
            printDocument.DefaultPageSettings.Landscape = true;

            // 打印预览
            using (PrintPreviewDialog previewDialog = new PrintPreviewDialog())
            {
                previewDialog.Document = printDocument;
                previewDialog.ShowDialog();
            }

            // 实际打印
            using (PrintDialog printDialog = new PrintDialog())
            {
                printDialog.Document = printDocument;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    currentRowIndex = 0;
                    printDocument.Print();
                }
            }
        }

        private void PrintData(PrintPageEventArgs e)
        {
            int topMargin = e.MarginBounds.Top;
            int leftMargin = e.MarginBounds.Left;
            int printableWidth = e.MarginBounds.Width;
            int y = topMargin;
            int rowHeight = 30;

            using (Font font = new Font("Arial", 6.5f)) // 字体略大一点
            using (Pen pen = new Pen(Color.Black))
            {
                Brush brush = Brushes.Black;

                int columnCount = dataGridView_score.Columns.Count;
                int[] columnWidths = new int[columnCount];

                // 计算每列宽度（按内容比例计算，或平均分配）
                int averageWidth = printableWidth / columnCount;
                for (int i = 0; i < columnCount; i++)
                {
                    columnWidths[i] = averageWidth;
                }

                int xStart = leftMargin;

                // 打印表头
                int x = xStart;
                for (int i = 0; i < columnCount; i++)
                {
                    Rectangle rect = new Rectangle(x, y, columnWidths[i], rowHeight);
                    e.Graphics.FillRectangle(Brushes.LightGray, rect);
                    e.Graphics.DrawRectangle(pen, rect);

                    string headerText = dataGridView_score.Columns[i].HeaderText ?? string.Empty;
                    e.Graphics.DrawString(headerText, font, brush, rect, new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center,
                        Trimming = StringTrimming.EllipsisCharacter
                    });

                    x += columnWidths[i];
                }

                y += rowHeight;

                // 打印数据行
                while (currentRowIndex < dataGridView_score.Rows.Count)
                {
                    DataGridViewRow row = dataGridView_score.Rows[currentRowIndex];

                    if (dataGridView_score.AllowUserToAddRows && row.IsNewRow)
                    {
                        currentRowIndex++;
                        continue;
                    }

                    x = xStart;
                    for (int i = 0; i < columnCount; i++)
                    {
                        Rectangle rect = new Rectangle(x, y, columnWidths[i], rowHeight);
                        e.Graphics.DrawRectangle(pen, rect);

                        string text = row.Cells[i].Value?.ToString() ?? string.Empty;
                        e.Graphics.DrawString(text, font, brush, rect, new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter
                        });

                        x += columnWidths[i];
                    }

                    currentRowIndex++;
                    y += rowHeight;

                    if (y + rowHeight > e.MarginBounds.Bottom)
                    {
                        e.HasMorePages = true;
                        return;
                    }
                }

                e.HasMorePages = false;
            }
        }


        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            PrintData(e);
        }

    }
}
