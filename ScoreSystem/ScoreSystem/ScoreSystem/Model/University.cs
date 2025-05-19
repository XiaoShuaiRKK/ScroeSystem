using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class University
    {
        private long id;
        private string name;
        private long universityLevel;
        private double scienceScoreLine;
        private double artScoreLine;
        private int year;

        public long Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public long UniversityLevel { get => universityLevel; set => universityLevel = value; }
        public double ScienceScoreLine { get => scienceScoreLine; set => scienceScoreLine = value; }
        public double ArtScoreLine { get => artScoreLine; set => artScoreLine = value; }
        public int Year { get => year; set => year = value; }
    }
}
