using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class ScoreTrendPoint
    {
        private long examId;
        private double score;
        private int rank;

        public long ExamId { get => examId; set => examId = value; }
        public double Score { get => score; set => score = value; }
        public int Rank { get => rank; set => rank = value; }
    }
}
