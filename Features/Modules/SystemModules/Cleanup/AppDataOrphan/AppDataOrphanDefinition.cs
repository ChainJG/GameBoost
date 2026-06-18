namespace GameBoost.Features.Modules.SystemModules.Cleanup.AppDataOrphan
{
    public sealed class AppDataOrphanDefinition
    {
        public required string DisplayName { get; init; }

        public required IReadOnlyList<string> FolderNames { get; init; }

        public IReadOnlyList<string> InstalledProgramNames { get; init; } = [];

        public IReadOnlyList<string> ProcessNames { get; init; } = [];

        public bool HighRisk { get; init; }

        public bool DeleteWhenEmptyOnly { get; init; }

        public TimeSpan MinimumAge { get; init; } = TimeSpan.FromDays(180);
    }
}
