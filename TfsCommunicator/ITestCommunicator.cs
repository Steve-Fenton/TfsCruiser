using Microsoft.TeamFoundation.Server;
using System.Collections.Generic;
using TfsCommunicator.Models;

namespace TfsCommunicator
{
    public interface ITestCommunicator
    {
        TestResult GetLatestBuildTestResult(string projectName, string buildDefintionName, string serverName);
        TestResult GetPreviousBuildTestResult(string projectName, string buildDefintionName, string serverName);
        TestResult GetLatestBuildTestResultByTitle(string buildTitle, string serverName);
        List<TestResult> GetLatestTestPlanStatusReport(string projectName);
        List<ProjectInfo> GetProjects();
    }
}
