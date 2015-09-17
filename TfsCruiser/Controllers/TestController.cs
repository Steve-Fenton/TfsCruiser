using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TfsCommunicator;
using TfsCommunicator.Models;
using TfsCruiser.Models;
using TfsCruiser.ViewModels;

namespace TfsCruiser.Controllers
{
    public class TestController : Controller
    {
        private readonly ITestCommunicator testCommunicator;

        public TestController()
        {
            string tfsServerAddress = ConfigurationManager.AppSettings["TfsServerAddress"];
            string domain = ConfigurationManager.AppSettings["TfsDomain"];
            string username = ConfigurationManager.AppSettings["TfsUsername"];
            string password = ConfigurationManager.AppSettings["TfsPassword"];

            NetworkCredential netCred = new NetworkCredential(
               username,
               password);

            this.testCommunicator = new TestCommunicator(tfsServerAddress, netCred);
        }

        public TestController(ITestCommunicator testCommunicator)
        {
            this.testCommunicator = testCommunicator;
        }

        public ActionResult TestPlanIndex()
        {
            var viewModel = new TestPlanViewModel();
            // Why can't i get this instantiated on contructor with get/set?
            viewModel.TestResults = new List<TestResult>();
            foreach (ProjectInfo project in this.testCommunicator.GetProjects())
            {               
                viewModel.TestResults.AddRange(testCommunicator.GetLatestTestPlanStatusReport(project.Name));
            };
            return View(viewModel);
        }

        public ActionResult Index()
        {
            var latestTestResult = testCommunicator.GetLatestBuildTestResult("PROJECT NAME", "BUILD_DEFINITION", "Build Server");
            var viewModel = new BuildTestsViewModel
            {
                UnitTestResult = latestTestResult,
                CurrentCoverage = latestTestResult.CodeCoverage,
                PreviousCoverage = testCommunicator.GetPreviousBuildTestResult("PROJECT NAME", "BUILD_DEFINITION", "Build Server").CodeCoverage,
                AutomationTestResult = new List<TestResult>
                {
                    testCommunicator.GetLatestBuildTestResultByTitle("BUILD DEFINITION TITLE 1", "Build Server 2"),
                    testCommunicator.GetLatestBuildTestResultByTitle("BUILD DEFINITION TITLE 2", "Build Server 3"),
                }
            };

            return View(viewModel);
        }
    }
}
