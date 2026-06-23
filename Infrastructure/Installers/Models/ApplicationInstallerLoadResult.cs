using GameBoost.MVVM.ViewModels.Shared.Info;
using GameBoost.Shared.Models;

namespace GameBoost.Infrastructure.Installers.Models
{
    public sealed class ApplicationInstallerLoadResult
    {
        public required IReadOnlyList<InfoCardViewModel> AllCards { get; init; }

        public required IReadOnlyDictionary<string, IReadOnlyList<InfoCardViewModel>> CardsByCategory { get; init; }

        public required IReadOnlyList<CategoryFiltersDefinition> CategoryFilters { get; init; }

        public required int InstalledCount { get; init; }

        public required int AvailableCount { get; init; }
    }
}
