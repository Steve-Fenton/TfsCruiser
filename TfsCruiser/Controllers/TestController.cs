namespace TfsCruiser.Controllers
{
    using System.Configuration;
    using System.Web.Mvc;
    using Fenton.TeamServices;
    using Fenton.TeamServices.TestRestApi;

    public class TestController : Controller
    {
        private readonly TestApi _testApi;

        public TestController()
        {
            _testApi = new TestApi(new ApiConfig());
        }

        public ActionResult Index()
        {
            ViewBag.Theme = ConfigurationManager.AppSettings["Theme"] ?? "default";

            return View(_testApi.List());
        }

        public ActionResult Update()
        {
            return View(_testApi.List());
        }
    }
}