using GameBoost.Application;
using GameBoost.Application.Operations;
using GameBoost.Application.Selection;
using GameBoost.Application.Startup;
using GameBoost.Application.Titlebar;
using GameBoost.Core.Dock;
using GameBoost.Core.EventArguments;
using GameBoost.MVVM.Core;
using GameBoost.MVVM.UserControls.Shared.Titlebar;
using GameBoost.MVVM.ViewModels.Shared.Selection;
using GameBoost.Shared.Helpers;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GameBoost.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly AsyncRelayCommand _dockActionCommand;

        private readonly HomeViewModel _homeViewModel;
        private readonly GlobalOperationService _globalOperations;
        private readonly TitleBarActionService _titleBarActions;
        private readonly StartupNotificationService _startupNotifications;
        private readonly SelectionExecutionRequirementService _selectionRequirements;

        private SelectionViewModel? _activeSelectionViewModel;

        public ObservableCollection<DockItem> Pages { get; }
        public ObservableCollection<TitleBarActionViewModel> TitleBarActions => _titleBarActions.Actions;

        public GlobalOperationService GlobalOperations => _globalOperations;
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

        public MainViewModel()
        {
            _globalOperations = new GlobalOperationService();
            _titleBarActions = new TitleBarActionService(_globalOperations);
            _startupNotifications = new StartupNotificationService(_titleBarActions);
            _selectionRequirements = new SelectionExecutionRequirementService(_titleBarActions);

            _dockActionCommand = new AsyncRelayCommand(
                ExecuteDockAction,
                CanExecuteDockAction);

            var windowsViewModel = new WindowsViewModel("Windows Optimisation");
            var systemViewModel = new SystemViewModel("System Optimisation");

            _homeViewModel = new HomeViewModel(
            [
                windowsViewModel,
                systemViewModel
            ]);

            Pages =
            [
                new DockItem("Home", PackIconKind.Home, _homeViewModel),
                new DockItem("Windows", PackIconKind.MicrosoftWindows, windowsViewModel),
                new DockItem("System", PackIconKind.Computer, systemViewModel),
            ];

            SelectedPage = Pages[0];
        }

        internal async Task InitialiseStartup()
        {
            _startupNotifications.AddStartupActions();

            await _homeViewModel.RefreshRecommendedActionAsync();
        }

        private void Navigate(DockItem page)
        {
            CurrentView = page.ViewModel;
            PageTitle = page.Title;
            PageIcon = page.Icon;

            UpdateDockState();
        }

        private void AttachSelectionViewModel(object? viewModel)
        {
            if (_activeSelectionViewModel is not null)
            {
                _activeSelectionViewModel.StateChanged -= OnSelectionStateChanged;
                _activeSelectionViewModel.ExecutionRequirementsDetected -= OnExecutionRequirementsDetected;
            }

            _activeSelectionViewModel = viewModel as SelectionViewModel;

            if (_activeSelectionViewModel is not null)
            {
                _activeSelectionViewModel.StateChanged += OnSelectionStateChanged;
                _activeSelectionViewModel.ExecutionRequirementsDetected += OnExecutionRequirementsDetected;
            }

            UpdateGlobalProgressState();
        }

        private void OnSelectionStateChanged()
        {
            UpdateGlobalProgressState();
            RefreshDockActionState();
        }

        private void OnExecutionRequirementsDetected(ExecutionRequirementsEventArgs args)
        {
            _selectionRequirements.HandleRequirements(args);
        }

        private void RefreshDockActionState()
        {
            OnPropertyChanged(nameof(DockActionText));
            OnPropertyChanged(nameof(DockActionIcon));
            OnPropertyChanged(nameof(IsDockActionEnabled));

            UpdateDockState();

            _dockActionCommand.RaiseCanExecuteChanged();
        }

        private void UpdateDockState()
        {
            GameBoostContext.Dock?.SetState(
                CanUseDockAction()
                    ? DockState.Full
                    : DockState.Compact);
        }

        private void UpdateGlobalProgressState()
        {
            var isSelectionExecutionActive =
                _activeSelectionViewModel?.DisplayScreenType == SelectionScreenType.Execution;

            _globalOperations.SetSelectionExecutionActive(isSelectionExecutionActive);
        }

        private bool CanExecuteDockAction()
        {
            return CanUseDockAction();
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