using ScoreSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Data
{
    public static class CourseHelper
    {
        public static string GetCourseNameById(int id)
        {
            var names = Enum.GetNames(typeof(CourseEnum));
            if (id >= 1 && id <= names.Length)
                return names[id - 1];
            return "未知";
        }

        public static int GetCourseIdByName(string name)
        {
            var names = Enum.GetNames(typeof(CourseEnum));
            int index = Array.IndexOf(names, name);
            return index >= 0 ? index + 1 : -1;
        }

        // 所有课程名列表
        public static List<string> GetAllCourseNames()
        {
            return Enum.GetNames(typeof(CourseEnum)).ToList();
        }
    }
}
