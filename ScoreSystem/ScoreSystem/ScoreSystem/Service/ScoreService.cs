using MathNet.Numerics.Distributions;
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
    public class ScoreService
    {
        private static ScoreService intance;
        private List<Exam> exams;

        private ScoreService() { }
        public static ScoreService GetIntance()
        {
            if(intance == null)
            {
                intance = new ScoreService();
            }
            return intance;
        }

        public async Task<List<Exam>> GetExams()
        {
            string url = HttpUtil.GetUrl("/exam/list");
            string jsonResult = await HttpUtil.GetAsync(url);
            var response = JsonSerializer.Deserialize<ApiResponse<List<Exam>>>(jsonResult, JsonUtil.GetOptions());
            if(response.Code == 200)
            {
                exams = response.Data;
                return response.Data;
            }
            else
            {
                MessageBox.Show(response.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Exam>();
            }
        }

        public async Task<List<StudentScore>> GetScoresByClass(int examId,int classId)
        {
            string url = HttpUtil.GetUrl($"/teacher/getClass/studentScore?exam_id={examId}&class_id={classId}");
            string jsonResult = await HttpUtil.GetAsync(url);
            var response = JsonSerializer.Deserialize<ApiResponse<List<StudentScore>>>(jsonResult, JsonUtil.GetOptions());
            if (response.Code == 200)
            {
                return response.Data;
            }
            else
            {
                MessageBox.Show(response.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<StudentScore>();
            }
        }

        public int GetExamId(string examName)
        {
            if (exams == null || !exams.Any())
            {
                return -1;
            }
            var matchedExem = exams.FirstOrDefault(e => e.Name == examName);
            if (matchedExem != null)
            {
                return matchedExem.Id;
            }
            else
            {
                return -1;
            }
        }

        public async Task<bool> AddScore(List<ScoreEntity> scores)
        {
            string url = HttpUtil.GetUrl("/score/batchAdd");
            // 序列化为 JSON
            string jsonBody = JsonSerializer.Serialize(scores, JsonUtil.GetRequestOptions());
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
