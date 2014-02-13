using System;
using System.Net;
using Microsoft.TeamFoundation.Client;

namespace TfsCommunicator
{
    public class CredentialsProvider : ICredentialsProvider
    {
        private NetworkCredential credentials;

        public CredentialsProvider(string user, string domain, string password)
        {
            credentials = new NetworkCredential(user, password, domain);
        }

        public ICredentials GetCredentials(Uri uri, ICredentials failedCredentials)
        {
            return credentials;
        }

        public void NotifyCredentialsAuthenticated(Uri uri)
        {
            throw new NotImplementedException();
        }
    }
}
