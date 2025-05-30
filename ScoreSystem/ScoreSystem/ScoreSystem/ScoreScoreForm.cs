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
        private PrintDocument printDocument = new PrintDocument();
        private int currentRowIndex = 0;
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
                        row[$"{cname}-班排"] = classRank.Rank;

                    if (gradeRank != null)
                        row[$"{cname}-年排"] = gradeRank.Rank;
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
                    courseName = Enum.IsDefined(typeof(CourseEnum), courseId)
                        ? ((CourseEnum)courseId).ToString()
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
                int courseId = (int)courseEnum;

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
            if (dataGridView_score.DataSource == null)
            {
                MessageBox.Show("暂无数据可打印！");
                return;
            }

            currentRowIndex = 0; // 重置行索引

            PrintPreviewDialog previewDialog = new PrintPreviewDialog();
            previewDialog.Document = printDocument;
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

            int totalGridWidth = dataGridView_score.Columns.Cast<DataGridViewColumn>().Sum(c => c.Width);
            int printAreaWidth = e.MarginBounds.Width;

            Dictionary<int, int> scaledColWidths = new Dictionary<int, int>();
            foreach (DataGridViewColumn col in dataGridView_score.Columns)
            {
                float colRatio = (float)col.Width / totalGridWidth;
                scaledColWidths[col.Index] = (int)(colRatio * printAreaWidth);
            }

            int x = leftMargin;
            foreach (DataGridViewColumn col in dataGridView_score.Columns)
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

            while (currentRowIndex < dataGridView_score.Rows.Count)
            {
                DataGridViewRow row = dataGridView_score.Rows[currentRowIndex];
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
