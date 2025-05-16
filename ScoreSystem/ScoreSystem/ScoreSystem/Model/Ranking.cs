using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class Ranking
    {
        private int courseId;
        private string courseName;
        private double score;
        private int rank;
        private int total;
        private string scope;

        public int CourseId { get => courseId; set => courseId = value; }
        public string CourseName { get => courseName; set => courseName = value; }
        public double Score { get => score; set => score = value; }
        public int Rank { get => rank; set => rank = value; }
        public int Total { get => total; set => total = value; }
        public string Scope { get => scope; set => scope = value; }
    }
}
