using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class ScoreEntity
    {
        private int id;
        private string studentNumber;
        private int courseId;
        private int examId;
        private double score;
        private string comment;

        public int Id { get => id; set => id = value; }
        public string StudentNumber { get => studentNumber; set => studentNumber = value; }
        public int CourseId { get => courseId; set => courseId = value; }
        public int ExamId { get => examId; set => examId = value; }
        public double Score { get => score; set => score = value; }
        public string Comment { get => comment; set => comment = value; }
    }
}
