using GameBoost.Infrastructure.Installers.Catalog;
using GameBoost.Infrastructure.Installers.Models;
using GameBoost.MVVM.ViewModels.Shared.Info;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Models;
using MaterialDesignThemes.Wpf;
using System.Windows.Input;

namespace GameBoost.Infrastructure.Installers.Services
{
    public sealed class ApplicationInstallerLoaderService
    {
        public static async Task<ApplicationInstallerLoadResult> LoadAsync(ICommand InstallOrCancelAppCommand, CancellationToken token) =>
            await Task.Run(() => LoadCore(InstallOrCancelAppCommand, token), token);

        private static async Task<ApplicationInstallerLoadResult>? LoadCore(ICommand InstallOrCancelAppCommand, CancellationToken token)
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

            var cards = apps.Select(app => CreateAppCard(app, InstallOrCancelAppCommand)).ToList();

            var cardsByCategory = cards
                .Where(card => card.Content is AppInstallDefinition)
                .GroupBy(card => ((AppInstallDefinition)card.Content!).Category.ToString())
                .ToDictionary(
                    group => group.Key,
                    group => (IReadOnlyList<InfoCardViewModel>)
                    [.. group
                        .OrderBy(card => ((AppInstallDefinition)card.Content!).SortOrder)
                        .ThenBy(card => card.Title, StringComparer.OrdinalIgnoreCase)],
                    StringComparer.OrdinalIgnoreCase);

            var categoryFilters = BuildCategoryFilter(apps);

            return new ApplicationInstallerLoadResult
            {
                AllCards = cards,
                CardsByCategory = cardsByCategory,
                CategoryFilters = categoryFilters,
                AvailableCount = apps.Count,
                InstalledCount = apps.Count(app => app.IsInstalled)
            };
        }

        private static InfoCardViewModel CreateAppCard(AppInstallDefinition app, ICommand InstallOrCancelAppCommand)
        {
            return new InfoCardViewModel
            {
                State = GetCardState(app),
                Icon = app.Icon,
                Title = app.Description,
                Info =app.DisplayName,
                Footer = GetFooterText(app),
                Content = app,
                Command = InstallOrCancelAppCommand
            };
        }

        public static InfoCardState GetCardState(AppInstallDefinition app)
        {
            if (app.IsInstalled)
                return InfoCardState.Success;

            if (app.IsInstalling)
                return InfoCardState.Running;

            if (app.InstallFailed)
                return InfoCardState.Error;

            if (app.RequiresAdmin)
                return InfoCardState.Warning;

            return InfoCardState.Info;
        }

        public static string GetFooterText(AppInstallDefinition app)
        {
            if (app.IsInstalled)
                return "Installed";

            if (app.IsInstalling)
                return "Installing - click to cancel";

            if (app.InstallFailed)
                return "Failed - click to retry";

            if (app.RequiresAdmin)
                return "Requires admin";

            return "Click to install";
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
