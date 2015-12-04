using Fenton.TeamServices.BuildRestApi;
using System.Configuration;
using System.Web.Mvc;

namespace TfsCruiser.Controllers
{
    public class BuildsController : Controller
    {
        private readonly BuildApi buildApi;

        public BuildsController()
        {
            string teamAccount = ConfigurationManager.AppSettings["TeamAccount"];
            string teamProject = ConfigurationManager.AppSettings["TeamProject"];
            string teamUsername = ConfigurationManager.AppSettings["TeamUsername"];
            string teamPassword = ConfigurationManager.AppSettings["TeamPassword"];

            buildApi = new BuildApi(teamAccount, teamProject, teamUsername, teamPassword);

        }

        public ActionResult Index()
        {
            var model = buildApi.List();
            return View(model);
        }

        public ActionResult Update()
        {
            var model = buildApi.List();
            return View(model);
        }
    }
}