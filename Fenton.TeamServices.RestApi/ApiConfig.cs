using System.Configuration;

namespace Fenton.TeamServices
{
    public class ApiConfig : IApiConfig
    {
        private readonly string _account = ConfigurationManager.AppSettings["TeamAccount"];
        private readonly string _project = ConfigurationManager.AppSettings["TeamProject"];
        private readonly string _username = ConfigurationManager.AppSettings["TeamUsername"];
        private readonly string _password = ConfigurationManager.AppSettings["TeamPassword"];

        public string Account
        {
            get { return _account; }
        }

        public string Project
        {
            get { return _project; }
        }

        public string Username
        {
            get { return _username; }
        }

        public string Password
        {
            get { return _password; }
        }
    }
}
