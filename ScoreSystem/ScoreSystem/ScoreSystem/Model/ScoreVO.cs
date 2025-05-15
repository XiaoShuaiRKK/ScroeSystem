using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class ScoreVO
    {
        private string studentNumber;
        private string courseName;
        private string examName;
        private double score;
        private string comment;

        public string StudentNumber { get => studentNumber; set => studentNumber = value; }
        public string CourseName { get => courseName; set => courseName = value; }
        public string ExamName { get => examName; set => examName = value; }
        public double Score { get => score; set => score = value; }
        public string Comment { get => comment; set => comment = value; }
    }
}
