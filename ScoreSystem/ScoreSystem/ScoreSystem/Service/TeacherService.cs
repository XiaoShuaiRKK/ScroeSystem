using NPOI.Util;
using ScoreSystem.Data;
using ScoreSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScoreSystem.Service
{
    public class TeacherService
    {
        public async Task<List<Student>> GetStudentByClassId(int classId)
        {
            string url = HttpUtil.GetUrl($"/teacher/get/student/byClass?class_id={classId}");
            string jsonResult = await HttpUtil.GetAsync(url);
            ApiResponse<List<Student>> response = JsonSerializer.Deserialize<ApiResponse<List<Student>>>(jsonResult, JsonUtil.GetOptions());
            if (response.Code == 200)
            {
                return response.Data;
            }
            else
            {
                MessageBox.Show(response.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Student>();
            };
        }

        public async Task<Student> GetStudent(string studentNumber)
        {
            string url = HttpUtil.GetUrl($"/teacher/get/student?student_number={studentNumber}");
            string jsonResult = await HttpUtil.GetAsync(url);
            ApiResponse<Student> response = JsonSerializer.Deserialize<ApiResponse<Student>>(jsonResult, JsonUtil.GetOptions());
            if (response.Code == 200)
            {
                return response.Data;
            }
            else
            {
                MessageBox.Show(response.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            };
        }
    }
}
