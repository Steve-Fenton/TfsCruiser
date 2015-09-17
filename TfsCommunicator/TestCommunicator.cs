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
            var teamProject = GetTestManagementProject(GetTeamServer(), projectName);
            var testPlans = teamProject.TestPlans.Query(string.Format("select * from TestPlan where StartDate<='{0}' and EndDate>='{0}'", DateTime.Now.Date.ToShortDateString()));
            return testPlans.Select(plan => calculateTestResultByPlans(teamProject, plan)).ToList();
        }

        private TestResult calculateTestResultByPlans(ITestManagementTeamProject project, ITestPlan testPlan)
        {
            var testResult = new TestResult();
            testResult.Name = string.Format("{0} - {1}", testPlan.Iteration, testPlan.Name);
            var testSuites = new List<ITestSuiteBase>();
            if (testPlan.RootSuite != null) testSuites.AddRange(GetTestSuiteRecursive(testPlan.RootSuite));
                    
            foreach (var testSuite in testSuites)
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

        private List<ITestSuiteBase> GetTestSuiteRecursive(IStaticTestSuite staticTestSuite)
        {
            // 1. Store results in the IStaticTestSuit list.
            var result = new List<ITestSuiteBase>();

            // 2. Store a stack of our TestSuite.
            var stack = new Stack<ITestSuiteBase>();

            // 3. Add Root Test Suite
            stack.Push(staticTestSuite);

            // 4. Continue while there are TestSuites to Process
            while (stack.Count > 0)
            {
                // A. Get top Suite
                var dir = stack.Pop();

                try
                {
                    // B. Add all TestSuite at this directory to the result List.
                    result.Add(dir);

                    // only static suites can contain subsuites
                    var staticDir = dir as IStaticTestSuite;
                    if (staticDir != null)
                    {
                        // C. Add all SubSuite at this TestSuite.
                        foreach (ITestSuiteBase ss in staticDir.SubSuites)
                        {
                            stack.Push(ss);
                        }
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                    // D. Fails to open the test suite
                }
            }

            return result;
        }
    }
}
