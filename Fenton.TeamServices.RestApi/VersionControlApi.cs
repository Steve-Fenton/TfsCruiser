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
            // https://www.visualstudio.com/integrate/api/tfvc/changesets
            var url = $"https://{_config.Account}.visualstudio.com/defaultcollection/_apis/tfvc/changesets/{changesetId}/changes?api-version={_version}";
            var result = RestApiClient.Get(url, _config.Username, _config.Password).Result;
            return JsonConvert.DeserializeObject<Changes>(result);
        }

        public Churn GetChurn(int folderDepth, DateTime from, DateTime to)
        {
            Dictionary<string, int> changes = GetChangesFromApi(from, to);

            int totalChanges = changes.Sum(c => c.Value);

            var churn = new Churn();
            churn.Files = GetFileChurn(changes, totalChanges);
            churn.Folders = GetFolderChurn(folderDepth, churn.Files, totalChanges);

            return churn;
        }

        private Dictionary<string, int> GetChangesFromApi(DateTime from, DateTime to)
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

                    var details = GetChangeDetailsWithCaching(changesetId);

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

            return churn;
        }

        private List<FileChurn> GetFileChurn(Dictionary<string, int> changes, int totalChanges)
        {
            var files = changes
                .Select(c => new FileChurn { ItemName = c.Key, Count = c.Value })
                .OrderByDescending(c => c.Count)
                .ToList();

            var max = files.Max(c => c.Count);
            var group = Math.Ceiling(max / 5M);

            files.ForEach(c =>
            {
                c.Score = (int)Math.Ceiling(c.Count / group);
                c.Percentile = (int)Math.Round((c.Count / (decimal)totalChanges) * 100, 0);
            });

            return files;
        }

        private List<FolderChurn> GetFolderChurn(int folderDepth, IList<FileChurn> fileChurn, int totalChanges)
        {
            IDictionary<string, int> folderDictionary = new Dictionary<string, int>();
            foreach (var file in fileChurn)
            {
                var folder = GetFolder(folderDepth, file.ItemName);

                if (folderDictionary.ContainsKey(folder))
                {
                    folderDictionary[folder] += file.Count;
                }
                else
                {
                    folderDictionary.Add(folder, 1);
                }
            }

            var folders = folderDictionary
                .Select(f => new FolderChurn { ItemName = f.Key, Count = f.Value })
                .OrderByDescending(c => c.Count)
                .ToList();

            var max = folders.Max(c => c.Count);
            var group = Math.Ceiling(max / 5M);

            folders.ForEach(c =>
            {
                c.Score = (int)Math.Ceiling(c.Count / group);
                c.Percentile = (int)Math.Round((c.Count / (decimal)totalChanges) * 100, 0);
            });

            return folders;
        }

        private string GetFolder(int folderDepth, string path)
        {
            var parts = path.Split('/').Take(folderDepth);

            return string.Join(Path.DirectorySeparatorChar.ToString(), parts);
        }

        private Changes GetChangeDetailsWithCaching(int changesetId)
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