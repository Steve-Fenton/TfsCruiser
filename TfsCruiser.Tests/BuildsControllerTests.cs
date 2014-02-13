using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Web.Mvc;
using TfsCommunicator;
using TfsCruiser.Controllers;
using TfsCruiser.Models;

namespace TfsCruiser.Tests
{
    [TestClass]
    public class BuildsControllerTests
    {
        private IBuildCommunicator buildCommunicator;

        [TestInitialize]
        public void SetUp()
        {
            this.buildCommunicator = MockRepository.GenerateStub<IBuildCommunicator>();
        }

        [TestMethod]
        public void InstantiateExpectNoException()
        {
            var buildsController = new BuildsController();
        }

        [TestMethod]
        public void IndexExpectCorrectViewResult()
        {
            this.buildCommunicator
                .Stub(bc => bc.GetBuildInformation(2, 4, "*", string.Empty))
                .Return(new BuildStatus());

            var buildsController = new BuildsController(buildCommunicator, 2, 4);
            var result = buildsController.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
        }

        [TestMethod]
        public void IndexWithProjectNameExpectCorrectViewResult()
        {
            const string projectName = "TestProjectName";

            this.buildCommunicator
                .Stub(bc => bc.GetBuildInformation(3, 5, projectName, string.Empty))
                .Return(new BuildStatus());

            var buildsController = new BuildsController(buildCommunicator, 3, 5);
            var result = buildsController.Index(projectName) as ViewResult;

            Assert.IsNotNull(result);

            var model = result.Model as BuildStatusModel;
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void IndexWithProjectNameAndBuildDefinitionExpectCorrectViewResult()
        {
            const string projectName = "TestProjectName";
            const string buildDefinition = "CI Build";

            this.buildCommunicator
                .Stub(bc => bc.GetBuildInformation(3, 5, projectName, buildDefinition))
                .Return(new BuildStatus());

            var buildsController = new BuildsController(buildCommunicator, 3, 5);
            var result = buildsController.Index(projectName, buildDefinition) as ViewResult;

            Assert.IsNotNull(result);

            var model = result.Model as BuildStatusModel;
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void UpdateExpectCorrectViewResult()
        {
            this.buildCommunicator
                .Stub(bc => bc.GetBuildInformation(2, 4, "*", string.Empty))
                .Return(new BuildStatus());

            var buildsController = new BuildsController(buildCommunicator, 2, 4);
            var result = buildsController.Update() as ViewResult;

            Assert.IsNotNull(result);

            var model = result.Model as BuildStatusModel;
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void UpdateWithProjectNameExpectCorrectViewResult()
        {
            const string projectName = "TestProjectName";

            this.buildCommunicator
                .Stub(bc => bc.GetBuildInformation(3, 5, projectName, string.Empty))
                .Return(new BuildStatus());

            var buildsController = new BuildsController(buildCommunicator, 3, 5);
            var result = buildsController.Update(projectName) as ViewResult;

            Assert.IsNotNull(result);

            var model = result.Model as BuildStatusModel;
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void UpdateWithProjectNameAndBuildDefinitionExpectCorrectViewResult()
        {
            const string projectName = "TestProjectName";
            const string buildDefinition = "CI Build";

            this.buildCommunicator
                .Stub(bc => bc.GetBuildInformation(3, 5, projectName, buildDefinition))
                .Return(new BuildStatus());

            var buildsController = new BuildsController(buildCommunicator, 3, 5);
            var result = buildsController.Update(projectName, buildDefinition) as ViewResult;

            Assert.IsNotNull(result);

            var model = result.Model as BuildStatusModel;
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void IndexExpectCorrectProjectMapping()
        {
            const string definitionName = "Definition";
            const string projectName = "Project Name";
            const string status = "Success";
            DateTime start = DateTime.Now.AddMinutes(-4d);
            DateTime end = DateTime.Now;

            var buildStatus = new BuildStatus();
            var project = new Project
            {
                DefinitionName = definitionName,
                Name = projectName,
                Status = status,
                StartTime = start,
                FinishTime = end
            };
            var run = new Project();
            project.Runs.Add(run);
            buildStatus.Projects.Add(project);

            this.buildCommunicator
                .Stub(bc => bc.GetBuildInformation(2, 4, "*", string.Empty))
                .Return(buildStatus);


            var buildsController = new BuildsController(buildCommunicator, 2, 4);
            var result = buildsController.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);

            var model = result.Model as BuildStatusModel;

            Assert.AreEqual(definitionName, model.Projects[0].DefinitionName);
            Assert.AreEqual(projectName, model.Projects[0].Name);
            Assert.AreEqual(status, model.Projects[0].Status);
            Assert.AreEqual(start, model.Projects[0].StartTime);
            Assert.AreEqual(end, model.Projects[0].FinishTime);
        }

        [TestMethod]
        public void IndexExpectCorrectRunMapping()
        {
            const string definitionName = "Definition";
            const string projectName = "Project Name";
            const string status = "Success";
            DateTime start = DateTime.Now.AddMinutes(-4d);
            DateTime end = DateTime.Now;

            var buildStatus = new BuildStatus();
            var project = new Project();
            var run = new Project
            {
                DefinitionName = definitionName,
                Name = projectName,
                Status = status,
                StartTime = start,
                FinishTime = end
            };
            project.Runs.Add(run);
            buildStatus.Projects.Add(project);

            this.buildCommunicator
                .Stub(bc => bc.GetBuildInformation(2, 4, "*", string.Empty))
                .Return(buildStatus);


            var buildsController = new BuildsController(buildCommunicator, 2, 4);
            var result = buildsController.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);

            var model = result.Model as BuildStatusModel;

            Assert.AreEqual(definitionName, model.Projects[0].Runs[0].DefinitionName);
            Assert.AreEqual(projectName, model.Projects[0].Runs[0].Name);
            Assert.AreEqual(status, model.Projects[0].Runs[0].Status);
            Assert.AreEqual(start, model.Projects[0].Runs[0].StartTime);
            Assert.AreEqual(end, model.Projects[0].Runs[0].FinishTime);
        }
    }
}
