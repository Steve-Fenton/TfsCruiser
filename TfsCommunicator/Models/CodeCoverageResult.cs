using System;

namespace TfsCommunicator.Models
{
    public class CodeCoverageResult
    {
        public DateTime CompletedTime { get; set; }
        public double Coverage { get; set; }

        public static CodeCoverageResult Map(DateTime finishTime, double coverage)
        {
            return new CodeCoverageResult()
            {
                CompletedTime = finishTime,
                Coverage = coverage
            };
        }
    }
}