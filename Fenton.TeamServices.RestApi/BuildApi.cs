using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Fenton.TeamServices.BuildRestApi
{
    public class BuildApi
    {
        private readonly IApiConfig _config;
        private string _version = "2.0";

        public BuildApi(IApiConfig config)
        {
            _config = config;
        }

        public IList<GroupedBuild> List(string statusfilter = "completed")
        {
            // https://www.visualstudio.com/integrate/api/build/builds
            // Unused filters:
            // [&definitions={string}][&queues={string}][&buildnumber={string}][&type={string}][&minfinishtime={datetime}][&maxfinishtime={datetime}][&requestedfor={string}][&reasonfilter={string}][&tagfilters={string}][&propertyfilters={string}][&$top={int}][&continuationtoken={string}]
            var url = $"https://{_config.Account}.visualstudio.com/defaultcollection/{_config.Project}/_apis/build/builds?api-version={_version}&statusfilter={statusfilter}";
            var result = RestApiClient.Get(url, _config.Username, _config.Password).Result;
            return MapToGroupedResult(result);
        }

        private static IList<GroupedBuild> MapToGroupedResult(string result)
        {
            var items = JsonConvert.DeserializeObject<Builds>(result);
            var names = items.value.Select(b => b.definition.name).Distinct();

            var grouped = new List<GroupedBuild>();
            foreach (var name in names)
            {
                grouped.Add(new GroupedBuild
                {
                    Name = name,
                    Builds = items.value.Where(b => b.definition.name == name).OrderByDescending(b => b.finishTime).Take(10).ToList()
                });
            }

            return grouped;
        }
    }
}
