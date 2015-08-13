using System;
using System.Linq;
using System.Collections.Generic;
using TfsCommunicator;

namespace TfsCruiser.Models
{
    public class ProjectModel
    {
        public ProjectModel()
        {
            PreviousRuns = new List<ProjectModel>();
        }

        public string DefinitionName { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string BuildNumber { get; set; }
        public string RequestedFor { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public List<ProjectModel> PreviousRuns { get; set; }

        private double duration;
        public double Duration
        {
            get
            {
                if (this.duration == 0d)
                {
                    this.duration = Math.Round((FinishTime - StartTime).TotalMinutes, 0);
                }
                return this.duration;
            }
        }

        public static ProjectModel Map(Project project)
        {
            var projectModel = new ProjectModel
            {
                DefinitionName = project.DefinitionName,
                Name = project.Name,
                Status = project.Status,
                StartTime = project.StartTime,
                FinishTime = project.FinishTime,
                BuildNumber = project.BuildNumber,
                RequestedFor = project.RequestedFor
            };

            foreach (var run in project.Runs)
            {
                projectModel.PreviousRuns.Add(ProjectModel.Map(run));
            }

            projectModel.PreviousRuns = projectModel.PreviousRuns.OrderBy(r => r.FinishTime).ToList();

            return projectModel;
        }
    }
}