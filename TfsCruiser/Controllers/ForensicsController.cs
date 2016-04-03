namespace TfsCruiser.Controllers
{
    using System;
    using System.Configuration;
    using System.Web.Mvc;
    using Fenton.TeamServices;
    using Fenton.TeamServices.BuildRestApi;

    public class ForensicsController : Controller
    {
        private readonly VersionControlApi _versionControlApi;

        public ForensicsController()
        {
            _versionControlApi = new VersionControlApi(new ApiConfig());
        }

        public ActionResult Index(DateTime? from, DateTime? to)
        {
            ViewBag.Theme = ConfigurationManager.AppSettings["Theme"] ?? "default";

            return View(GetChangesets(from, to));
        }

        public ActionResult Update(DateTime? from, DateTime? to)
        {
            return View(GetChangesets(from, to));
        }

        private System.Collections.Generic.IList<GroupedBuild> GetChangesets(DateTime? from, DateTime? to)
        {
            if (!from.HasValue)
            {
                from = DateTime.Today.AddMonths(-3);
            }

            if (!to.HasValue)
            {
                to = DateTime.UtcNow;
            }

            var model = _versionControlApi.List(from.Value, to.Value);

            return model;
        }
    }
}