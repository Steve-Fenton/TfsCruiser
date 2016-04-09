namespace Fenton.Forensics.TeamServices
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;
    using Fenton.Rest;
    using Fenton.Rest.TeamServices;

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

            var fileTypeConfig = Config.GetType("ChurnExcludedFileTypes", string.Empty);

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

        public IList<ChurnHistory> GetHistory(ForensicsViewModel model)
        {
            var filters = PopulateHistoryDefaults(model);

            if (filters.To <= filters.From)
            {
                throw new ArgumentException("From date should be earlier than To date.");
            }

            IList<ChurnHistory> history = new List<ChurnHistory>();

            var date = filters.From;

            do
            {
                var weekModel = new ForensicsViewModel
                {
                    From = date,
                    To = date.AddDays(7),
                    FolderDepth = filters.FolderDepth,
                    Path = filters.Path,
                    SelectedPath = filters.SelectedPath
                };

                var churn = GetChurn(weekModel);

                foreach (var file in churn.Churn.Files)
                {
                    history.Add(new ChurnHistory
                    {
                        ItemName = file.ItemName,
                        ChangeCount = file.Count,
                        StartOfWeek = date
                    });
                }

                date = weekModel.To;
            }
            while (date < filters.To);

            return history;
        }

        private ForensicsViewModel PopulateHistoryDefaults(ForensicsViewModel model)
        {
            return new ForensicsViewModel
            {
                FolderDepth = model.FolderDepth == default(int) ? 1 : model.FolderDepth,
                From = model.From == default(DateTime) ? StartOfWeek(DateTime.Today.AddMonths(-3), DayOfWeek.Monday) : StartOfWeek(model.From, DayOfWeek.Monday),
                To = model.To == default(DateTime) ? StartOfWeek(DateTime.Today, DayOfWeek.Monday) : StartOfWeek(model.To, DayOfWeek.Monday),
                Path = string.IsNullOrWhiteSpace(model.Path) ? null : model.Path,
                SelectedPath = string.IsNullOrWhiteSpace(model.SelectedPath) ? null : model.SelectedPath,
            };
        }

        private DateTime StartOfWeek(DateTime dateTime, DayOfWeek startOfWeek)
        {
            int diff = dateTime.DayOfWeek - startOfWeek;

            if (diff < 0)
            {
                diff += 7;
            }

            return dateTime.AddDays(-1 * diff).Date;
        }

        public ForensicsViewModel GetChurn(ForensicsViewModel model)
        {
            var filters = PopulateChurnDefaults(model);

            Dictionary<string, CountWithVersion> changes = null;

            string dir = "c:\\Temp\\TFSCruiser\\queries\\";
            Directory.CreateDirectory(dir);

            string file = Path.Combine(dir, $"{_config.Project}-changes-{filters.From.ToString("yyyyMMdd")}-{filters.To.ToString("yyyyMMdd")}.cache");

            if (File.Exists(file))
            {
                var contents = File.ReadAllText(file);
                changes = JsonConvert.DeserializeObject<Dictionary<string, CountWithVersion>>(contents);
            }
            else
            {
                changes = GetChangesFromApi(filters.From, filters.To);
                File.WriteAllText(file, JsonConvert.SerializeObject(changes));
            }

            int totalChanges = changes.Sum(c => c.Value.Count);

            filters.Churn = new Churn();

            if (!string.IsNullOrEmpty(filters.SelectedPath))
            {
                filters.Path = filters.SelectedPath;
                filters.SelectedPath = null;
                filters.FolderDepth++;

                filters.Churn.Files = GetFileChurn(changes, totalChanges, filters.Path);
                filters.Churn.Folders = GetFolderChurn(filters.FolderDepth, filters.Churn.Files, totalChanges, filters.Path);
            }
            else
            {
                filters.Churn.Files = GetFileChurn(changes, totalChanges);
                filters.Churn.Folders = GetFolderChurn(filters.FolderDepth, filters.Churn.Files, totalChanges);
            }

            return filters;
        }

        private ForensicsViewModel PopulateChurnDefaults(ForensicsViewModel model)
        {
            return new ForensicsViewModel
            {
                FolderDepth = model.FolderDepth == default(int) ? 1 : model.FolderDepth,
                From = model.From == default(DateTime) ? DateTime.Today.AddMonths(-3) : model.From,
                To = model.To == default(DateTime) ? DateTime.Today : model.To,
                Path = string.IsNullOrWhiteSpace(model.Path) ? null : model.Path,
                SelectedPath = string.IsNullOrWhiteSpace(model.SelectedPath) ? null : model.SelectedPath,
            };
        }

        private Dictionary<string, CountWithVersion> GetChangesFromApi(DateTime from, DateTime to)
        {
            var churn = new Dictionary<string, CountWithVersion>();

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
                        var ext = Path.GetExtension(detail.item.path);
                        if (_excludedFileTypes.Contains(ext))
                        {
                            continue;
                        }

                        if (churn.ContainsKey(detail.item.path))
                        {
                            churn[detail.item.path].Count++;
                            churn[detail.item.path].Version = Math.Max(detail.item.version, churn[detail.item.path].Version);
                        }
                        else
                        {
                            churn.Add(detail.item.path, new CountWithVersion { Count = 1, Version = detail.item.version });
                        }
                    }
                }

                page++;
            }
            while (morePages);

            return churn;
        }

        private List<FileChurn> GetFileChurn(Dictionary<string, CountWithVersion> changes, int totalChanges, string path = "")
        {
            var filePath = path.Replace('\\', '/');

            var files = changes
                .Where(c => c.Key.StartsWith(filePath, StringComparison.InvariantCultureIgnoreCase))
                .Select(c => new FileChurn { ItemName = c.Key, Count = c.Value.Count, Version = c.Value.Version })
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

        private List<FolderChurn> GetFolderChurn(int folderDepth, IList<FileChurn> fileChurn, int totalChanges, string path = "")
        {
            IDictionary<string, int> folderDictionary = new Dictionary<string, int>();
            foreach (var file in fileChurn)
            {
                var folder = GetFolder(folderDepth, file.ItemName);

                if (!folder.StartsWith(path, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

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
            string changeCache = "c:\\Temp\\TFSCruiser\\changesets\\";
            Directory.CreateDirectory(changeCache);

            string file = Path.Combine(changeCache, $"{_config.Project}-{changesetId}.cache");

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