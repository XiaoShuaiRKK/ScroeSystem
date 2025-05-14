using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class LoginResult
    {
        private string token;
        private User user;

        public string Token { get => token; set => token = value; }
        public User User { get => user; set => user = value; }
    }
}
