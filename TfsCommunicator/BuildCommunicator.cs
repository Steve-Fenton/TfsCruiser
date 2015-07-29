using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;

namespace TfsCommunicator
{
    [ExcludeFromCodeCoverage]
    public class BuildCommunicator : IBuildCommunicator
    {
        private string tfsServerAddress;
        private NetworkCredential credentialsProvider;

        public BuildCommunicator(string tfsServerAddress, NetworkCredential credentialsProvider)
        {
            this.tfsServerAddress = tfsServerAddress;
            this.credentialsProvider = credentialsProvider;
        }

        public BuildStatus GetBuildInformation(int maxDays = 5, int maxRuns = 10, string teamProject = "*", string buildDefinition = "")
        {
            var buildStatus = new BuildStatus();

            var builds = GetBuildsFromTfs(maxDays, teamProject, buildDefinition);

            var currentDefinition = string.Empty;

            foreach (var build in builds)
            {
                string definitionName = build.BuildDefinition.Name;
                var project = MapBuildToProject(build, definitionName);

                if (IsChildBuild(currentDefinition, definitionName))
                {
                    AddBuildToParentProject(buildStatus, definitionName, project, maxRuns);
                }
                else
                {
                    currentDefinition = definitionName;
                    AddBuild(buildStatus, project);
                }
            }

            return buildStatus;
        }

        private static void AddBuild(BuildStatus buildStatus, Project project)
        {
            buildStatus.Projects.Add(project);
        }

        private static bool IsChildBuild(string currentDefinition, string definitionName)
        {
            return definitionName == currentDefinition;
        }

        private IOrderedEnumerable<IBuildDetail> GetBuildsFromTfs(int maxDays, string teamProject, string buildDefinition)
        {
            IBuildServer buildServer = GetBuildServer();

            IBuildDetailSpec spec = GetBuildDetailSpec(maxDays, teamProject, buildDefinition, buildServer);

            var builds = buildServer.QueryBuilds(spec).Builds
                .OrderBy(b => b.BuildDefinition.Name)
                .ThenByDescending(b => b.FinishTime);
            
            return builds;
        }

        private IBuildServer GetBuildServer()
        {            
            FederatedCredential credentials = (tfsServerAddress.StartsWith("https")) ? 
                            (FederatedCredential) new BasicAuthCredential(credentialsProvider) : new SimpleWebTokenCredential(credentialsProvider.UserName, credentialsProvider.Password);

            TfsClientCredentials tfsCredentials = new TfsClientCredentials(credentials);
            tfsCredentials.AllowInteractive = false;

            var tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(tfsServerAddress));
            tfs.ClientCredentials = tfsCredentials;
            tfs.Authenticate();

            IBuildServer buildServer = tfs.GetService<IBuildServer>();
            return buildServer;
        }

        private static IBuildDetailSpec GetBuildDetailSpec(int maxDays, string teamProject, string buildDefinition, IBuildServer buildServer)
        {
            IBuildDetailSpec spec = string.IsNullOrEmpty(buildDefinition) ?
                buildServer.CreateBuildDetailSpec(teamProject) :
                buildServer.CreateBuildDetailSpec(teamProject, buildDefinition);

            spec.MinFinishTime = DateTime.Now.Subtract(TimeSpan.FromDays(maxDays));
            spec.MaxFinishTime = DateTime.Now;
            spec.QueryDeletedOption = QueryDeletedOption.IncludeDeleted;
            return spec;
        }


        private static Project MapBuildToProject(IBuildDetail build, string definitionName)
        {
            var project = new Project
            {
                DefinitionName = definitionName,
                Name = build.TeamProject,
                Status = build.Status.ToString(),
                StartTime = build.StartTime,
                FinishTime = build.FinishTime,
                BuildNumber = build.BuildNumber,
                RequestedFor = build.RequestedFor
            };
            return project;
        }

        private void AddBuildToParentProject(BuildStatus buildStatus, string definitionName, Project project, int maxRuns)
        {
            var parent = buildStatus.Projects.First(p => p.DefinitionName == definitionName);
            if (parent.Runs.Count < maxRuns)
            {
                parent.Runs.Add(project);
            }
        }
    }
}
