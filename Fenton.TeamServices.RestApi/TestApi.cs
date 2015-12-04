using Fenton.TeamServices.RestApi;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Fenton.TeamServices.TestRestApi
{
    public class TestApi
    {
        private string account;
        private string project;
        private string user;
        private string password;
        private string version = "1.0";

        public TestApi(string teamAccount, string teamProject, string teamUser, string teamPassword)
        {
            account = teamAccount;
            project = teamProject;
            user = teamUser;
            password = teamPassword;
        }

        public IList<GroupedTest> List(string statusfilter = "completed")
        {
            // https://www.visualstudio.com/integrate/api/test/runs
            // Unused filters:
            // [&builduri={string}&owner={string}&planid={int}&automated={bool}&skip={int}&$top={int}

            var url = $"http://{account}.visualstudio.com/defaultcollection/{project}/_apis/test/runs?api-version={version}&statusfilter={statusfilter}&includerundetails=true";
            var result = RestApiClient.Get(url, user, password).Result;
            return MapToGroupedResult(result);
        }

        private static IList<GroupedTest> MapToGroupedResult(string result)
        {
            var builds = JsonConvert.DeserializeObject<TestRuns>(result);
            var buildNames = builds.value.Select(b => b.name).Distinct();

            var groupedBuilds = new List<GroupedTest>();
            foreach (var name in buildNames)
            {
                groupedBuilds.Add(new GroupedTest
                {
                    Name = name,
                    TestRuns = builds.value.Where(b => b.name == name).OrderByDescending(b => b.startedDate).Take(10).ToList()
                });
            }

            return groupedBuilds;
        }
    }
}
