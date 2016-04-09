namespace Fenton.Forensics
{
    using System;

    public class ChurnHistory
    {
        public DateTime StartOfWeek { get; set; }

        public string ItemName { get; set; }

        public int ChangeCount { get; set; }
    }
}