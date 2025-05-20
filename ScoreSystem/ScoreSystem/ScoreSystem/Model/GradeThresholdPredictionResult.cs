using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class GradeThresholdPredictionResult
    {
        private string studentNumber;
        private string studentName;
        private List<ThresholdPredictionResult> predictionResults;

        public string StudentNumber { get => studentNumber; set => studentNumber = value; }
        public string StudentName { get => studentName; set => studentName = value; }
        public List<ThresholdPredictionResult> PredictionResults { get => predictionResults; set => predictionResults = value; }
    }
}
