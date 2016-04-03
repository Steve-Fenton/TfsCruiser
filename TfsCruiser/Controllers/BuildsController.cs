namespace TfsCruiser.Controllers
{
    using System.Configuration;
    using System.Web.Mvc;
    using Fenton.TeamServices;
    using Fenton.TeamServices.BuildRestApi;

    public class BuildsController : Controller
    {
        private readonly BuildApi _buildApi;

        public BuildsController()
        {
            _buildApi = new BuildApi(new ApiConfig());
        }

        public ActionResult Index()
        {
            ViewBag.Theme = ConfigurationManager.AppSettings["Theme"] ?? "default";

            return View(_buildApi.List());
        }

        public ActionResult Update()
        {
            return View(_buildApi.List());
        }
    }
}