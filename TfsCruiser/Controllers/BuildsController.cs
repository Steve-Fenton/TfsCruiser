using System.Configuration;
using System.Web.Mvc;
using Microsoft.TeamFoundation.Client;
using TfsCommunicator;
using TfsCruiser.Models;

namespace TfsCruiser.Controllers
{
    public class BuildsController : Controller
    {
        private readonly int maxDaysToDisplay;
        private readonly int maxRunsToDisplay;
        private readonly IBuildCommunicator buildCommunicator;

        public BuildsController()
        {
            string tfsServerAddress = ConfigurationManager.AppSettings["TfsServerAddress"];
            string domain = ConfigurationManager.AppSettings["TfsDomain"];
            string username = ConfigurationManager.AppSettings["TfsUsername"];
            string password = ConfigurationManager.AppSettings["TfsPassword"];
            if (!int.TryParse(ConfigurationManager.AppSettings["MaxDaysToDisplay"], out this.maxDaysToDisplay))
            {
                this.maxDaysToDisplay = 4;
            }
            if (!int.TryParse(ConfigurationManager.AppSettings["MaxRunsToDisplay"], out this.maxRunsToDisplay))
            {
                this.maxRunsToDisplay = 10;
            }

            ICredentialsProvider credentialsProvider = new CredentialsProvider(username, password, domain);
            this.buildCommunicator = new BuildCommunicator(tfsServerAddress, credentialsProvider);
        }

        public BuildsController(IBuildCommunicator buildCommunicator, int maxDaysToDisplay, int maxRunsToDisplay)
        {
            this.buildCommunicator = buildCommunicator;
            this.maxDaysToDisplay = maxDaysToDisplay;
            this.maxRunsToDisplay = maxRunsToDisplay;
        }

        public ActionResult Index(string project = "*", string build = "")
        {
            var model = GetBuildInformation(project, build);
            return View(model);
        }

        public ActionResult Update(string project = "*", string build = "")
        {
            var model = GetBuildInformation(project, build);
            return View(model);
        }

        private BuildStatusModel GetBuildInformation(string project, string build)
        {
            var buildInformation = buildCommunicator.GetBuildInformation(maxDaysToDisplay, maxRunsToDisplay, project, build);
            var model = BuildStatusModel.Map(buildInformation);
            return model;
        }
    }
}