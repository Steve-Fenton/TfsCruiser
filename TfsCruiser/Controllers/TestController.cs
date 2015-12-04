using Fenton.TeamServices;
using Fenton.TeamServices.TestRestApi;
using System.Web.Mvc;

namespace TfsCruiser.Controllers
{
    public class TestController : Controller
    {
        private readonly TestApi testApi;

        public TestController()
        {
            testApi = new TestApi(new ApiConfig());
        }

        public ActionResult Index()
        {
            return View(testApi.List());
        }

        public ActionResult Update()
        {
            return View(testApi.List());
        }
    }
}
