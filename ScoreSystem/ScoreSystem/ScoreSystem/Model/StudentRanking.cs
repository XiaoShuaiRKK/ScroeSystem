using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class StudentRanking
    {
        private string studentNumber;
        private string studentName;
        private List<Ranking> ranks;

        public string StudentNumber { get => studentNumber; set => studentNumber = value; }
        public string StudentName { get => studentName; set => studentName = value; }
        public List<Ranking> Ranks { get => ranks; set => ranks = value; }
    }
}
