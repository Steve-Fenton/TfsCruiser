namespace Fenton.TeamServices.BuildRestApi
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;

    public class VersionControlApi
    {
        private readonly IApiConfig _config;
        private readonly IList<string> _excludedFileTypes;
        private readonly int _pageSize;
        private readonly string _version = "1.0";

        public VersionControlApi(IApiConfig config)
        {
            _config = config;
            _pageSize = 50;

            var fileTypeConfig = ConfigurationManager.AppSettings["ChurnExcludedFileTypes"];

            if (!string.IsNullOrWhiteSpace(fileTypeConfig))
            {
                _excludedFileTypes = fileTypeConfig.Split(new char[] { ',' });
            }
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

        public Churn GetChurn(DateTime from, DateTime to)
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
                        if (_excludedFileTypes.Contains(Path.GetExtension(detail.item.path)))
                        {
                            continue;
                        }

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

            var ch = new Churn();

            ch.Files = churn
                .Select(c => new FileChurn { ItemName = c.Key, Count = c.Value })
                .OrderByDescending(c => c.Count)
                .ToList();

            var max = ch.Files.Max(c => c.Count);
            var group = Math.Ceiling(max / 5M);

            ch.Files.ForEach(c => c.Score = (int)Math.Ceiling(c.Count / group));

            IDictionary<string, int> folderDictionary = new Dictionary<string, int>();
            foreach (var file in ch.Files)
            {
                var folder = GetFolder(file.ItemName);

                if (folderDictionary.ContainsKey(folder))
                {
                    folderDictionary[folder] += file.Count;
                }
                else
                {
                    folderDictionary.Add(folder, 1);
                }
            }

            ch.Folders = folderDictionary
                .Select(f => new FolderChurn { ItemName = f.Key, Count = f.Value })
                .OrderByDescending(c => c.Count)
                .ToList();

            max = ch.Folders.Max(c => c.Count);
            group = Math.Ceiling(max / 5M);

            ch.Folders.ForEach(c => c.Score = (int)Math.Ceiling(c.Count / group));

            return ch;
        }

        private string GetFolder(string path)
        {
            const int depth = 4;

            var parts = path.Split('/').Take(depth);

            return string.Join(Path.DirectorySeparatorChar.ToString(), parts);
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