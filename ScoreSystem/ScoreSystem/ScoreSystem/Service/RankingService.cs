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

        public async Task<List<StudentRanking>> GetClassRanking3Course(int examId,int classId)
        {
            string url = HttpUtil.GetUrl($"/score/rankings/3-course/class?class_id={classId}&exam_id={examId}");
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

        public async Task<List<StudentRanking>> GetGradeRanking3Course(int examId, int grade)
        {
            string url = HttpUtil.GetUrl($"/score/rankings/3-course/grade?exam_id={examId}&grade={grade}");
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

        public async Task<List<StudentRanking>> GetClassRanking31Course(int examId, int classId)
        {
            string url = HttpUtil.GetUrl($"/score/rankings/group/one/class?class_id={classId}&exam_id={examId}");
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

        public async Task<List<StudentRanking>> GetGradeRanking31Course(int examId, int grade, int subjectGroupId)
        {
            string url = HttpUtil.GetUrl($"/score/rankings/group/one/grade?grade={grade}&exam_id={examId}&subject_group_id={subjectGroupId}");
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

        public async Task<List<StudentRanking>> GetClassRanking312Course(int examId, int classId)
        {
            string url = HttpUtil.GetUrl($"/score/rankings/group/312/class?class_id={classId}&exam_id={examId}");
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

        public async Task<List<StudentRanking>> GetGradeRanking312Course(int examId, int grade, int subjectGroupId)
        {
            string url = HttpUtil.GetUrl($"/score/rankings/group/312/grade?grade={grade}&exam_id={examId}&subject_group_id={subjectGroupId}");
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

        public async Task<List<StudentRanking>> GetClassRankingCourse(int examId, int classId)
        {
            string url = HttpUtil.GetUrl($"/score/rankings/course/class?class_id={classId}&exam_id={examId}");
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

        public async Task<List<StudentRanking>> GetGradeRankingCourse(int examId, int grade)
        {
            string url = HttpUtil.GetUrl($"/score/rankings/course/grade?grade={grade}&exam_id={examId}");
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

        public async Task<List<StudentRanking>> GetAllRangkingByClass(int examId,int classId)
        {
            string url = HttpUtil.GetUrl($"/score/rankings/class/all?class_id={classId}&exam_id={examId}");
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
                case RankModeEnum.三科总分:
                    return await GetClassRanking3Course(examId, classId);
                case RankModeEnum.三加一总分:
                    return await GetClassRanking31Course(examId, classId);
                case RankModeEnum.三加一加二总分:
                    return await GetClassRanking312Course(examId, classId);
                case RankModeEnum.各科:
                    return await GetClassRankingCourse(examId, classId);
                default:
                    return new List<StudentRanking>();
            }
        }

        public async Task<List<StudentRanking>> GetGradeRanking(int examId,int subjectGroupId,int grade,RankModeEnum mode)
        {
            switch (mode)
            {
                case RankModeEnum.总分:
                    return await GetGradeRankingByTotal(examId,grade);
                case RankModeEnum.三科总分:
                    return await GetGradeRanking3Course(examId, grade);
                case RankModeEnum.三加一总分:
                    return await GetGradeRanking31Course(examId, grade,subjectGroupId);
                case RankModeEnum.三加一加二总分:
                    return await GetGradeRanking312Course(examId, grade,subjectGroupId);
                case RankModeEnum.各科:
                    return await GetGradeRankingCourse(examId, grade);
                default:
                    return new List<StudentRanking>();
            }
        }
    }
}
