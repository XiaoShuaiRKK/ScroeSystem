using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class StudentScoreTrendResult
    {
        private string studentName;
        private string studentNumber;
        private Dictionary<string, List<ScoreTrendPoint>> trend;

        public string StudentName { get => studentName; set => studentName = value; }
        public string StudentNumber { get => studentNumber; set => studentNumber = value; }
        public Dictionary<string, List<ScoreTrendPoint>> Trend { get => trend; set => trend = value; }
    }
}
