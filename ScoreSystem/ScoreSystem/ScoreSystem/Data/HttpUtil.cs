using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Data
{
    public class HttpUtil
    {
        private static HttpClient client = new HttpClient();
        private static readonly string BASE_URL = "http://127.0.0.1:12442/score";

        public static HttpClient GetHttp()
        {
            return client;
        }

        public static void SetToken(string token)
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public static string GetUrl(string url)
        {
            return BASE_URL + url;
        }

        public static async Task<string> PostAsync(string url,string jsonBody)
        {
            //try
            //{
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                return await response.Content.ReadAsStringAsync();
            
            //}
            //catch(Exception e)
            //{
            //    Console.WriteLine(e.Message);
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
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    return null;
            //}
        }
    }
}
