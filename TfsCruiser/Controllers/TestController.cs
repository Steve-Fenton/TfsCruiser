using Fenton.TeamServices;
using Fenton.TeamServices.TestRestApi;
using System.Web.Mvc;

namespace TfsCruiser.Controllers
{
    public class TestController : Controller
    {
        private readonly TestApi _testApi;

        public TestController()
        {
            _testApi = new TestApi(new ApiConfig());
        }

        public ActionResult Index()
        {
            return View(_testApi.List());
        }

        public ActionResult Update()
        {
            return View(_testApi.List());
        }
    }
}
