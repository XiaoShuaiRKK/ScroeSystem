using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class CriticalConfigDTO
    {
        private long id;
        private int grade;
        private int year;
        private int universityLevel;
        private int targetCount;
        private int floatUpCount;
        private int floatDownCount;
        private long subjectGroupId;

        public long Id { get => id; set => id = value; }
        public int Grade { get => grade; set => grade = value; }
        public int Year { get => year; set => year = value; }
        public int UniversityLevel { get => universityLevel; set => universityLevel = value; }
        public int TargetCount { get => targetCount; set => targetCount = value; }
        public int FloatUpCount { get => floatUpCount; set => floatUpCount = value; }
        public int FloatDownCount { get => floatDownCount; set => floatDownCount = value; }
        public long SubjectGroupId { get => subjectGroupId; set => subjectGroupId = value; }
    }
}
