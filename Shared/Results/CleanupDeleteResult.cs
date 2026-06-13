namespace GameBoost.Shared.Results
{
    public sealed class CleanupDeleteResult
    {
        public long DeletedBytes { get; init; }

        public int DeletedFiles { get; init; }

        public int FailedFiles { get; init; }

        public int DeletedDirectories { get; init; }

        public static CleanupDeleteResult Empty { get; } = new();
    }
}
