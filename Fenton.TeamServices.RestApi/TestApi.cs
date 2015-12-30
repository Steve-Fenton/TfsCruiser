using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fenton.TeamServices.TestRestApi
{
    public class TestApi
    {
        private IApiConfig _config;
        private string _version = "2.0-preview";

        public TestApi(IApiConfig config)
        {
            _config = config;
        }

        public IList<GroupedTest> List(string statusfilter = "completed")
        {
            // https://www.visualstudio.com/integrate/api/test/runs
            var url = $"https://{_config.Account}.visualstudio.com/defaultcollection/{_config.Project}/_apis/test/runs/query?api-version={_version}&includerundetails=true&$top=60";
            var query = "{ \"query\": \"Select * From TestRun Order By CompleteDate DESC \" }";
            var result = RestApiClient.Post(url, _config.Username, _config.Password, query).Result;
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
