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

        public async Task<List<StudentClassHistory>> GetStudentClassHistoryByNumber(string studentNumber)
        {
            string url = HttpUtil.GetUrl($"/teacher/getClass/history?student_number={studentNumber}");
            string jsonResult = await HttpUtil.GetAsync(url);
            ApiResponse<List<StudentClassHistory>> response = JsonSerializer.Deserialize<ApiResponse<List<StudentClassHistory>>>(jsonResult, JsonUtil.GetOptions());
            if (response.Code == 200)
            {
                return response.Data;
            }
            else
            {
                MessageBox.Show(response.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<StudentClassHistory>();
            };
        }

        public async Task<bool> AddTeacher(List<Teacher> teachers)
        {
            string url = HttpUtil.GetUrl("/user/batch/add/teacher");
            // 序列化为 JSON
            string jsonBody = JsonSerializer.Serialize(teachers, JsonUtil.GetRequestOptions());
            string jsonResult = await HttpUtil.PostAsync(url, jsonBody);
            var response = JsonSerializer.Deserialize<ApiResponse<bool?>>(jsonResult, JsonUtil.GetOptions());
            if (response.Code == 200 && response.Data == true)
            {
                return true;
            }
            else
            {
                MessageBox.Show(response.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public async Task<List<TeacherVO>> GetTeachers()
        {
            string url = HttpUtil.GetUrl($"/user/get/teachers");
            string jsonResult = await HttpUtil.GetAsync(url);
            ApiResponse<List<TeacherVO>> response = JsonSerializer.Deserialize<ApiResponse<List<TeacherVO>>>(jsonResult, JsonUtil.GetOptions());
            if (response.Code == 200)
            {
                return response.Data;
            }
            else
            {
                MessageBox.Show(response.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<TeacherVO>();
            };
        }

        public async Task<bool> DeleteTeacher(Teacher teacher)
        {
            string url = HttpUtil.GetUrl("/user/delete/teacher");
            // 序列化为 JSON
            string jsonBody = JsonSerializer.Serialize(teacher, JsonUtil.GetRequestOptions());
            string jsonResult = await HttpUtil.PostAsync(url, jsonBody);
            var response = JsonSerializer.Deserialize<ApiResponse<bool?>>(jsonResult, JsonUtil.GetOptions());
            if (response.Code == 200 && response.Data == true)
            {
                return true;
            }
            else
            {
                MessageBox.Show(response.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
