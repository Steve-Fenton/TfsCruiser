using System.Collections.Generic;
using TfsCommunicator.Models;
using System.Linq;

namespace TfsCruiser.ViewModels
{
    public class TestsViewModel
    {
        public TestResult UnitTestResult { get; set; }
        public IEnumerable<TestResult> AutomationTestResult { get; set; }
        public CodeCoverageResult CurrentCoverage { get; set; }
        public CodeCoverageResult PreviousCoverage { get; set; }

        private const string SuccessfulClassName = "Succeeded";
        private const string FailedClassName = "Failed";

        public string GetCodeCoverageIcon()
        {
            if (CurrentCoverage.Coverage < PreviousCoverage.Coverage)
            {
                return "Decreased";
            }
            else if (CurrentCoverage.Coverage == PreviousCoverage.Coverage)
            {
                return "NoChange";
            }
            return "Increased";
        }

        public string GetCodeCoverageClass()
        {
            if(CurrentCoverage.Coverage < PreviousCoverage.Coverage)
            {
                return FailedClassName;
            }
            return SuccessfulClassName;
        }

        public string GetUnitTestResultClass()
        {
            if(UnitTestResult.PassedTestCount == UnitTestResult.TotalTestCount)
            {
                return SuccessfulClassName;
            }
            return FailedClassName;
        }

        public string GetAutomationTestResultClass()
        {
            if(AutomationTestResult.Any(a => a.FailedTestCount > 0))
            {
                return FailedClassName;
            }
            return SuccessfulClassName;
        }
    }
}