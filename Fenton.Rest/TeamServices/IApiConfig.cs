namespace Fenton.Rest.TeamServices
{
    public interface IApiConfig
    {
        string Account { get; }

        string Password { get; }

        string Project { get; }

        string Username { get; }
    }
}