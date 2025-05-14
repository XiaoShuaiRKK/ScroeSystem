using ScoreSystem.Data;
using ScoreSystem.Model;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace ScoreSystem.Service
{
    public class UserService
    {
        private static UserService intance;
        private UserService() { }
        public static UserService GetIntance()
        {
            if(intance == null)
            {
                intance = new UserService();
            }
            return intance;
        }

        private User user;

        public async Task<bool> Login(string username,string password)
        {
            string url = HttpUtil.GetUrl("/user/login");
            var loginRequest = new
            {
                username = username,
                password = password
            };
            string jsonBody = JsonSerializer.Serialize(loginRequest);
            string jsonResult = await HttpUtil.PostAsync(url,jsonBody);
            ApiResponse<LoginResult> loginResult = JsonSerializer.Deserialize<ApiResponse<LoginResult>>(jsonResult, JsonUtil.GetOptions());
            MessageBox.Show(loginResult.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if(loginResult.Code == 200)
            {
                var result = loginResult.Data;
                HttpUtil.SetToken(result.Token);
                user = result.User;
                return true;
            }
            return false;
        }

        public User GetUser()
        {
            return user;
        }
    }
}
