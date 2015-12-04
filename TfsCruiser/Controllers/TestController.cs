using Fenton.TeamServices.TestRestApi;
using System.Configuration;
using System.Web.Mvc;


namespace TfsCruiser.Controllers
{
    public class TestController : Controller
    {
        private readonly TestApi testApi;

        public TestController()
        {
            string teamAccount = ConfigurationManager.AppSettings["TeamAccount"];
            string teamProject = ConfigurationManager.AppSettings["TeamProject"];
            string teamUsername = ConfigurationManager.AppSettings["TeamUsername"];
            string teamPassword = ConfigurationManager.AppSettings["TeamPassword"];

            testApi = new TestApi(teamAccount, teamProject, teamUsername, teamPassword);
        }

        public ActionResult Index()
        {
            var model = testApi.List();
            return View(model);
        }

        public ActionResult Update()
        {
            var model = testApi.List();
            return View(model);
        }
    }
}
