using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;

namespace TfsCommunicator.Models
{
    public class TestResult
    {

        public string Name { get; set; }
        public string ServerName { get; set; }
        public DateTime CompletedTime { get; set; }
        public int PassedTestCount { get; set; }
        public int FailedTestCount { get; set; }
        public int NotRunTestCount { get; set; }
        public int TotalTestCount { get; set; }
        public CodeCoverageResult CodeCoverage { get; set; }

        private const string SuccessfulClassName = "Succeeded";
        private const string FailedClassName = "Failed";
        private const string PartiallySucceeded = "PartiallySucceeded";
        private const string OnTrack = "OnTrack";

        public string GetTestResultCssClass()
        {

            if (FailedTestCount > 0) return FailedClassName; // Any fail it's bad       
            if (TotalTestCount < 5) return PartiallySucceeded; // Less than five tests? probably plan requires work, so regardless, partial            
            if ((PassedTestCount - NotRunTestCount) <= 0) return PartiallySucceeded; // Less that 50% passing, badish
            if (TotalTestCount == PassedTestCount) return SuccessfulClassName; // All pass it's good
            return OnTrack; // More than 50% passing, on track
        }

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