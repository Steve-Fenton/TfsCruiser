namespace Fenton.Rest.TeamServices
{
    public class TeamServicesApiConfig : IApiConfig
    {
        private readonly string _account = Config.GetType("TeamAccount", string.Empty);
        private readonly string _project = Config.GetType("TeamProject", string.Empty);
        private readonly string _username = Config.GetType("TeamUsername", string.Empty);
        private readonly string _password = Config.GetType("TeamPassword", string.Empty);

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