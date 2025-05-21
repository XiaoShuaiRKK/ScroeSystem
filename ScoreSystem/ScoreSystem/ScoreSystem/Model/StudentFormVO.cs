using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class StudentFormVO
    {
        private string name;
        private string userName;
        private string password;
        private string studentNumber;
        private string className;
        private string state;
        private DateTime enrollmentDate;
        private string subjectGroupName;
        private string electiveCourse1Name;
        private string electiveCourse2Name;
        private int year;

        public string Name { get => name; set => name = value; }
        public string UserName { get => userName; set => userName = value; }
        public string Password { get => password; set => password = value; }
        public string StudentNumber { get => studentNumber; set => studentNumber = value; }
        public string ClassName { get => className; set => className = value; }
        public string State { get => state; set => state = value; }
        public DateTime EnrollmentDate { get => enrollmentDate; set => enrollmentDate = value; }
        public string SubjectGroupName { get => subjectGroupName; set => subjectGroupName = value; }
        public string ElectiveCourse1Name { get => electiveCourse1Name; set => electiveCourse1Name = value; }
        public string ElectiveCourse2Name { get => electiveCourse2Name; set => electiveCourse2Name = value; }
        public int Year { get => year; set => year = value; }
    }
}
