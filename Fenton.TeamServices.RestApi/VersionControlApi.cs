namespace Fenton.TeamServices.BuildRestApi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    public class VersionControlApi
    {
        private readonly IApiConfig _config;
        private readonly int _pageSize;
        private string _version = "1.0";

        public VersionControlApi(IApiConfig config)
        {
            _config = config;
            _pageSize = 50;
        }

        public Changesets List(DateTime from, DateTime to, int page = 0)
        {
            // https://www.visualstudio.com/integrate/api/tfvc/changesets
            var url = $"https://{_config.Account}.visualstudio.com/defaultcollection/{_config.Project}/_apis/tfvc/changesets?$orderby=id desc&$top={_pageSize}&$skip={_pageSize * page}&maxCommentLength=20&fromDate={from.ToString("dd-MMM-yy HH:mm:ss")}&toDate={to.ToString("dd-MMM-yy HH:mm:ss")}&api-version={_version}";
            var result = RestApiClient.Get(url, _config.Username, _config.Password).Result;
            return JsonConvert.DeserializeObject<Changesets>(result);
        }

        public Changes Details(int changesetId)
        {
            var url = $"https://{_config.Account}.visualstudio.com/defaultcollection/_apis/tfvc/changesets/{changesetId}/changes?api-version={_version}";
            var result = RestApiClient.Get(url, _config.Username, _config.Password).Result;
            return JsonConvert.DeserializeObject<Changes>(result);
        }

        public IList<Churn> GetChurn(DateTime from, DateTime to)
        {
            var churn = new Dictionary<string, int>();

            bool morePages;
            int page = 0;

            do
            {
                var results = List(from, to, page);
                morePages = (results.count == _pageSize);

                foreach (var item in results.value)
                {
                    var changesetId = item.changesetId;

                    var details = ChangeDetailsWithCaching(changesetId);

                    foreach (var detail in details.value)
                    {
                        if (churn.ContainsKey(detail.item.path))
                        {
                            churn[detail.item.path]++;
                        }
                        else
                        {
                            churn.Add(detail.item.path, 1);
                        }
                    }
                }

                page++;
            }
            while (morePages);

            var churnList = churn
                .Select(c => new Churn { ItemName = c.Key, Count = c.Value })
                .OrderByDescending(c => c.Count)
                .Take(25)
                .ToList();

            var max = churnList.Max(c => c.Count);
            var group = Math.Ceiling(max / 5M);

            churnList.ForEach(c => c.Score = (int)Math.Ceiling(c.Count / group));

            return churnList;
        }

        private Changes ChangeDetailsWithCaching(int changesetId)
        {
            string dir = "c:\\Temp\\TFSCruiser";
            Directory.CreateDirectory(dir);

            string file = Path.Combine(dir, $"{_config.Project}-changeset-{changesetId}.cache");

            if (File.Exists(file))
            {
                var contents = File.ReadAllText(file);
                return JsonConvert.DeserializeObject<Changes>(contents);
            }

            var details = Details(changesetId);

            File.WriteAllText(file, JsonConvert.SerializeObject(details));

            return details;
        }
    }
}