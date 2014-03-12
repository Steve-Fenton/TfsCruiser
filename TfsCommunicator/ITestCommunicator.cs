using TfsCommunicator.Models;

namespace TfsCommunicator
{
    public interface ITestCommunicator
    {
        TestResult GetLatestTestResult(string projectName, string buildDefintionName, string serverName);
        TestResult GetPreviousTestResult(string projectName, string buildDefintionName, string serverName);
        TestResult GetLatestTestResultByTitle(string buildTitle, string serverName);
    }
}
