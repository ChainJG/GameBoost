namespace GameBoost.Features.Modules.SystemModules.Cleanup.Options
{
    public sealed class CleanupScanOptions
    {
        public bool ProbeDeleteAccess { get; init; } = true;

        public TimeSpan? IgnoreFilesNewerThan { get; init; } =
            TimeSpan.FromMinutes(10);

        public int MaxDegreeOfParallelism { get; init; } =
            Math.Clamp(Environment.ProcessorCount / 2, 2, 6);
    }
}
