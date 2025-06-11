using ScoreSystem.Data;
using ScoreSystem.Model;
using ScoreSystem.Service;
using SixLabors.ImageSharp.Formats.Bmp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ScoreSystem
{
    public partial class ScoreTrendForm : Form
    {
        private FormAutoScaler autoScaler;
        private Student student;
        private StudentScoreTrendResult trendResult;
        private List<StudentClassHistory> classHistories;
        private TeacherService teacherService = TeacherService.GetIntance();
        private TrendService trendService = new TrendService();
        private ScoreStudentEditForm studentEditForm;

        public ScoreTrendForm(Student student)
        {
            this.student = student;
            InitializeComponent();
            autoScaler = new FormAutoScaler(this);
        }

        private async void ScoreTrendForm_Load(object sender, EventArgs e)
        {
            label_name.Text = $"学生名字: {student.Name}";
            label_student_number.Text = $"学号: {student.StudentNumber}";
            label_join_date.Text = $"入学时间: {student.EnrollmentDate}";
            label_course.Text = $"选科一: {((CourseEnum)student.ElectiveCourse1Id - 1)} 选科二: {((CourseEnum)student.ElectiveCourse2Id - 1)}";
            comboBox_subject.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_grade.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_mode.DropDownStyle = ComboBoxStyle.DropDownList;

            LoadRankModes(); // 👈 加载模式 ComboBox
            await LoadGradesAsync(); // 加载年级列表
            LoadDataGridView();
        }


        private async void LoadDataGridView()
        {
            dataGridView_history.DataSource = null;

            var selectedMode = GetSelectedMode();

            DataTable table = new DataTable();
            table.Columns.Add("考试名称");

            if (selectedMode == RankModeEnum.各科)
            {
                if (trendResult == null || trendResult.Trend == null || trendResult.Trend.Count == 0)
                    return;

                var subjects = trendResult.Trend.Keys.ToList();
                int examCount = trendResult.Trend.Values.First().Count;

                foreach (var subject in subjects)
                {
                    table.Columns.Add(subject);
                }

                for (int i = 0; i < examCount; i++)
                {
                    var row = table.NewRow();
                    row["考试名称"] = $"第{i + 1}次考试";

                    foreach (var subject in subjects)
                    {
                        var scoreList = trendResult.Trend[subject];
                        if (i < scoreList.Count)
                        {
                            row[subject] = scoreList[i].Score;
                        }
                    }

                    table.Rows.Add(row);
                }
                dataGridView_history.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            else if (selectedMode == RankModeEnum.三加一加二总分)
            {
                int selectedGrade = (int)comboBox_grade.SelectedValue;
                var totalTrend = await trendService.GetStudent312ScoreTrend(student.StudentNumber, selectedGrade);

                if (totalTrend == null || totalTrend.Count == 0)
                    return;

                table.Columns.Add("总分");

                int index = 1;
                foreach (var kvp in totalTrend.OrderBy(k => k.Key))
                {
                    var row = table.NewRow();
                    row["考试名称"] = $"第{index++}次考试";
                    row["总分"] = kvp.Value;
                    table.Rows.Add(row);
                }
                dataGridView_history.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }

            dataGridView_history.DataSource = table;
            
            dataGridView_history.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView_history.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        /// <summary>
        /// 加载 RankModeEnum 中“各科”和“三加一加二总分” 到 comboBox_mode
        /// </summary>
        private void LoadRankModes()
        {
            var allowedModes = new[]
           {
                RankModeEnum.各科,
                RankModeEnum.三加一加二总分
            };

            // 使用 KeyValuePair 明确类型
            var modeItems = allowedModes
                .Select(mode => new KeyValuePair<string, RankModeEnum>(mode.ToString(), mode))
                .ToList();

            comboBox_mode.SelectedIndexChanged -= comboBox_mode_SelectedIndexChanged;

            comboBox_mode.DataSource = modeItems;
            comboBox_mode.DisplayMember = "Key";
            comboBox_mode.ValueMember = "Value";

            comboBox_mode.SelectedIndex = 0;

            comboBox_mode.SelectedIndexChanged += comboBox_mode_SelectedIndexChanged;
        }

        /// <summary>
        /// 获取用户选择的模式
        /// </summary>
        private RankModeEnum GetSelectedMode()
        {
            return (RankModeEnum)comboBox_mode.SelectedValue;
        }

        // 加载年级列表
        private async Task LoadGradesAsync()
        {
            classHistories = await teacherService.GetStudentClassHistoryByNumber(student.StudentNumber);

            var distinctGrades = classHistories
                .Select(h => h.Grade)
                .Distinct()
                .Where(g => Enum.IsDefined(typeof(GradeEnum), g))
                .ToList();

            var gradeDisplayItems = distinctGrades
                .Select(g => new
                {
                    GradeValue = g,
                    GradeText = Enum.GetName(typeof(GradeEnum), g)
                })
                .ToList();

            comboBox_grade.DataSource = gradeDisplayItems;
            comboBox_grade.DisplayMember = "GradeText";
            comboBox_grade.ValueMember = "GradeValue";

            comboBox_grade.SelectedIndexChanged += comboBox_grade_SelectedIndexChanged;

            if (comboBox_grade.Items.Count > 0)
            {
                comboBox_grade.SelectedIndex = 0;

                // ✅ 自动触发加载趋势图
                comboBox_grade_SelectedIndexChanged(comboBox_grade, EventArgs.Empty);
            }
        }

        // 年级选择变更事件
        private async void comboBox_grade_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_grade.SelectedValue == null) return;

            int selectedGrade = (int)comboBox_grade.SelectedValue;
            RankModeEnum selectedMode = GetSelectedMode();

            chart_trend.Series.Clear();
            chart_trend.ChartAreas.Clear();

            var chartArea = new ChartArea("MainArea");
            chartArea.AxisX.Title = "考试";
            chartArea.AxisY.Title = "分数";
            chartArea.AxisY.Minimum = 0;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chart_trend.ChartAreas.Add(chartArea);

            if (selectedMode == RankModeEnum.各科)
            {
                trendResult = await trendService.GetStudentCourseScoreTrend(student.StudentNumber, selectedGrade);

                if (trendResult == null || trendResult.Trend == null || trendResult.Trend.Count == 0)
                {
                    MessageBox.Show("未找到该学生的各科成绩趋势数据。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    comboBox_subject.DataSource = null;
                    return;
                }

                comboBox_subject.Visible = true;

                LoadSubjects();
                comboBox_subject_SelectedIndexChanged(comboBox_subject, EventArgs.Empty);
            }
            else if (selectedMode == RankModeEnum.三加一加二总分)
            {
                var totalTrend = await trendService.GetStudent312ScoreTrend(student.StudentNumber, selectedGrade);

                if (totalTrend == null || totalTrend.Count == 0)
                {
                    MessageBox.Show("未找到该学生的总分趋势数据。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                comboBox_subject.Visible = false;

                var series = new Series("总分趋势")
                {
                    ChartType = SeriesChartType.Line,
                    Color = Color.DarkOrange,
                    BorderWidth = 2,
                    MarkerStyle = MarkerStyle.Circle,
                    MarkerSize = 7,
                    IsValueShownAsLabel = true
                };

                int index = 1;
                foreach (var entry in totalTrend.OrderBy(kvp => kvp.Key))
                {
                    string label = $"第{index++}次考试";
                    series.Points.AddXY(label, entry.Value);
                }

                chart_trend.Series.Add(series);
            }

            // ✅ 添加此行，确保表格也更新
            LoadDataGridView();
        }

        // 加载科目下拉框
        private void LoadSubjects()
        {
            var subjects = trendResult.Trend.Keys.ToList();
            subjects.Insert(0, "全部"); // 添加“全部”

            comboBox_subject.SelectedIndexChanged -= comboBox_subject_SelectedIndexChanged;
            comboBox_subject.DataSource = subjects;
            comboBox_subject.SelectedIndexChanged += comboBox_subject_SelectedIndexChanged;

            if (comboBox_subject.Items.Count > 0)
            {
                comboBox_subject.SelectedIndex = 0; // 默认选中“全部”
            }
        }

        // 科目选择事件
        private void comboBox_subject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (trendResult == null || comboBox_subject.SelectedItem == null)
                return;

            string selectedSubject = comboBox_subject.SelectedItem.ToString();

            chart_trend.Series.Clear();
            chart_trend.ChartAreas.Clear();

            var chartArea = new ChartArea("MainArea");
            chartArea.AxisX.Title = "考试";
            chartArea.AxisY.Title = "分数";
            chartArea.AxisY.Minimum = 0;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chart_trend.ChartAreas.Add(chartArea);

            if (selectedSubject == "全部")
            {
                // 展示所有科目的折线图
                var colorList = new[] { Color.Blue, Color.Red, Color.Green, Color.Orange, Color.Purple, Color.Brown };
                int colorIndex = 0;

                foreach (var subject in trendResult.Trend.Keys)
                {
                    var series = new Series(subject)
                    {
                        ChartType = SeriesChartType.Line,
                        Color = colorList[colorIndex % colorList.Length],
                        BorderWidth = 2,
                        MarkerStyle = MarkerStyle.Circle,
                        MarkerSize = 7,
                        IsValueShownAsLabel = true
                    };

                    var points = trendResult.Trend[subject];
                    for (int i = 0; i < points.Count; i++)
                    {
                        string label = $"第{i + 1}次考试";
                        series.Points.AddXY(label, points[i].Score);
                    }

                    chart_trend.Series.Add(series);
                    colorIndex++;
                }
            }
            else
            {
                if (!trendResult.Trend.ContainsKey(selectedSubject))
                    return;

                var points = trendResult.Trend[selectedSubject];
                var series = new Series(selectedSubject)
                {
                    ChartType = SeriesChartType.Line,
                    Color = Color.Blue,
                    BorderWidth = 2,
                    MarkerStyle = MarkerStyle.Circle,
                    MarkerSize = 7,
                    IsValueShownAsLabel = true
                };

                for (int i = 0; i < points.Count; i++)
                {
                    string label = $"第{i + 1}次考试";
                    series.Points.AddXY(label, points[i].Score);
                }

                chart_trend.Series.Add(series);
            }
            LoadDataGridView();
        }

        // 绘制所有科目的折线图
        private void DrawAllSubjects()
        {
            Color[] colors = { Color.Blue, Color.Red, Color.Green, Color.Orange, Color.Purple, Color.Brown };
            int colorIndex = 0;

            foreach (var kvp in trendResult.Trend)
            {
                string subject = kvp.Key;
                var points = kvp.Value;

                var series = new Series(subject)
                {
                    ChartType = SeriesChartType.Line,
                    Color = colors[colorIndex % colors.Length],
                    BorderWidth = 2,
                    MarkerStyle = MarkerStyle.Circle,
                    MarkerSize = 7,
                    IsValueShownAsLabel = true
                };

                for (int i = 0; i < points.Count; i++)
                {
                    string label = $"第{i + 1}次考试";
                    series.Points.AddXY(label, points[i].Score);
                }

                chart_trend.Series.Add(series);
                colorIndex++;
            }
        }

        // 绘制单科目折线图
        private void DrawSingleSubject(string subject, List<ScoreTrendPoint> points)
        {
            var series = new Series(subject)
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                BorderWidth = 2,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 7,
                IsValueShownAsLabel = true
            };

            for (int i = 0; i < points.Count; i++)
            {
                string label = $"第{i + 1}次考试";
                series.Points.AddXY(label, points[i].Score);
            }

            chart_trend.Series.Add(series);
        }

        private void comboBox_mode_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 模式切换时自动刷新图表
            comboBox_grade_SelectedIndexChanged(comboBox_grade, EventArgs.Empty);
            LoadDataGridView();
        }

        private void menu_edit_Click(object sender, EventArgs e)
        {
            if(studentEditForm == null)
            {
                studentEditForm = new ScoreStudentEditForm(student);
                studentEditForm.Show();
                studentEditForm.FormClosed += (s, ex) =>
                {
                    studentEditForm = null;
                };
            }
            else
            {
                studentEditForm.BringToFront();
            }
        }
    }
}
