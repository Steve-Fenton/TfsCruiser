using Fenton.TeamServices.BuildRestApi;
using Fenton.TeamServices;
using System.Web.Mvc;

namespace TfsCruiser.Controllers
{
    public class BuildsController : Controller
    {
        private readonly BuildApi _buildApi;

        public BuildsController()
        {
            _buildApi = new BuildApi(new ApiConfig());
        }

        public ActionResult Index()
        {
            return View(_buildApi.List());
        }

        public ActionResult Update()
        {
            return View(_buildApi.List());
        }
    }
}