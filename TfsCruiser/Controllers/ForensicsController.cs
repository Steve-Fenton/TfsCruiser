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
        private readonly int _defaultFolderDepth = 4;

        public ForensicsController()
        {
            _versionControlApi = new VersionControlApi(new ApiConfig());
        }

        public ActionResult Index(int? folderDepth, DateTime? from, DateTime? to)
        {
            ViewBag.Theme = ConfigurationManager.AppSettings["Theme"] ?? "default";

            return View(GetModel(folderDepth, from, to));
        }

        public ActionResult Update(int? folderDepth, DateTime? from, DateTime? to)
        {
            return View(GetModel(folderDepth, from, to));
        }

        public ActionResult FileChurn(int? folderDepth, DateTime? from, DateTime? to)
        {
            ViewBag.Theme = ConfigurationManager.AppSettings["Theme"] ?? "default";

            return View(GetModel(folderDepth, from, to));
        }

        public ActionResult FolderChurn(int? folderDepth, DateTime? from, DateTime? to)
        {
            ViewBag.Theme = ConfigurationManager.AppSettings["Theme"] ?? "default";

            return View(GetModel(folderDepth, from, to));
        }

        private Churn GetModel(int? folderDepth, DateTime? from, DateTime? to)
        {
            if (!folderDepth.HasValue)
            {
                folderDepth = _defaultFolderDepth;
            }

            if (!from.HasValue)
            {
                from = DateTime.Today.AddMonths(-3);
            }

            if (!to.HasValue)
            {
                to = DateTime.UtcNow;
            }

            var model = _versionControlApi.GetChurn(folderDepth.Value, from.Value, to.Value);

            return model;
        }
    }
}