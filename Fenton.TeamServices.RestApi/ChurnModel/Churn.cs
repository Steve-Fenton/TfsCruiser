namespace Fenton.TeamServices.BuildRestApi
{
    using System.Collections.Generic;

    public class Churn
    {
        public List<FileChurn> Files { get; set; }

        public List<FolderChurn> Folders { get; set; }
    }
}