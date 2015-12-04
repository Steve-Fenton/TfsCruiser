using Fenton.TeamServices.BuildRestApi;
using Fenton.TeamServices;
using System.Web.Mvc;

namespace TfsCruiser.Controllers
{
    public class BuildsController : Controller
    {
        private readonly BuildApi buildApi;

        public BuildsController()
        {
            buildApi = new BuildApi(new ApiConfig());
        }

        public ActionResult Index()
        {
            return View(buildApi.List());
        }

        public ActionResult Update()
        {
            return View(buildApi.List());
        }
    }
}