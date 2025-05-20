using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class Teacher
    {
        private string name;
        private string username;
        private string password;
        private string teacherNumber;
        private int state;

        public string Name { get => name; set => name = value; }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public string TeacherNumber { get => teacherNumber; set => teacherNumber = value; }
        public int State { get => state; set => state = value; }
    }
}
