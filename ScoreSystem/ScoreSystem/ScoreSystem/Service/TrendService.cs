using NPOI.XSSF.Model;
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
    public class TrendService
    {

        public async Task<StudentScoreTrendResult> GetStudentCourseScoreTrend(string studentNumber,int grade)
        {
            string url = HttpUtil.GetUrl($"/student/trend/student/score-trend?student_number={studentNumber}&grade={grade}");
            string jsonResult = await HttpUtil.GetAsync(url);
            ApiResponse<StudentScoreTrendResult> response = JsonSerializer.Deserialize<ApiResponse<StudentScoreTrendResult>>(jsonResult, JsonUtil.GetOptions());
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

        public async Task<Dictionary<long,double>> GetStudent312ScoreTrend(string studentNumber,int grade)
        {
            string url = HttpUtil.GetUrl($"/student/trend/student/312/score-trend?student_number={studentNumber}&grade={grade}");
            string jsonResult = await HttpUtil.GetAsync(url);
            ApiResponse<Dictionary<long, double>> response = JsonSerializer.Deserialize<ApiResponse<Dictionary<long, double>>>(jsonResult, JsonUtil.GetOptions());
            if (response.Code == 200)
            {
                return response.Data;
            }
            else
            {
                MessageBox.Show(response.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new Dictionary<long, double>();
            };
        }
    }
}
