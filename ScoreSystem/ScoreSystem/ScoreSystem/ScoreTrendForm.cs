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
        private Student student;
        private StudentScoreTrendResult trendResult;
        private List<StudentClassHistory> classHistories;
        private TeacherService teacherService = new TeacherService();
        private TrendService trendService = new TrendService();

        public ScoreTrendForm(Student student)
        {
            this.student = student;
            InitializeComponent();
        }

        private async void ScoreTrendForm_Load(object sender, EventArgs e)
        {
            label_name.Text = $"学生名字: {student.Name}";
            label_student_number.Text = $"学号: {student.StudentNumber}";
            comboBox_subject.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_grade.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_mode.DropDownStyle = ComboBoxStyle.DropDownList;

            LoadRankModes(); // 👈 加载模式 ComboBox
            await LoadGradesAsync(); // 加载年级列表
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

            var modeItems = allowedModes
                .Select(mode => new
                {
                    Text = mode.ToString(),
                    Value = (int)mode
                })
                .ToList();

            comboBox_mode.DataSource = modeItems;
            comboBox_mode.DisplayMember = "Text";
            comboBox_mode.ValueMember = "Value";

            // 可选：默认选中第一项
            comboBox_mode.SelectedIndex = 0;
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
                // 获取各科趋势数据
                trendResult = await trendService.GetStudentCourseScoreTrend(student.StudentNumber, selectedGrade);

                if (trendResult == null || trendResult.Trend == null || trendResult.Trend.Count == 0)
                {
                    MessageBox.Show("未找到该学生的各科成绩趋势数据。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    comboBox_subject.DataSource = null;
                    return;
                }

                // 显示科目选择下拉框
                comboBox_subject.Visible = true;

                LoadSubjects(); // 加载科目下拉框
                comboBox_subject_SelectedIndexChanged(comboBox_subject, EventArgs.Empty); // 绘图
            }
            else if (selectedMode == RankModeEnum.三加一加二总分)
            {
                // 获取总分趋势数据
                var totalTrend = await trendService.GetStudent312ScoreTrend(student.StudentNumber, selectedGrade);

                if (totalTrend == null || totalTrend.Count == 0)
                {
                    MessageBox.Show("未找到该学生的总分趋势数据。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 隐藏科目选择下拉框
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
        }
    }
}
