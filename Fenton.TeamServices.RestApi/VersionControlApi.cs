namespace Fenton.TeamServices.BuildRestApi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class VersionControlApi
    {
        private readonly IApiConfig _config;
        private string _version = "1.0";

        public VersionControlApi(IApiConfig config)
        {
            _config = config;
        }

        public IList<GroupedBuild> List(DateTime from, DateTime to)
        {
            // https://www.visualstudio.com/integrate/api/tfvc/changesets
            var url = $"https://{_config.Account}.visualstudio.com/defaultcollection/{_config.Project}/tfvc/changesets?fromDate={from.ToString("s")}&toDate={to.ToString("s")}&api-version={_version}";
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