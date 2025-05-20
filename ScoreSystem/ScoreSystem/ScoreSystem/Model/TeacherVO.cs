using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class TeacherVO
    {
        private long id;
        private long userId;
        private string name;
        private int state;
        private string teacherNumber;

        public long Id { get => id; set => id = value; }
        public long UserId { get => userId; set => userId = value; }
        public string Name { get => name; set => name = value; }
        public int State { get => state; set => state = value; }
        public string TeacherNumber { get => teacherNumber; set => teacherNumber = value; }
    }
}
