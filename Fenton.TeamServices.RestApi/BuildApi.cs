using Fenton.TeamServices.RestApi;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Fenton.TeamServices.BuildRestApi
{
    public class BuildApi
    {
        private string account;
        private string project;
        private string user;
        private string password;
        private string version = "2.0";

        public BuildApi(string teamAccount, string teamProject, string teamUser, string teamPassword)
        {
            account = teamAccount;
            project = teamProject;
            user = teamUser;
            password = teamPassword;
        }

        public IList<GroupedBuild> List(string statusfilter = "completed")
        {
            // https://www.visualstudio.com/integrate/api/build/builds
            // Unused filters:
            // [&definitions={string}][&queues={string}][&buildnumber={string}][&type={string}][&minfinishtime={datetime}][&maxfinishtime={datetime}][&requestedfor={string}][&reasonfilter={string}][&tagfilters={string}][&propertyfilters={string}][&$top={int}][&continuationtoken={string}]

            var url = $"https://{account}.visualstudio.com/defaultcollection/{project}/_apis/build/builds?api-version={version}&statusfilter={statusfilter}";
            var result = RestApiClient.Get(url, user, password).Result;
            return MapToGroupedResult(result);
        }

        private static IList<GroupedBuild> MapToGroupedResult(string result)
        {
            var builds = JsonConvert.DeserializeObject<Builds>(result);
            var buildNames = builds.value.Select(b => b.definition.name).Distinct();

            var groupedBuilds = new List<GroupedBuild>();
            foreach (var name in buildNames)
            {
                groupedBuilds.Add(new GroupedBuild
                {
                    Name = name,
                    Builds = builds.value.Where(b => b.definition.name == name).OrderByDescending(b => b.finishTime).Take(10).ToList()
                });
            }

            return groupedBuilds;
        }
    }
}
