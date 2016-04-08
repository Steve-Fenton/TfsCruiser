namespace TfsCruiser.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Chart.Mvc.ComplexChart;
    using Fenton.TeamServices;
    using Fenton.TeamServices.BuildRestApi;

    public class ForensicsController : Controller
    {
        private readonly VersionControlApi _versionControlApi;

        public ForensicsController()
        {
            _versionControlApi = new VersionControlApi(new ApiConfig());
        }

        public ActionResult Index(ForensicsViewModel model)
        {
            var churn = _versionControlApi.GetChurn(model);

            return View(churn);
        }

        public ActionResult History(ForensicsViewModel model)
        {
            var history = _versionControlApi.GetHistory(model);

            IDictionary<string, int> topList = GetTopList(history);

            var colorList = new List<string>
            {
                // Purple
                "#54278f",
                "#6a51a3",
                "#807dba",
                "#9e9ac8",
                "#bcbddc",

                // Red
                "#fc9272",
                "#fb6a4a",
                "#ef3b2c",
                "#cb181d",
                "#a50f15",
            };

            var colourIndex = 0;

            var paths = topList.OrderByDescending(l => l.Value).Take(10).OrderBy(l => l.Value).Select(l => l.Key);

            var lineChart = new LineChart();
            lineChart.ChartConfiguration.BezierCurve = false;
            lineChart.ChartConfiguration.ScaleShowHorizontalLines = true;
            lineChart.ChartConfiguration.ScaleShowVerticalLines = true;
            lineChart.ChartConfiguration.DatasetFill = false;
            lineChart.ChartConfiguration.MultiTooltipTemplate = "<%= datasetLabel %> - <%= value %>";

            var labels = history.Select(h => h.StartOfWeek).Distinct().OrderBy(d => d).ToList();

            lineChart.ComplexData.Labels.AddRange(labels.Select(d => d.ToString("yyyy-MM-dd")));

            foreach (var path in paths)
            {
                var values = new List<double>();
                var pathHistory = history.Where(h => h.ItemName == path);

                foreach (var label in labels)
                {
                    values.Add(pathHistory.SingleOrDefault(d => d.StartOfWeek == label)?.ChangeCount ?? 0);
                }

                lineChart.ComplexData.Datasets.Add(new ComplexDataset
                {
                    Data = values,
                    Label = path.Substring(path.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase) + 1),
                    StrokeColor = colorList[colourIndex],
                    PointColor = colorList[colourIndex]
                });

                colourIndex++;
            }

            return View(lineChart);
        }

        public ActionResult FileChurn(ForensicsViewModel model)
        {
            var churn = _versionControlApi.GetChurn(model);

            return View(churn);
        }

        private static IDictionary<string, int> GetTopList(IList<ChurnHistory> history)
        {
            IDictionary<string, int> topList = new Dictionary<string, int>();

            foreach (var item in history)
            {
                if (topList.ContainsKey(item.ItemName))
                {
                    topList[item.ItemName] += item.ChangeCount;
                }
                else
                {
                    topList.Add(item.ItemName, item.ChangeCount);
                }
            }

            return topList;
        }
    }
}