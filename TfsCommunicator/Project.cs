using System;
using System.Collections.Generic;

namespace TfsCommunicator
{
    public class Project
    {
        public Project()
        {
            Runs = new List<Project>();
        }

        public string DefinitionName { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string BuildNumber { get; set; }
        public string RequestedFor { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime FinishTime { get; set; }
        public List<Project> Runs { get; set; }
    }
}