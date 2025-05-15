using ScoreSystem.Data;
using ScoreSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScoreSystem.Service
{
    public class StudentService
    {
        private static StudentService intance;
        private StudentService() { }
        public static StudentService GetIntance()
        {
            if(intance == null)
            {
                intance = new StudentService();
            }
            return intance;
        }

        public async Task<bool> AddStudent(List<StudentDTO> students)
        {
            string url = HttpUtil.GetUrl("/user/batch/add/student");
            var studentList = students.Select(s => new
            {
                name = s.Name,
                userName = s.UserName,
                password = s.Password,
                studentNumber = s.StudentNumber,
                classId = s.ClassId,
                state = s.State,
                enrollmentDate = s.EnrollmentDate.ToString("yyyy-MM-dd"), // 关键格式化
                subjectGroupId = s.SubjectGroupId,
                electiveCourse1Id = s.ElectiveCourse1Id,
                electiveCourse2Id = s.ElectiveCourse2Id
            }).ToList();

            // 序列化为 JSON
            string jsonBody = JsonSerializer.Serialize(studentList, JsonUtil.GetRequestOptions());
            string jsonResult = await HttpUtil.PostAsync(url, jsonBody) ;
            var response = JsonSerializer.Deserialize<ApiResponse<bool?>>(jsonResult,JsonUtil.GetOptions());
            if(response.Code == 200 && response.Data == true)
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
