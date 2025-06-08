using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class ExamClassSubjectStat
    {
        private long id;
        private long examId;
        private long courseId;
        private int classId;
        private string className;
        private int universityLevel;
        private double avgScore;
        private double synergyRate;
        private int synergyCount;
        private double contributionRate;
        private int contributionCount;

        public long Id { get => id; set => id = value; }
        public long ExamId { get => examId; set => examId = value; }
        public long CourseId { get => courseId; set => courseId = value; }
        public int ClassId { get => classId; set => classId = value; }
        public int UniversityLevel { get => universityLevel; set => universityLevel = value; }
        public double AvgScore { get => avgScore; set => avgScore = value; }
        public double SynergyRate { get => synergyRate; set => synergyRate = value; }
        public int SynergyCount { get => synergyCount; set => synergyCount = value; }
        public double ContributionRate { get => contributionRate; set => contributionRate = value; }
        public int ContributionCount { get => contributionCount; set => contributionCount = value; }
        public string ClassName { get => className; set => className = value; }
    }
}
