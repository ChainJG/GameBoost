using System.Diagnostics;

namespace GameBoost.Shared.Results
{
    public sealed class ProcessResult
    {
        public int DetectedCount { get; set; }

        public int ClosedCount { get; set; }

        public int KilledCount { get; set; }

        public int SkippedCount { get; set; }

        public int FailedCount { get; set; }

        public List<string> Messages { get; } = [];
    }
}
