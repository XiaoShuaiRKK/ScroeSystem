using ScoreSystem.Model;
using ScoreSystem.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScoreSystem
{
    public partial class GradeRankingControlForm : UserControl
    {
        private ScoreService scoreService = ScoreService.GetIntance();
        private List<Exam> exams;
        public GradeRankingControlForm()
        {
            InitializeComponent();
        }

        private void GradeRankingControlForm_Load(object sender, EventArgs e)
        {
            this.comboBox_exam.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_mode.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox_subject_group.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBoxInit();
        }

        private async void ComboBoxInit()
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

            comboBox_subject_group.DataSource = Enum.GetValues(typeof(SubjectGroupEnum))
                .Cast<SubjectGroupEnum>()
                .Select(s => new
                {
                    Value = (int)s,
                    Name = s.ToString()
                }).ToList();
            comboBox_subject_group.DisplayMember = "Name";
            comboBox_subject_group.ValueMember = "Value";

            // RankModeEnum绑定
            comboBox_mode.DataSource = Enum.GetValues(typeof(RankModeEnum))
                .Cast<RankModeEnum>()
                .Select(e => new
                {
                    Value = (int)e,
                    Name = e.ToString()
                })
                .ToList();
            comboBox_mode.DisplayMember = "Name";
            comboBox_mode.ValueMember = "Value";
        }
    }
}
