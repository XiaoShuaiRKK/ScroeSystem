using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class StudentDTO
    {
        private string name;
        private string userName;
        private string password;
        private string studentNumber;
        private int classId;
        private int state;
        private DateTime enrollmentDate;
        private int subjectGroupId;
        private int electiveCourse1Id;
        private int electiveCourse2Id;
        private int year;

        public string Name { get => name; set => name = value; }
        public string UserName { get => userName; set => userName = value; }
        public string Password { get => password; set => password = value; }
        public string StudentNumber { get => studentNumber; set => studentNumber = value; }
        public int ClassId { get => classId; set => classId = value; }
        public int State { get => state; set => state = value; }
        public DateTime EnrollmentDate { get => enrollmentDate; set => enrollmentDate = value; }
        public int SubjectGroupId { get => subjectGroupId; set => subjectGroupId = value; }
        public int ElectiveCourse1Id { get => electiveCourse1Id; set => electiveCourse1Id = value; }
        public int ElectiveCourse2Id { get => electiveCourse2Id; set => electiveCourse2Id = value; }
        public int Year { get => year; set => year = value; }
    }
}
