using ScoreSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Data
{
    public class ScoreTrendFormManage
    {
        private static readonly Dictionary<string, ScoreTrendForm> OpenForms = new Dictionary<string, ScoreTrendForm>();
        private static string currentStudentNumber = null;
        private static ScoreTrendForm currentForm = null;

        public static void ShowTrendForm(Student student)
        {
            if (student == null) return;

            string studentNumber = student.StudentNumber;

            // 如果当前窗体仍在并且是同一学生，直接前置
            if (currentForm != null && !currentForm.IsDisposed)
            {
                if (currentStudentNumber == studentNumber)
                {
                    currentForm.BringToFront();
                    return;
                }
                else
                {
                    currentForm.Close();
                }
            }

            // 打开新窗体
            var newForm = new ScoreTrendForm(student);
            currentForm = newForm;
            currentStudentNumber = studentNumber;

            // 清理逻辑
            newForm.FormClosed += (s, e) =>
            {
                if (currentForm == newForm)
                {
                    currentForm = null;
                    currentStudentNumber = null;
                }
            };

            newForm.Show();
        }

    }
}
