using System.IO;

namespace GameBoost.Features.Modules.SystemModules.Cleanup.AppDataOrphan
{
    public sealed class DirectoryScanCandidate
    {
        public required DirectoryInfo Directory { get; init; }

        public required string DisplayName { get; init; }

        public required string Reason { get; init; }

        public required DirectoryScanConfidence Confidence { get; init; }

        public required long SizeBytes { get; init; }

        public required int FileCount { get; init; }

        public required int DirectoryCount { get; init; }

        public required DateTime LastWriteTimeUtc { get; init; }

        public bool IsEmpty => FileCount == 0 && DirectoryCount == 0;
    }

    public enum DirectoryScanConfidence
    {
        None,
        Low,
        Medium,
        High
    }
}
