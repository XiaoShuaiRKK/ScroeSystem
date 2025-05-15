using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class Exam
    {
        private int id;
        private string name;
        private int grade;
        private DateTime startDate;
        private DateTime endDate;

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public int Grade { get => grade; set => grade = value; }
        public DateTime StartDate { get => startDate; set => startDate = value; }
        public DateTime EndDate { get => endDate; set => endDate = value; }
    }
}
