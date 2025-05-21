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
    public class CriticalService
    {
        public async Task<List<CriticalConfig>> GetCriticalConfigs()
        {
            string url = HttpUtil.GetUrl($"/critical/config/get");
            string jsonResult = await HttpUtil.GetAsync(url);
            var response = JsonSerializer.Deserialize<ApiResponse<List<CriticalConfig>>>(jsonResult, JsonUtil.GetOptions());
            if (response.Code == 200)
            {
                return response.Data;
            }
            else
            {
                MessageBox.Show(response.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<CriticalConfig>();
            }
        }

        public async Task<bool> AddScore(List<CriticalConfig> criticalConfigs)
        {
            string url = HttpUtil.GetUrl("/critical/config/batchAdd");
            // 序列化为 JSON
            string jsonBody = JsonSerializer.Serialize(criticalConfigs, JsonUtil.GetRequestOptions());
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

        public async Task<List<CriticalStudentLog>> GetCriticalByGrade(int grade,int year)
        {
            string url = HttpUtil.GetUrl($"/critical/get?grade={grade}&year={year}");
            string jsonResult = await HttpUtil.GetAsync(url);
            var response = JsonSerializer.Deserialize<ApiResponse<List<CriticalStudentLog>>>(jsonResult, JsonUtil.GetOptions());
            if (response.Code == 200)
            {
                return response.Data;
            }
            else
            {
                MessageBox.Show(response.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<CriticalStudentLog>();
            }
        }
    }
}
