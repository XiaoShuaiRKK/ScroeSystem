using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class StudentClassHistory
    {
        private string studentNumber;
        private long classId;
        private int grade;
        private int year;

        public string StudentNumber { get => studentNumber; set => studentNumber = value; }
        public long ClassId { get => classId; set => classId = value; }
        public int Grade { get => grade; set => grade = value; }
        public int Year { get => year; set => year = value; }
    }
}
