using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScoreSystem.Data
{
    public class HttpUtil
    {
        private static HttpClient client = new HttpClient();
        private static string BASE_URL = "http://127.0.0.1:12442/score"; // 默认值
        private static string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt");

        static HttpUtil()
        {
            LoadBaseUrl();
        }

        /// <summary>
        /// 从 config.txt 中读取 IP:PORT，然后构建 BASE_URL
        /// </summary>
        public static void LoadBaseUrl()
        {
            try
            {
                if (File.Exists(configPath))
                {
                    var addr = File.ReadAllText(configPath).Trim(); // 例: 192.168.1.100:8080
                    if (!string.IsNullOrWhiteSpace(addr))
                    {
                        BASE_URL = $"http://{addr}/score";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取服务器地址失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public static string GetUrl(string url)
        {
            return BASE_URL + url;
        }

        public static void SetToken(string token)
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public static async Task<string> PostAsync(string url, string jsonBody)
        {
            //try
            //{
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                return await response.Content.ReadAsStringAsync();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("网络错误: " + ex.Message, "请求失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return null;
            //}
        }

        public static async Task<string> GetAsync(string url)
        {
            //try
            //{
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                return null;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("网络错误: " + ex.Message, "请求失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return null;
            //}
        }

        public static void ClearToken()
        {
            client.DefaultRequestHeaders.Authorization = null;
        }
    }
}
