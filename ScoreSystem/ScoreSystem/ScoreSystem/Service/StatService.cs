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
    public class StatService
    {
        private static StatService intance;
        private StatService() { }
        public static StatService GetIntance()
        {
            if(intance == null)
            {
                intance = new StatService();
            }
            return intance;
        }

        public async Task<bool> GenerateStat(int examId)
        {
            string url = HttpUtil.GetUrl($"/exam/stat/generate?exam_id={examId}");
            // 序列化为 JSON
            string jsonResult = await HttpUtil.PostAsync(url, "");
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

        public async Task<List<ExamClassSubjectStat>> GetStats(int examId,int subjectGroupId)
        {
            string url = HttpUtil.GetUrl($"/exam/stat/get?exam_id={examId}&subject_group_id={subjectGroupId}");
            string jsonResult = await HttpUtil.GetAsync(url);
            var response = JsonSerializer.Deserialize<ApiResponse<List<ExamClassSubjectStat>>>(jsonResult, JsonUtil.GetOptions());
            if (response.Code == 200)
            {
                return response.Data;
            }
            else
            {
                MessageBox.Show(response.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<ExamClassSubjectStat>();
            }
        }
    }
}
