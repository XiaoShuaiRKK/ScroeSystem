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

        public static JsonSerializerOptions GetOptions()
        {
            return options;
        }
    }
}
