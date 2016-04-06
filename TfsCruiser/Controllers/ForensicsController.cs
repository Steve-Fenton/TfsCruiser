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

        [Route("Forensics/FolderChurn")]
        public ActionResult Index(ForensicsViewModel model)
        {
            ViewBag.Theme = ConfigurationManager.AppSettings["Theme"] ?? "default";

            var churn = _versionControlApi.GetChurn(model);

            return View(churn);
        }

        public ActionResult FileChurn(ForensicsViewModel model)
        {
            ViewBag.Theme = ConfigurationManager.AppSettings["Theme"] ?? "default";

            var churn = _versionControlApi.GetChurn(model);

            return View(churn);
        }
    }
}