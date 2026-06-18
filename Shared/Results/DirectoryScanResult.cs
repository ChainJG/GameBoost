using GameBoost.Features.Modules.SystemModules.Cleanup.AppDataOrphan;

namespace GameBoost.Shared.Results
{
    public sealed class DirectoryScanResult
    {
        public required IReadOnlyList<DirectoryScanCandidate> Candidates { get; init; }

        public required long TotalBytes { get; init; }

        public required int ScannedFolders { get; init; }

        public required int SkippedFolders { get; init; }

        public int CandidateCount => Candidates.Count;
        public static DirectoryScanResult Empty { get; } = new()
        {
            Candidates = [],
            TotalBytes = 0,
            ScannedFolders = 0,
            SkippedFolders = 0
        };
    }

    public enum DirectoryScanConfidence
    {
        None,
        Low,
        Medium,
        High
    }
}
