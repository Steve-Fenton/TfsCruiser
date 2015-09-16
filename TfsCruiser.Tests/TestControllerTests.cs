using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Linq;
using System.Web.Mvc;
using TfsCommunicator;
using TfsCommunicator.Models;
using TfsCruiser.Controllers;
using TfsCruiser.ViewModels;

namespace TfsCruiser.Tests
{
    [TestClass]
    public class TestControllerTests
    {
        private ITestCommunicator testCommunicator;
        private TestController target;

        public TestControllerTests()
        {
            this.testCommunicator = MockRepository.GenerateStub<ITestCommunicator>();
            this.target = new TestController(testCommunicator);
        }

        [TestMethod]
        public void InstantiateExpectNoException()
        {
            var testController = new TestController();
        }

        [TestMethod]
        public void IndexWithValidDataExpectCorrectResult()
        {
            var previous = GetTestResultWithCoverage();
            previous.CompletedTime = DateTime.Parse("30/12/2000");
            previous.CodeCoverage.Coverage = 70.01;

            testCommunicator.Stub(t => t.GetLatestBuildTestResult(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                            .Return(GetTestResultWithCoverage());
            testCommunicator.Stub(t => t.GetPreviousBuildTestResult(Arg<string>.Is.Anything, Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                            .Return(previous);
            testCommunicator.Stub(t => t.GetLatestBuildTestResultByTitle(Arg<string>.Is.Anything, Arg<string>.Is.Anything))
                            .Return(GetTestResultWithNullCoverage());
                                

            var result = target.Index() as ViewResult;
            Assert.IsNotNull(result);
            var parsedResult = result.Model as BuildTestsViewModel;
            Assert.IsNotNull(parsedResult);
            Assert.AreEqual(2, parsedResult.AutomationTestResult.Count());
            Assert.AreEqual(70.22, parsedResult.CurrentCoverage.Coverage);
            Assert.AreEqual(70.01, parsedResult.PreviousCoverage.Coverage);
            Assert.AreEqual(50, parsedResult.UnitTestResult.PassedTestCount);
        }



        [TestMethod]
        public void EnsureGoodTestReportSuccess()
        {
            Assert.AreEqual(GetRestResultForGoodTest().GetTestResultCssClass(), "Succeeded");
        }

        [TestMethod]
        public void EnsureBadTestReportFail()
        {
            Assert.AreEqual(GetRestResultForVeryBadTest().GetTestResultCssClass(), "Failed");
        }

        [TestMethod]
        public void EnsureMediocreTestReportPartial()
        {
            Assert.AreEqual(GetRestResultForBalancedTest().GetTestResultCssClass(), "PartiallySucceeded");
        }


        [TestMethod]
        public void EnsureBitBetterThanMediocreTestReportOnTrack()
        {
            var balancedTestPlan = GetRestResultForBalancedTest();
            Assert.AreEqual(balancedTestPlan.GetTestResultCssClass(), "PartiallySucceeded");
            balancedTestPlan.TotalTestCount++;
            balancedTestPlan.PassedTestCount++;
            Assert.AreEqual(balancedTestPlan.GetTestResultCssClass(), "OnTrack");
        }


        [TestMethod]
        public void EnsureRubbishTestReportPartial()
        {
            Assert.AreEqual(GetRestResultForRubbishTest().GetTestResultCssClass(), "PartiallySucceeded");
        }


        // Mocks

        private TestResult GetTestResultWithNullCoverage()
        {
            return new TestResult
            {
                CompletedTime = DateTime.Parse("01/01/2001"),
                FailedTestCount = 0,
                NotRunTestCount = 0,
                PassedTestCount = 50,
                ServerName = "Server",
                TotalTestCount = 50
            };
        }

        private TestResult GetTestResultWithCoverage()
        {
            return new TestResult()
            {
                CodeCoverage = new CodeCoverageResult
                {
                     CompletedTime = DateTime.Parse("02/01/2001"),
                     Coverage = 70.22
                },
                CompletedTime = DateTime.Parse("01/01/2001"),
                FailedTestCount = 0,
                NotRunTestCount = 0,
                PassedTestCount = 50,
                ServerName = "Server",
                TotalTestCount = 50
            };
        }


        private TestResult GetRestResultForGoodTest()
        {
            return new TestResult()
            {              
                FailedTestCount = 0,
                NotRunTestCount = 0,
                PassedTestCount = 50,                
                TotalTestCount = 50
            };
        }

        private TestResult GetRestResultForVeryBadTest()
        {
            return new TestResult()
            {
                FailedTestCount = 50,
                NotRunTestCount = 0,
                PassedTestCount = 0,
                TotalTestCount = 50
            };
        }

        private TestResult GetRestResultForBalancedTest()
        {
            return new TestResult()
            {
                FailedTestCount = 0,
                NotRunTestCount = 25,
                PassedTestCount = 25,
                TotalTestCount = 50
            };
        }

        private TestResult GetRestResultForRubbishTest()
        {
            return new TestResult()
            {
                FailedTestCount = 0,
                NotRunTestCount = 1,
                PassedTestCount = 1,
                TotalTestCount = 2
            };
        }
    }
}
