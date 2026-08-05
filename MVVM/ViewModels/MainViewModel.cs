using GameBoost.Application.Operations;
using GameBoost.Application.Selection.Services;
using GameBoost.Application.Startup;
using GameBoost.Application.Titlebar;
using GameBoost.Core.Dock;
using GameBoost.MVVM.Core;
using GameBoost.MVVM.UserControls.Shared.Titlebar;
using GameBoost.MVVM.ViewModels.Shared.Selection;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GameBoost.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly AsyncRelayCommand _dockActionCommand;

        private readonly StartupNotificationService _startupNotifications;
        private readonly StartupStateService _startupState;
        private readonly RecommendedActionService _recommendedActions;

        private SelectionViewModel? _activeSelectionViewModel;

        public ObservableCollection<DockItem> Pages { get; }

        public ObservableCollection<TitleBarActionViewModel> TitleBarActions { get; }
        public GlobalOperationService GlobalOperations { get; }

        public ICommand DockActionCommand => _dockActionCommand;

        private object? _currentView;
        public object? CurrentView
        {
            get => _currentView;
            set
            {
                if (!Set(ref _currentView, value))
                    return;

                AttachSelectionViewModel(value);
                RefreshDockActionState();
            }
        }

        private DockItem? _selectedPage;
        public DockItem? SelectedPage
        {
            get => _selectedPage;
            set
            {
                if (value is null)
                    return;

                if (!Set(ref _selectedPage, value))
                    return;

                Navigate(value);
            }
        }

        public static string AppVersionText => UIHelper.VersionText;
        private string _pageTitle = string.Empty;
        public string PageTitle
        {
            get => _pageTitle;
            set => Set(ref _pageTitle, value);
        }

        private PackIconKind _pageIcon = PackIconKind.Home;
        public PackIconKind PageIcon
        {
            get => _pageIcon;
            set => Set(ref _pageIcon, value);
        }

        public string DockActionText =>
            _activeSelectionViewModel?.DisplayScreenType switch
            {
                SelectionScreenType.Execution => "Cancel",
                SelectionScreenType.Result => "Back",
                SelectionScreenType.Selection => "Apply",
                _ => "Apply"
            };

        public PackIconKind DockActionIcon =>
            _activeSelectionViewModel?.DisplayScreenType switch
            {
                SelectionScreenType.Execution => PackIconKind.Close,
                SelectionScreenType.Result => PackIconKind.ArrowLeft,
                SelectionScreenType.Selection => PackIconKind.Play,
                _ => PackIconKind.Play
            };

        public bool IsDockActionEnabled => CanUseDockAction();

        /// <summary>The dock's expanded/compact state, bound by the view.</summary>
        public DockState DockState => CanUseDockAction()
            ? DockState.Full
            : DockState.Compact;

        public MainViewModel(
            GlobalOperationService globalOperations,
            TitleBarActionService titleBarActions,
            StartupNotificationService startupNotifications,
            StartupStateService startupState,
            RecommendedActionService recommendedActions,
            HomeViewModel homeViewModel,
            WindowsViewModel windowsViewModel,
            SystemViewModel systemViewModel,
            StorageViewModel storageViewModel,
            ApplicationInstallerViewModel applicationInstallerViewModel)
        {
            GlobalOperations = globalOperations;
            TitleBarActions = titleBarActions.Actions;

            _startupNotifications = startupNotifications;
            _startupState = startupState;
            _recommendedActions = recommendedActions;

            _dockActionCommand = new AsyncRelayCommand(
                ExecuteDockAction,
                CanUseDockAction);

            _recommendedActions.RegisterSelectionPages(
            [
                windowsViewModel,
                systemViewModel
            ]);

            Pages =
            [
                new DockItem("Home", PackIconKind.Home, homeViewModel),
                new DockItem("Windows", PackIconKind.MicrosoftWindows, windowsViewModel),
                new DockItem("System", PackIconKind.Computer, systemViewModel),
                new DockItem("Storage", PackIconKind.Storage, storageViewModel),
                new DockItem("Installer", PackIconKind.ApplicationArray, applicationInstallerViewModel)
            ];

            SelectedPage = Pages[0];
        }

        public async Task InitialiseStartup(IProgress<ProgressResult>? progress = null, CancellationToken token = default)
        {
            _startupNotifications.AddStartupActions();

            await _recommendedActions.RefreshAllAsync(progress, token);

            _startupState.NotifyStartupCompleted();
        }

        private void Navigate(DockItem page)
        {
            CurrentView = page.ViewModel;
            PageTitle = page.Title;
            PageIcon = page.Icon;

            OnPropertyChanged(nameof(DockState));
        }

        private void AttachSelectionViewModel(object? viewModel)
        {
            if (_activeSelectionViewModel is not null)
                _activeSelectionViewModel.StateChanged -= OnSelectionStateChanged;

            _activeSelectionViewModel = viewModel as SelectionViewModel;

            if (_activeSelectionViewModel is not null)
                _activeSelectionViewModel.StateChanged += OnSelectionStateChanged;
        }

        private void OnSelectionStateChanged()
        {
            RefreshDockActionState();
        }


        private void RefreshDockActionState()
        {
            OnPropertyChanged(nameof(DockActionText));
            OnPropertyChanged(nameof(DockActionIcon));
            OnPropertyChanged(nameof(IsDockActionEnabled));
            OnPropertyChanged(nameof(DockState));

            _dockActionCommand.RaiseCanExecuteChanged();
        }

        private bool CanUseDockAction()
        {
            return _activeSelectionViewModel?.DisplayScreenType switch
            {
                SelectionScreenType.Selection => _activeSelectionViewModel.HasRunnableSelection,
                SelectionScreenType.Execution => true,
                SelectionScreenType.Result => true,
                _ => false
            };
        }

        private async Task ExecuteDockAction()
        {
            if (_activeSelectionViewModel is null)
                return;

            switch (_activeSelectionViewModel.DisplayScreenType)
            {
                case SelectionScreenType.Selection:
                    await _activeSelectionViewModel.ExecuteSelectedActionsAsync();
                    break;

                case SelectionScreenType.Execution:
                    _activeSelectionViewModel.CancelExecution();
                    break;

                case SelectionScreenType.Result:
                    _activeSelectionViewModel.ReturnToSelection();
                    break;
            }

            RefreshDockActionState();
        }
    }
}
