using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class ClassEntity
    {
        private int id;
        private string name;
        private int grade;
        private int subjectGroupId;
        private long headTeacherId;
        private string teacherName;

        public string Name { get => name; set => name = value; }
        public int Grade { get => grade; set => grade = value; }
        public int SubjectGroupId { get => subjectGroupId; set => subjectGroupId = value; }
        public long HeadTeacherId { get => headTeacherId; set => headTeacherId = value; }
        public string TeacherName { get => teacherName; set => teacherName = value; }
        public int Id { get => id; set => id = value; }
    }
}
