using GameBoost.Application;
using GameBoost.Infrastructure.Installers.Catalog;
using GameBoost.Infrastructure.Installers.Models;
using GameBoost.MVVM.Core;
using GameBoost.MVVM.ViewModels.Shared.Info;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Models;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Packaging;
using System.Windows.Input;

namespace GameBoost.MVVM.ViewModels
{
    public sealed class ApplicationInstallerViewModel : ObservableObject
    {
        private readonly GameBoostUIServices _uiService;

        private readonly List<AppInstallDefinition> _allApps = [];

        #region Collections
        public ObservableCollection<InfoCardViewModel> AppCards { get; } = [];
        public ObservableCollection<CategoryFiltersDefinition> CategoryFilters { get; } = [];
        #endregion

        #region Commands
        public ICommand SelectCategoryCommand { get; }
        #endregion

        #region Search Text Properties
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (!Set(ref _searchText, value))
                    return;

                RefreshAppCards();
            }
        }
        #endregion

        #region Category Properties
        private static CategoryFiltersDefinition AllCategory = new()
        {
            Category = "All",
            Icon = PackIconKind.ArrowAll
        };

        private CategoryFiltersDefinition _selectedCategory = AllCategory;
        public CategoryFiltersDefinition SelectedCategory
        {
            get => _selectedCategory;
            private set
            {
                if (!Set(ref _selectedCategory, value))
                    return;

                RefreshAppCards();
            }
        }
        #endregion


        public ApplicationInstallerViewModel(GameBoostUIServices uiService)
        {
            _uiService = uiService;

            SelectCategoryCommand = new AsyncRelayCommand<CategoryFiltersDefinition>(SelectCategoryAsync);

            LoadApps();
        }

        private void LoadApps()
        {
            _allApps.Clear();

            _allApps.AddRange(AppInstallCatalog.GetApps());

            LoadCategoryFilters();

            RefreshInstalledStates();
        }

        private void RefreshInstalledStates()
        {
            var installedPrograms = InstalledProgramSnapshot.GetCached();

            foreach (var app in _allApps)
            {

                app.IsInstalled = installedPrograms.ContainsAny(
                    app.InstalledProgramNames);
            }

            RefreshAppCards();
        }

        private void RefreshAppCards()
        {
            var filteredApps = _allApps
                .Where(MatchesSelectedCategory)
                .Where(MatchesSearchText)
                .OrderBy(app => app.SortOrder)
                .ThenBy(app => app.DisplayName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            AppCards.Clear();

            foreach (var app in filteredApps)
                AppCards.Add(CreateAppCard(app));
        }

        private static InfoCardViewModel CreateAppCard(AppInstallDefinition app)
        {
            return new InfoCardViewModel
            {
                State = GetCardState(app),
                Icon = app.Icon,
                Title = app.Category.ToString(),
                Info = app.DisplayName,
                Footer = GetFooterText(app),
                Content = app,
            };
        }

        private static InfoCardState GetCardState(AppInstallDefinition app)
        {
            if (app.IsInstalled)
                return InfoCardState.Success;

            if (app.IsInstalling)
                return InfoCardState.Performance;

            if (app.InstallFailed)
                return InfoCardState.Error;

            if (app.RequiresAdmin)
                return InfoCardState.Warning;

            if (app.IsSelected)
                return InfoCardState.Recommended;

            return InfoCardState.Info;
        }
        private static string GetFooterText(AppInstallDefinition app)
        {
            if (app.IsInstalled)
                return "Installed";

            if (app.IsInstalling)
                return "Installing...";

            if (app.InstallFailed)
                return "Failed";

            if (app.IsSelected)
                return "Selected";

            if (app.RequiresAdmin)
                return "Requires admin";

            return app.Description;
        }

        #region Match Methods
        private bool MatchesSelectedCategory(AppInstallDefinition app)
        {
            if (string.Equals(SelectedCategory.Category, AllCategory.Category, StringComparison.OrdinalIgnoreCase))
                return true;

            return string.Equals(app.Category.ToString(), SelectedCategory.Category, StringComparison.OrdinalIgnoreCase);
        }

        private bool MatchesSearchText(AppInstallDefinition app)
        {
            if (string.IsNullOrWhiteSpace(SearchText))
                return true;

            return app.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                   || app.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                   || app.Tags.Any(tag => tag.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }
        #endregion


        #region Category Methods
        private void LoadCategoryFilters()
        {
            CategoryFilters.Clear();

            CategoryFilters.Add(AllCategory);

            var categories = _allApps
                .Select(app => app.Category)
                .Distinct()
                .OrderBy(category => category.ToString(), StringComparer.OrdinalIgnoreCase);

            foreach (var category in categories)
                CategoryFilters.Add(CreateCategroyFilter(category));
        }

        private CategoryFiltersDefinition CreateCategroyFilter(AppInstallCategory category)
        {
            var icon = category switch
            {
                AppInstallCategory.Browser => PackIconKind.Web,
                AppInstallCategory.Communication => PackIconKind.MessageText,
                AppInstallCategory.Gaming => PackIconKind.ControllerClassic,
                AppInstallCategory.Launcher => PackIconKind.RocketLaunch,
                AppInstallCategory.Utility => PackIconKind.Tools,
                AppInstallCategory.Media => PackIconKind.PlayCircle,
                AppInstallCategory.Development => PackIconKind.CodeTags,
                AppInstallCategory.Hardware => PackIconKind.Chip,
                AppInstallCategory.Streaming => PackIconKind.Broadcast,
                AppInstallCategory.Productivity => PackIconKind.BriefcaseCheck,
                _ => PackIconKind.Shape
            };

            return new CategoryFiltersDefinition
            {
                Category = category.ToString(),
                Icon = icon
            };
        }

        private Task SelectCategoryAsync(CategoryFiltersDefinition? category)
        {
            if (category is null || string.IsNullOrWhiteSpace(category.Category))
                return Task.CompletedTask;

            SelectedCategory = category;

            return Task.CompletedTask;
        }
        #endregion
    }
}
