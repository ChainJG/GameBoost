namespace GameBoost.Shared.Results
{
    public sealed class CleanupResult
    {
        public long DeletedBytes { get; set; }

        public int DeletedFiles { get; set; }

        public int FailedFiles { get; set; }

        public int DeletedDirectories { get; set; }

        public int FaildedDirectories { get; set; }

        public int SkippedDirectories { get; set; }

        public static CleanupResult Empty { get; } = new();
    }
}
