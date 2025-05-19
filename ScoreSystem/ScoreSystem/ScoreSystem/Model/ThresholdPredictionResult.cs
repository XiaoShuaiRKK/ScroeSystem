using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class ThresholdPredictionResult
    {
        private string level;
        private int totalExams;
        private int qualifiedExams;
        private double probability;

        public string Level { get => level; set => level = value; }
        public int TotalExams { get => totalExams; set => totalExams = value; }
        public int QualifiedExams { get => qualifiedExams; set => qualifiedExams = value; }
        public double Probability { get => probability; set => probability = value; }
    }
}
