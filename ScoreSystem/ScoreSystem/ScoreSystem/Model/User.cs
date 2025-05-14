using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreSystem.Model
{
    public class User
    {
        private long id;
        private string name;
        private string username;
        private string level;
        private string role;

        public long Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Username { get => username; set => username = value; }
        public string Level { get => level; set => level = value; }
        public string Role { get => role; set => role = value; }
    }
}
