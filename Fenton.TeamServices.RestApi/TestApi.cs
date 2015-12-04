using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Fenton.TeamServices.TestRestApi
{
    public class TestApi
    {
        private IApiConfig _config;
        private string _version = "1.0";

        public TestApi(IApiConfig config)
        {
            _config = config;
        }

        public IList<GroupedTest> List(string statusfilter = "completed")
        {
            // https://www.visualstudio.com/integrate/api/test/runs
            // Unused filters:
            // [&builduri={string}&owner={string}&planid={int}&automated={bool}&skip={int}&$top={int}

            var url = $"http://{_config.Account}.visualstudio.com/defaultcollection/{_config.Project}/_apis/test/runs?api-version={_version}&statusfilter={statusfilter}&includerundetails=true";
            var result = RestApiClient.Get(url, _config.Username, _config.Password).Result;
            return MapToGroupedResult(result);
        }

        private static IList<GroupedTest> MapToGroupedResult(string result)
        {
            var items = JsonConvert.DeserializeObject<TestRuns>(result);
            var names = items.value.Select(b => b.name).Distinct();

            var grouped = new List<GroupedTest>();
            foreach (var name in names)
            {
                grouped.Add(new GroupedTest
                {
                    Name = name,
                    TestRuns = items.value.Where(b => b.name == name).OrderByDescending(b => b.startedDate).Take(10).ToList()
                });
            }

            return grouped;
        }
    }
}
