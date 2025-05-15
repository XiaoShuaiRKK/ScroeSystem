using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScoreSystem.Data
{
    public class JsonUtil
    {
        private static JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private static JsonSerializerOptions questOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static JsonSerializerOptions GetOptions()
        {
            return options;
        }

        public static JsonSerializerOptions GetRequestOptions()
        {
            return questOptions;
        }
    }
}
