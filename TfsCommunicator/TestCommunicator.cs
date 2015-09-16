using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using TfsCommunicator.Models;

namespace TfsCommunicator
{
    public class TestCommunicator : ITestCommunicator
    {
        private string tfsServerAddress;
        private NetworkCredential credentialsProvider;
        private ITestManagementService testManagementService;

        public TestCommunicator(string tfsServerAddress, NetworkCredential credentialsProvider)
        {
            this.tfsServerAddress = tfsServerAddress;
            this.credentialsProvider = credentialsProvider;
        }

        public List<ProjectInfo> GetProjects()
        {
            var projectCollection = this.GetTeamServer().GetService<ICommonStructureService>();
            return projectCollection.ListProjects().ToList();
        }

        public TestResult GetLatestBuildTestResult(string projectName, string buildDefintionName, string serverName)
        {
            var buildDefinition = GetBuildDefinition(projectName, buildDefintionName);
            var buildDetail = GetLatestSuccessfulBuildDetail(buildDefinition);
            var buildTestResult = GetBuildTestResults(buildDetail, projectName);
            var codeCoverage = GetCodeCoverage(buildDetail, projectName);

            return TestResult.Map(buildTestResult, codeCoverage, serverName);
        }

        public TestResult GetPreviousBuildTestResult(string projectName, string buildDefintionName, string serverName)
        {
            var buildDefinition = GetBuildDefinition(projectName, buildDefintionName);
            var buildDetail = GetPreviousDaySuccessfulBuildDetail(buildDefinition);
            var buildTestResult = GetBuildTestResults(buildDetail, projectName);
            var codeCoverage = GetCodeCoverage(buildDetail, projectName);

            return TestResult.Map(buildTestResult, codeCoverage, serverName);
        }

        public TestResult GetLatestBuildTestResultByTitle(string buildTitle, string serverName)
        {
            var testResultServer = GetLatestBuildTestResultByTitle(buildTitle);
            return TestResult.Map(testResultServer, null, serverName);
        }

        private IBuildDefinition GetBuildDefinition(string projectName, string buildName)
        {
            // Get the latest build for this definition
            IBuildServer buildServer = (IBuildServer)GetTeamServer().GetService(typeof(IBuildServer));
            IBuildDefinition buildDef = buildServer.GetBuildDefinition(projectName, buildName);
            return buildDef;
        }

        private IBuildDetail GetLatestSuccessfulBuildDetail(IBuildDefinition buildDefinition)
        {
            IBuildDetail[] buildDetail = buildDefinition.QueryBuilds();

            var build = buildDetail.Where(b => b.Status == Microsoft.TeamFoundation.Build.Client.BuildStatus.Succeeded)
                                                .OrderByDescending(f => f.FinishTime)
                                                .First();
            return build;
        }

        private IBuildDetail GetPreviousDaySuccessfulBuildDetail(IBuildDefinition buildDefinition)
        {
            IBuildDetail[] buildDetail = buildDefinition.QueryBuilds();

            var previousBuild = buildDetail.Where(b => b.Status == Microsoft.TeamFoundation.Build.Client.BuildStatus.Succeeded)
                                                .Where(d => d.FinishTime.Date != DateTime.Now.Date)
                                                .OrderByDescending(f => f.FinishTime)
                                                .First();
            return previousBuild;
        }

        private ITestRun GetBuildTestResults(IBuildDetail buildDetail, string projectName)
        {
            var testManagementProject = GetTestManagementProject(GetTeamServer(), projectName);
            return testManagementProject.TestRuns.ByBuild(buildDetail.Uri).First();
        }

        private ITestRun GetLatestBuildTestResultByTitle(string buildTitle)
        {
            return testManagementService.QueryTestRuns("SELECT * FROM TestRun WHERE Title Contains '" + buildTitle.Trim() + "' AND State = 'Completed'")
                                            .OrderByDescending(t => t.DateCompleted)
                                            .First();
        }

        private double GetCodeCoverage(IBuildDetail build, string projectName)
        {
            var testManagementProject = GetTestManagementProject(GetTeamServer(), projectName);
            var codeCoverage = testManagementProject.CoverageAnalysisManager.QueryBuildCoverage(build.Uri.AbsoluteUri, CoverageQueryFlags.Modules);

            Double blocksCovered = codeCoverage[0].Modules.Sum(b => b.Statistics.BlocksCovered);
            Double blocksNotCovered = codeCoverage[0].Modules.Sum(c => c.Statistics.BlocksNotCovered);
            Double totalBlocks = blocksCovered + blocksNotCovered;

            var percentage = ((blocksCovered / totalBlocks) * 100);
            return percentage;
        }

        private TfsTeamProjectCollection GetTeamServer()
        {
            TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(tfsServerAddress), credentialsProvider);
            tfs.EnsureAuthenticated();
            return tfs;
        }

        private ITestManagementTeamProject GetTestManagementProject(TfsTeamProjectCollection tfs, string teamProject)
        {
            this.testManagementService = (ITestManagementService)tfs.GetService(typeof(ITestManagementService));
            ITestManagementTeamProject tmProject = testManagementService.GetTeamProject(teamProject);
            return tmProject;
        }

        public List<TestResult> GetLatestTestPlanStatusReport(string projectName)
        {
            var tfs = GetTeamServer();
            var teamProject = GetTestManagementProject(GetTeamServer(), projectName);
            var testPlans = teamProject.TestPlans.Query(string.Format("select * from TestPlan where StartDate<='{0}' and EndDate>='{0}'", DateTime.Now.Date.ToShortDateString()));
            var results = new List<TestResult>();
            foreach (var plan in testPlans)
            {
                results.Add(calculateTestResultByPlans(teamProject, plan));
            }
            return results;
        }

        private TestResult calculateTestResultByPlans(ITestManagementTeamProject project, ITestPlan testPlan)
        {
            var testResult = new TestResult();
            testResult.Name = string.Format("{0} - {1}", testPlan.Iteration, testPlan.Name);
            foreach (var testSuite in testPlan.RootSuite.SubSuites)
            {

                string queryForTestPointsForSpecificTestSuite = string.Format(CultureInfo.InvariantCulture, "SELECT * FROM TestPoint WHERE SuiteId = {0}", testSuite.Id);
                var testPoints = testPlan.QueryTestPoints(queryForTestPointsForSpecificTestSuite);

                foreach (var point in testPoints)
                {
                    // only get the last result for the current test point 
                    // otherwise we would mix test results with different users
                    var result = project
                        .TestResults
                        .ByTestId(point.TestCaseId)
                        .LastOrDefault(testResultToFind => testResultToFind.TestPointId == point.Id);
                    updateTestResultWithOutcome(testResult, result);
                }
            }
            return testResult;
        }

        private void updateTestResultWithOutcome(TestResult resultToUpdate, ITestResult resultToUpdateWith)
        {
            resultToUpdate.TotalTestCount++;
            // for some test points we might have no results yet
            if (resultToUpdateWith == null)
            {
                resultToUpdate.NotRunTestCount++;
                return;
            }
            switch (resultToUpdateWith.Outcome)
            {
                case TestOutcome.Passed:
                    resultToUpdate.PassedTestCount++;
                    break;
                case TestOutcome.Failed:
                    resultToUpdate.FailedTestCount++;
                    break;
                default:
                    resultToUpdate.NotRunTestCount++;
                    break;
            }
        }
    }
}
