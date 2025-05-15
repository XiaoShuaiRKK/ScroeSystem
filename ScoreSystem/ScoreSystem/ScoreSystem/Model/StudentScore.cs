using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class StudentScore
    {
        private string name;
        private string studentNumber;
        private string className;
        private List<ScoreEntity> scores;

        public string Name { get => name; set => name = value; }
        public string StudentNumber { get => studentNumber; set => studentNumber = value; }
        public string ClassName { get => className; set => className = value; }
        public List<ScoreEntity> Scores { get => scores; set => scores = value; }
    }
}
