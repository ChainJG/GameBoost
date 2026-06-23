using GameBoost.Application;
using GameBoost.Infrastructure.Installers.Models;
using GameBoost.Infrastructure.Installers.Services;
using GameBoost.MVVM.Core;
using GameBoost.MVVM.ViewModels.Shared.Info;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Models;
using GameBoost.Shared.Results;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace GameBoost.MVVM.ViewModels
{
    public sealed class ApplicationInstallerViewModel : ObservableObject
    {
        private readonly GameBoostUIServices _uiService;
        private readonly ApplicationInstallerLoaderService _loaderService = new();
        private readonly ApplicationInstallerService _installerService = new();

        private readonly Dictionary<string, CancellationTokenSource> _activeInstallations = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, IReadOnlyList<InfoCardViewModel>> _cardsByCategory = new(StringComparer.OrdinalIgnoreCase);

        private readonly List<InfoCardViewModel> _allCards = [];
        private readonly List<InfoCardViewModel> _filteredCards = [];

        private CancellationTokenSource? _loadCancellation;

        #region Batch Properties
        private const int AppCardBatchSize = 32;
        private int _loadedAppCount;
        #endregion

        #region Collections
        public ObservableCollection<InfoCardViewModel> AppCards { get; } = [];
        public ObservableCollection<CategoryFiltersDefinition> CategoryFilters { get; } = [];
        #endregion

        #region Commands
        public ICommand InstallOrCancelAppCommand { get; }
        public ICommand LoadMoreAppsCommand { get; }
        private void RaiseCommandStates()
        {
            if (LoadMoreAppsCommand is AsyncRelayCommand loadMoreCommand)
                loadMoreCommand.RaiseCanExecuteChanged();
        }
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

                RebuildFilteredCards();
            }
        }
        #endregion

        #region Category Properties
        private static readonly CategoryFiltersDefinition AllCategory = new()
        {
            Category = "All",
            Icon = PackIconKind.ArrowAll
        };

        private CategoryFiltersDefinition _selectedCategory = AllCategory;
        public CategoryFiltersDefinition SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (value is null)
                    return;

                if (!Set(ref _selectedCategory, value))
                    return;

                RebuildFilteredCards();
            }
        }
        #endregion

        #region Loading Properties
        private bool _isLoaded;
        public bool IsLoaded
        {
            get => _isLoaded;
            private set
            {
                if (!Set(ref _isLoaded, value))
                    return;

                RaiseCommandStates();
            }
        }

        private bool _isLoading = true;
        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                if (!Set(ref _isLoading, value))
                    return;

                RaiseCommandStates();
            }
        }

        private bool _loadFailed;
        public bool LoadFailed
        {
            get => _loadFailed;
            private set => Set(ref _loadFailed, value);
        }
        #endregion

        #region Display UI
        private int _availableCount;
        public int AvailableCount
        {
            get => _availableCount;
            private set => Set(ref _availableCount, value);
        }

        private int _installedCount;
        public int InstalledCount
        {
            get => _installedCount;
            private set => Set(ref _installedCount, value);
        }

        public int LoadedAppCardCount => AppCards.Count;
        public int TotalFilteredAppCount => _filteredCards.Count;
        public bool CanLoadMoreApps => _loadedAppCount < _filteredCards.Count;

        private string _statusText = "Loading application catalog...";
        public string StatusText
        {
            get => _statusText;
            private set => Set(ref _statusText, value);
        }
        #endregion

        public ApplicationInstallerViewModel(GameBoostUIServices uiService)
        {
            _uiService = uiService;

            InstallOrCancelAppCommand = new RelayCommand<InfoCardViewModel>(InstallOrCancelApp);
            LoadMoreAppsCommand = new AsyncRelayCommand(LoadMoreAppsAsync, CanLoadMoreAppCards);

            StartBackgroundLoad();
        }

        #region Load In Background Methods
        private void StartBackgroundLoad()
        {
            _loadCancellation?.Cancel();
            _loadCancellation?.Dispose();

            _loadCancellation = new CancellationTokenSource();

            _= LoadInBackgroundAsync(_loadCancellation.Token);

        }
        private async Task LoadInBackgroundAsync(CancellationToken token)
        {
            try
            {
                IsLoading = true;
                IsLoaded = false;
                LoadFailed = false;
                StatusText = "Loading Application Catalog...";

                var result = await _loaderService.LoadAsync(InstallOrCancelAppCommand, token);

                token.ThrowIfCancellationRequested();

                await UIHelper.RunOnUiThreadAsync(() =>
                {
                    IsLoaded = true;
                    IsLoading = false;
                    LoadFailed = false;

                    ApplyLoadResult(result);
                });
            }
            catch (OperationCanceledException)
            {
                // Ignore cancellation.
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Application installer catalog failed to load: {ex.Message}");
#endif

                await UIHelper.RunOnUiThreadAsync(() =>
                {
                    IsLoading = false;
                    IsLoaded = false;
                    LoadFailed = true;
                    StatusText = "Failed to load application catalog";
                });
            }
        }
        #endregion

        #region Installation Application

        // ICommand Method
        private void InstallOrCancelApp(InfoCardViewModel? card)
        {
            if (card?.Content is not AppInstallDefinition app)
                return;

            if (app.IsInstalled)
                return;

            if (_activeInstallations.TryGetValue(app.Id, out var activeInstall))
            {
                activeInstall.Cancel();
                return;
            }

            _ = InstallAppAsync(app, card);
        }
        private async Task InstallAppAsync(AppInstallDefinition app, InfoCardViewModel card)
        {
            using var cancellation = new CancellationTokenSource();

            _activeInstallations[app.Id] = cancellation;

            _uiService.GlobalOperations.BeginOperation();

            try
            {
                app.IsInstalling = true;
                app.InstallFailed = false;

                UpdateCardState(card, app);

                var progress = new Progress<ProgressResult>(UpdateProgress);

                var result = await _installerService.InstallAsync(app, progress, cancellation.Token);

                if (result.Cancelled)
                {
                    app.IsInstalling = false;
                    app.InstallFailed = false;

                    StatusText = result.Message;

                    UpdateCardState(card, app);
                    return;
                }

                if (!result.Success)
                {
                    app.InstallFailed = true;
                    app.IsInstalling = false;
                    app.IsInstalled = false;

                    UpdateCardState(card, app);
                    return;
                }

                app.IsInstalling = false;
                app.InstallFailed = false;
                app.IsInstalled = true;

                InstalledCount++;
                UpdateCardState(card, app);
            }
            catch (Exception ex)
            {
                app.IsInstalling = false;
                app.InstallFailed = true;
                app.IsInstalled = false;

                StatusText = $"Failed to install {app.DisplayName}: {ex.Message}";

                UpdateCardState(card, app);
            }
            finally
            {
                _activeInstallations.Remove(app.Id);
                _uiService.GlobalOperations.EndOperation();

                RaiseCommandStates();
            }
        }

        private void UpdateProgress(ProgressResult result)
        {
            StatusText = $"{result.Status}";
        }

        private void UpdateCardState(InfoCardViewModel card, AppInstallDefinition app)
        {
            card.State = _loaderService.GetCardState(app);
            card.Footer = _loaderService.GetFooterText(app);
        }
        #endregion

        #region Build Methods
        private void ApplyLoadResult(ApplicationInstallerLoadResult result)
        {
            _allCards.Clear();
            _allCards.AddRange(result.AllCards);

            _cardsByCategory.Clear();

            foreach(var pair in result.CardsByCategory)
                _cardsByCategory[pair.Key] = pair.Value;

            AvailableCount = result.AvailableCount;
            InstalledCount = result.InstalledCount;

            CategoryFilters.Clear();

            foreach(var filter in result.CategoryFilters)
                CategoryFilters.Add(filter);

            SelectedCategory =
                CategoryFilters.FirstOrDefault(filter =>
                    string.Equals(filter.Category, "All", StringComparison.OrdinalIgnoreCase))
                ?? CategoryFilters.FirstOrDefault()
                ?? AllCategory;

            StatusText = $"Loaded {AvailableCount} applications";

            RebuildFilteredCards();
        }

        private void RebuildFilteredCards()
        {
            if (!IsLoaded)
                return;

            var sourceCards = GetCardsForSelectedCategory();

            if (!string.IsNullOrWhiteSpace(SearchText))
                sourceCards = [.. sourceCards.Where(MatchesSearchText)];

            _filteredCards.Clear();
            _filteredCards.AddRange(sourceCards);

            ResetVisibleAppCards();

            OnPropertyChanged(nameof(TotalFilteredAppCount));
            OnPropertyChanged(nameof(LoadedAppCardCount));
            OnPropertyChanged(nameof(CanLoadMoreApps));

            StatusText = $"Loaded {TotalFilteredAppCount}";

            RaiseCommandStates();
        }
        private IReadOnlyList<InfoCardViewModel> GetCardsForSelectedCategory()
        {
            if (string.Equals(SelectedCategory.Category, AllCategory.Category, StringComparison.OrdinalIgnoreCase))
                return _allCards;

            return _cardsByCategory.TryGetValue(
                SelectedCategory.Category,
                out var cards)
                    ? cards
                    : [];
        }
        private void ResetVisibleAppCards()
        {
            AppCards.Clear();

            _loadedAppCount = 0;

            LoadNextAppCardBatch();
        }
        #endregion

        #region Load Batch Methods
        private void LoadNextAppCardBatch()
        {
            if (!CanLoadMoreApps)
                return;

            var nextCards = _filteredCards
                .Skip(_loadedAppCount)
                .Take(AppCardBatchSize)
                .ToList();

            foreach (var card in nextCards)
                AppCards.Add(card);

            _loadedAppCount += nextCards.Count;

            OnPropertyChanged(nameof(LoadedAppCardCount));
            OnPropertyChanged(nameof(CanLoadMoreApps));

            RaiseCommandStates();
        }
        private bool CanLoadMoreAppCards()
        {
            return IsLoaded &&
                   !IsLoading &&
                   CanLoadMoreApps;
        }

        // ICommand Method
        private Task LoadMoreAppsAsync()
        {
            LoadNextAppCardBatch();

            return Task.CompletedTask;
        }
        #endregion

        #region Match Methods
        private bool MatchesSearchText(
            InfoCardViewModel card)
        {
            if (card.Content is not AppInstallDefinition app)
                return false;

            return app.DisplayName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                   || app.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                   || app.Tags.Any(tag => tag.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }
        #endregion
    }
}
