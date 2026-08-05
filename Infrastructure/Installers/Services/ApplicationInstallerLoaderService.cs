using GameBoost.Infrastructure.Installers.Catalog;
using GameBoost.Infrastructure.Installers.Models;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Models;
using MaterialDesignThemes.Wpf;

namespace GameBoost.Infrastructure.Installers.Services
{
    public sealed class ApplicationInstallerLoaderService
    {
        public static async Task<ApplicationInstallerLoadResult> LoadAsync(CancellationToken token) =>
            await Task.Run(() => LoadCore(token), token);

        private static ApplicationInstallerLoadResult LoadCore(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var installedPrograms = InstalledProgramSnapshot.GetCached();

            var apps = AppInstallCatalog.GetApps()
                .OrderBy(app => app.SortOrder)
                .ThenBy(app => app.DisplayName, StringComparer.OrdinalIgnoreCase).ToList();

            foreach (var app in apps)
            {
                token.ThrowIfCancellationRequested();
                app.IsInstalled = installedPrograms.ContainsAny(app.InstalledProgramNames);
            }

            return new ApplicationInstallerLoadResult
            {
                Apps = apps,
                CategoryFilters = BuildCategoryFilter(apps),
                AvailableCount = apps.Count,
                InstalledCount = apps.Count(app => app.IsInstalled)
            };
        }

        #region Category Methods
        // Creates category filters base on the active available AppInstallDefinition
        private static List<CategoryFiltersDefinition> BuildCategoryFilter(IEnumerable<AppInstallDefinition> apps) =>
        [
            // Adds All Category
            new CategoryFiltersDefinition
            {
                Category = "All",
                Icon = PackIconKind.ViewGrid,
            },

            .. apps
                .GroupBy(app => app.Category)
                .OrderBy(group => group.Key.ToString(), StringComparer.OrdinalIgnoreCase)
                .Select(group => new CategoryFiltersDefinition
                {
                    Category = group.Key.ToString(),
                    Icon = group.First().Icon
                })
        ];
        #endregion
    }
}
