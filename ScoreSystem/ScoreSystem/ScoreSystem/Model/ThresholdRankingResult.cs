using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class ThresholdRankingResult
    {
        Dictionary<string, int> levelCounts;
        Dictionary<string, double> levelRates;
        Dictionary<string, List<StudentRanking>> levelStudentList;

        public Dictionary<string, int> LevelCounts { get => levelCounts; set => levelCounts = value; }
        public Dictionary<string, double> LevelRates { get => levelRates; set => levelRates = value; }
        public Dictionary<string, List<StudentRanking>> LevelStudentList { get => levelStudentList; set => levelStudentList = value; }
    }
}
