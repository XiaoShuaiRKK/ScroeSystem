using Org.BouncyCastle.Utilities;
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
    public class UniversitySevice
    {
        private static UniversitySevice intance;
        private UniversitySevice() { }

        public static UniversitySevice GetIntance()
        {
            if (intance == null)
            {
                intance = new UniversitySevice();
            }
            return intance;
        }


        public async Task<bool> AddUniversities(List<University> universities)
        {
            string url = HttpUtil.GetUrl("/university/batchAdd");
            // 序列化为 JSON
            string jsonBody = JsonSerializer.Serialize(universities, JsonUtil.GetRequestOptions());
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

        public async Task<List<University>> GetUniversities()
        {
            string url = HttpUtil.GetUrl($"/university/getAll");
            string jsonResult = await HttpUtil.GetAsync(url);
            var response = JsonSerializer.Deserialize<ApiResponse<List<University>>>(jsonResult, JsonUtil.GetOptions());
            if (response.Code == 200)
            {
                return response.Data;
            }
            else
            {
                MessageBox.Show(response.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<University>();
            }
        }

        public async Task<List<GradeThresholdPredictionResult>> PredictGradeThresholdProbability(int grade)
        {
            string url = HttpUtil.GetUrl($"/university/threshold/predict/grade");
            string jsonResult = await HttpUtil.GetAsync(url);
            var response = JsonSerializer.Deserialize<ApiResponse<List<GradeThresholdPredictionResult>>>(jsonResult, JsonUtil.GetOptions());
            if (response.Code == 200)
            {
                return response.Data;
            }
            else
            {
                MessageBox.Show(response.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<GradeThresholdPredictionResult>();
            }
        }
    }
}
