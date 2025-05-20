using Org.BouncyCastle.Bcpg.Sig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class CriticalStudentLog
    {
        private long id;
        private long examId;
        private string studentNumber;
        private string studentName;
        private int universityLevel;
        private int scoreRank;
        private int targetRank;
        private int gap;
        private double score;
        private long subjectGroupId;

        public long Id { get => id; set => id = value; }
        public long ExamId { get => examId; set => examId = value; }
        public string StudentNumber { get => studentNumber; set => studentNumber = value; }
        public string StudentName { get => studentName; set => studentName = value; }
        public int UniversityLevel { get => universityLevel; set => universityLevel = value; }
        public int ScoreRank { get => scoreRank; set => scoreRank = value; }
        public int TargetRank { get => targetRank; set => targetRank = value; }
        public int Gap { get => gap; set => gap = value; }
        public double Score { get => score; set => score = value; }
        public long SubjectGroupId { get => subjectGroupId; set => subjectGroupId = value; }
    }
}
