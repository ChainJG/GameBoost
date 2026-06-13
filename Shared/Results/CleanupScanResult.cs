namespace GameBoost.Shared.Results
{
    public sealed class CleanupScanResult
    {
        public long EstimatedDeletableBytes { get; init; }

        public int ScannedFiles { get; init; }

        public int DeletableFiles { get; init; }

        public int SkippedFiles { get; init; }

        public int InaccessibleFiles { get; init; }

        public int LockedFiles { get; init; }

        public int ScannedDirectories { get; init; }

        public static CleanupScanResult Empty { get; } = new();
    }
}
