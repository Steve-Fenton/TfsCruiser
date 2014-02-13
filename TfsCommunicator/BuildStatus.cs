using System;
using System.Collections.Generic;

namespace TfsCommunicator
{
    public class BuildStatus
    {
        public BuildStatus()
        {
            Projects = new List<Project>();
        }

        public List<Project> Projects { get; private set; }
    }
}