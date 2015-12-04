using System.Configuration;

namespace Fenton.TeamServices
{
    public class ApiConfig : IApiConfig
    {
        private readonly string account = ConfigurationManager.AppSettings["TeamAccount"];
        private readonly string project = ConfigurationManager.AppSettings["TeamProject"];
        private readonly string username = ConfigurationManager.AppSettings["TeamUsername"];
        private readonly string password = ConfigurationManager.AppSettings["TeamPassword"];

        public string Account { get { return account; } }
        public string Project { get { return project; } }
        public string Username { get { return username; } }
        public string Password { get { return password; } }
    }
}
