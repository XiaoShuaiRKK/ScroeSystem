using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class ExamSubjectThreshold
    {
        private int id;
        private int examId;
        private int courseId;
        private double thresholdScore;

        public int Id { get => id; set => id = value; }
        public int ExamId { get => examId; set => examId = value; }
        public int CourseId { get => courseId; set => courseId = value; }
        public double ThresholdScore { get => thresholdScore; set => thresholdScore = value; }
    }
}
