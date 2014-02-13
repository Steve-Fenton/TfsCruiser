using System;
using System.Collections.Generic;
using TfsCommunicator;

namespace TfsCruiser.Models
{
    public class BuildStatusModel
    {
        public BuildStatusModel()
        {
            Projects = new List<ProjectModel>();
        }

        public List<ProjectModel> Projects { get; private set; }

        private decimal boxHeightPercentage;

        public decimal BoxHeightPercentage
        {
            get
            {
                if (this.boxHeightPercentage == 0m)
                {
                    this.boxHeightPercentage = (100.00m / Math.Ceiling((decimal)Projects.Count / 2)) - 1m;
                }
                return this.boxHeightPercentage;
            }
        }

        public static BuildStatusModel Map(BuildStatus buildStatus)
        {
            var buildStatusModel = new BuildStatusModel();

            foreach (var project in buildStatus.Projects)
            {
                buildStatusModel.Projects.Add(ProjectModel.Map(project));
            }

            return buildStatusModel;
        }
    }
}