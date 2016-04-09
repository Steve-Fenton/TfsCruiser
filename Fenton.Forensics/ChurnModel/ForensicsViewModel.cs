namespace Fenton.Forensics
{
    using System;

    public class ForensicsViewModel
    {
        public string Path { get; set; }

        public string SelectedPath { get; set; }

        public int FolderDepth { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public Churn Churn { get; set; }
    }
}