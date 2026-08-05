using GameBoost.Shared.Models;

namespace GameBoost.Infrastructure.Installers.Models
{
    public sealed class ApplicationInstallerLoadResult
    {
        public required IReadOnlyList<AppInstallDefinition> Apps { get; init; }

        public required IReadOnlyList<CategoryFiltersDefinition> CategoryFilters { get; init; }

        public required int InstalledCount { get; init; }

        public required int AvailableCount { get; init; }
    }
}
