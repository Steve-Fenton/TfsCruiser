using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;

namespace TfsCommunicator.Models
{
    public class TestResult
    {
        public string ServerName { get; set; }
        public DateTime CompletedTime { get; set; }
        public int PassedTestCount { get; set; }
        public int FailedTestCount { get; set; }
        public int NotRunTestCount { get; set; }
        public int TotalTestCount { get; set; }
        public CodeCoverageResult CodeCoverage { get; set; }

        public static TestResult Map(ITestRun testRun, double? coverage, string serverName)
        {
            return new TestResult()
            {
                ServerName = serverName,
                CompletedTime = testRun.DateCompleted,
                PassedTestCount = testRun.Statistics.PassedTests,
                FailedTestCount = testRun.Statistics.FailedTests,
                NotRunTestCount = testRun.Statistics.InconclusiveTests,
                TotalTestCount = testRun.Statistics.TotalTests,
                CodeCoverage = coverage != null ? new CodeCoverageResult()
                    {
                        CompletedTime = testRun.DateCompleted,
                        Coverage =  coverage.Value
                    } : null
            };
        }
    }



}