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
    public class RankingService
    {
        private static RankingService intance;
        private RankingService() { }
        public static RankingService GetIntance()
        {
            if(intance == null)
            {
                intance = new RankingService();
            }
            return intance;
        }

        public async Task<List<StudentRanking>> GetClassRankingByTotal(int examId,int classId)
        {
            string url = HttpUtil.GetUrl($"/score/rankings/student/total/byClass?class_id={classId}&exam_id={examId}");
            string jsonResult = await HttpUtil.GetAsync(url);
            var response = JsonSerializer.Deserialize<ApiResponse<List<StudentRanking>>>(jsonResult, JsonUtil.GetOptions());
            if (response.Code == 200)
            {
                return response.Data;
            }
            else
            {
                MessageBox.Show(response.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<StudentRanking>();
            }
        }

        public async Task<List<StudentRanking>> GetGradeRankingByTotal(int examId,int grade)
        {
            string url = HttpUtil.GetUrl($"/score/rankings/student/total/byGrade?exam_id={examId}&grade={grade}");
            string jsonResult = await HttpUtil.GetAsync(url);
            var response = JsonSerializer.Deserialize<ApiResponse<List<StudentRanking>>>(jsonResult, JsonUtil.GetOptions());
            if (response.Code == 200)
            {
                return response.Data;
            }
            else
            {
                MessageBox.Show(response.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<StudentRanking>();
            }
        }

        public async Task<List<StudentRanking>> GetClassRanking(int examId,int classId,RankModeEnum mode)
        {
            switch (mode)
            {
                case RankModeEnum.总分:
                    return await GetClassRankingByTotal(examId, classId);
                default:
                    return new List<StudentRanking>();
            }
        }
    }
}
