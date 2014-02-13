using System;
using System.Collections.Generic;
using TfsCommunicator;

namespace TfsCruiser.Models
{
    public class ProjectModel
    {
        public ProjectModel()
        {
            Runs = new List<ProjectModel>();
        }

        public string DefinitionName { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public List<ProjectModel> Runs { get; set; }

        private double duration;
        public double Duration
        {
            get
            {
                if (this.duration == 0d)
                {
                    this.duration = Math.Round((FinishTime - StartTime).TotalMinutes, 1);
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
            };

            foreach (var run in project.Runs)
            {
                projectModel.Runs.Add(ProjectModel.Map(run));
            }

            return projectModel;
        }
    }
}