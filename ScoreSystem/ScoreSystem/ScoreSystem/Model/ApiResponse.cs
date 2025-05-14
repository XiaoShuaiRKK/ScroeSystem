using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class ApiResponse<T>
    {
        private int code;
        private string message;
        private T data;
        private long timestamp;

        public int Code { get => code; set => code = value; }
        public string Message { get => message; set => message = value; }
        public T Data { get => data; set => data = value; }
        public long Timestamp { get => timestamp; set => timestamp = value; }
    }
}
