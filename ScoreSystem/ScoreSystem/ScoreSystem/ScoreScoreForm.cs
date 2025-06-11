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
        private FormAutoScaler autoScaler;
        private ScoreMainForm scoreMainForm;
        private List<Exam> exams;
        private List<ExamSubjectThreshold> thresholds;
        private List<ClassEntity> classEntities;
        private List<StudentRanking> studentRankings;
        private ScoreService scoreService = ScoreService.GetIntance();
        private ClassService classService = ClassService.GetIntance();
        private RankingService rankingService = RankingService.GetIntance();
        private bool isLoaded = false;
        // 类成员部分
        private Dictionary<string, double> originalScores = new Dictionary<string, double>();
        //print
        private int currentRowIndex; // 确保这是类级变量
        private PrintDocument printDocument = new PrintDocument(); // 确保这是类级变量
        private int rowHeight = 30;
        private bool isEditing = false;
        private List<string> editableColumns = new List<string>();

        //private Bitmap dgvBitmap;




        public ScoreScoreForm(ScoreMainForm scoreMainForm)
        {
            this.scoreMainForm = scoreMainForm;
            InitializeComponent();
            autoScaler = new FormAutoScaler(this);
            printDocument.PrintPage += PrintDocument_PrintPage;
            menu_rank.Enabled = false;
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
                Name = $"{e.Name}（{(Enum.IsDefined(typeof(GradeEnum), e.Grade) ? ((GradeEnum)e.Grade).ToString() : "未知年级")}）（{e.Year}）"
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
            editableColumns.Clear();

            foreach (var courseId in allCourseIds)
            {
                string name = GetCourseName(courseId);
                courseNameMap[courseId] = name;

                columnNames.Add(name);                      // 成绩列
                columnNames.Add($"{name}-班排");           // 排名列
                columnNames.Add($"{name}-年排");

                if (courseId >= 0) // 仅允许普通单科成绩列可编辑，排除 -1/-2/-3 这些汇总列
                {
                    editableColumns.Add(name);
                }
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

            // 初始全部列只读
            foreach (DataGridViewColumn col in dataGridView_score.Columns)
            {
                col.ReadOnly = true;
            }
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

            using (Font font = new Font("Arial", 6f)) // 字体大小适中
            using (Pen pen = new Pen(Color.Black))
            {
                Brush brush = Brushes.Black;

                int columnCount = dataGridView_score.Columns.Count;
                int[] columnWidths = new int[columnCount];

                int averageWidth = printableWidth / columnCount;
                for (int i = 0; i < columnCount; i++)
                {
                    columnWidths[i] = averageWidth;
                }

                int xStart = leftMargin;

                while (currentRowIndex < dataGridView_score.Rows.Count)
                {
                    DataGridViewRow row = dataGridView_score.Rows[currentRowIndex];
                    if (dataGridView_score.AllowUserToAddRows && row.IsNewRow)
                    {
                        currentRowIndex++;
                        continue;
                    }

                    int x = xStart;

                    // 第一行：打印列头
                    for (int i = 0; i < columnCount; i++)
                    {
                        Rectangle rect = new Rectangle(x, y, columnWidths[i], rowHeight);
                        e.Graphics.FillRectangle(Brushes.LightGray, rect);
                        e.Graphics.DrawRectangle(pen, rect);

                        string headerText = dataGridView_score.Columns[i].HeaderText ?? "";
                        e.Graphics.DrawString(headerText, font, brush, rect, new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        });

                        x += columnWidths[i];
                    }

                    y += rowHeight;
                    x = xStart;

                    // 第二行：打印学生数据
                    for (int i = 0; i < columnCount; i++)
                    {
                        Rectangle rect = new Rectangle(x, y, columnWidths[i], rowHeight);
                        e.Graphics.DrawRectangle(pen, rect);

                        string valueText = row.Cells[i].Value?.ToString() ?? "";
                        e.Graphics.DrawString(valueText, font, brush, rect, new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        });

                        x += columnWidths[i];
                    }

                    y += rowHeight + 10; // 增加间隔
                    currentRowIndex++;

                    // 如果剩余空间不足以再打印下一组（两行），则分页
                    if (y + 2 * rowHeight > e.MarginBounds.Bottom)
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

        private async void button_edit_Click(object sender, EventArgs e)
        {
            if (!isEditing)
            {
                // 进入编辑状态
                isEditing = true;
                button_edit.Text = "保存";

                dataGridView_score.ReadOnly = false;
                // 允许可编辑列编辑，禁止其它列编辑
                foreach (DataGridViewColumn col in dataGridView_score.Columns)
                {
                    string header = col.HeaderText;
                    if (editableColumns.Contains(header) && !header.Contains("-班排") && !header.Contains("-年排"))
                        col.ReadOnly = false;
                    else
                        col.ReadOnly = true;
                }

                CacheOriginalScores();
            }
            else
            {
                // 退出编辑状态，保存数据
                isEditing = false;
                button_edit.Text = "修改";

                int currentExamId = (int)comboBox_exam.SelectedValue;
                List<ScoreEntity> updatedScores = new List<ScoreEntity>();

                foreach (DataGridViewRow row in dataGridView_score.Rows)
                {
                    if (row.IsNewRow) continue;

                    string studentNumber = row.Cells["学号"]?.Value?.ToString();
                    if (string.IsNullOrEmpty(studentNumber)) continue;

                    foreach (DataGridViewColumn col in dataGridView_score.Columns)
                    {
                        string courseName = col.HeaderText;

                        if (!editableColumns.Contains(courseName) || courseName.Contains("-班排") || courseName.Contains("-年排"))
                            continue;

                        object val = row.Cells[col.Index].Value;
                        if (val == null) continue;

                        if (double.TryParse(val.ToString(), out double newScore))
                        {
                            string key = $"{studentNumber}_{courseName}";

                            if (!originalScores.TryGetValue(key, out double oldScore) || oldScore != newScore)
                            {
                                if (Enum.TryParse<CourseEnum>(courseName, out CourseEnum courseEnum))
                                {
                                    updatedScores.Add(new ScoreEntity
                                    {
                                        StudentNumber = studentNumber,
                                        CourseId = (int)courseEnum + 1,
                                        ExamId = currentExamId,
                                        Score = newScore
                                    });
                                }
                            }
                        }
                    }
                }

                if (updatedScores.Count > 0)
                {
                    bool result = await scoreService.UpdateScoreList(updatedScores);
                    if (result)
                    {
                        MessageBox.Show("成绩保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ScoreInit(); // 刷新数据
                    }
                }
                else
                {
                    MessageBox.Show("没有修改任何成绩，无需保存。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // 保存后全部列只读
                foreach (DataGridViewColumn col in dataGridView_score.Columns)
                {
                    col.ReadOnly = true;
                }
            }
        }

        // 缓存当前所有可编辑成绩的原始值，方便比较是否修改
        private void CacheOriginalScores()
        {
            originalScores.Clear();

            foreach (DataGridViewRow row in dataGridView_score.Rows)
            {
                if (row.IsNewRow) continue;

                string studentNumber = row.Cells["学号"]?.Value?.ToString();
                if (string.IsNullOrEmpty(studentNumber)) continue;

                foreach (DataGridViewColumn col in dataGridView_score.Columns)
                {
                    string courseName = col.HeaderText;

                    if (!editableColumns.Contains(courseName) || courseName.Contains("-班排") || courseName.Contains("-年排"))
                        continue;

                    if (double.TryParse(row.Cells[col.Index].Value?.ToString(), out double score))
                    {
                        string key = $"{studentNumber}_{courseName}";
                        originalScores[key] = score;
                    }
                }
            }
        }
    }
}
