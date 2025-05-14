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
    public class ClassService
    {
        public async Task<List<ClassEntity>> GetAllClasses()
        {
            string url = HttpUtil.GetUrl("/class/getAll");
            string jsonResult = await HttpUtil.GetAsync(url);
            ApiResponse<List<ClassEntity>> response = JsonSerializer.Deserialize<ApiResponse<List<ClassEntity>>>(jsonResult, JsonUtil.GetOptions());
            if(response.Code == 200)
            {
                return response.Data;
            }
            else
            {
                MessageBox.Show(response.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<ClassEntity>();
            }
        }
    }
}
