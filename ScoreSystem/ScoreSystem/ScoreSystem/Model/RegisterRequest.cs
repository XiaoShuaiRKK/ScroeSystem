using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class RegisterRequest
    {
        private string username;
        private string password;
        private string name;
        private int level;
        private int role;

        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public string Name { get => name; set => name = value; }
        public int Level { get => level; set => level = value; }
        public int Role { get => role; set => role = value; }
    }
}
