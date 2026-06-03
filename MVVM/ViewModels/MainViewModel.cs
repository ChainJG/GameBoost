using GameBoost.Application;
using GameBoost.Core;
using GameBoost.Core.Dock;
using GameBoost.Features.Updates;
using GameBoost.MVVM.Core;
using GameBoost.MVVM.UserControls.Shared.TitlebarAction;
using GameBoost.MVVM.ViewModels.Shared.Selection;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions.Misc;
using GameBoost.Shared.Results;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GameBoost.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly AsyncRelayCommand _dockActionCommand;

        private SelectionViewModel? _activeSelectionViewModel;

        #region Titlebar Action ObservableCollection
        public ObservableCollection<TitleBarActionViewModel> TitleBarActions { get; } = [];

        private void AddOrReplaceTitleBarAction(TitleBarActionViewModel action)
        {
            var existingAction = TitleBarActions
                .FirstOrDefault(item => item.Key == action.Key);

            if (existingAction is not null)
                TitleBarActions.Remove(existingAction);

            TitleBarActions.Add(action);
        }
        private void RemoveTitleBarAction(string key)
        {
            var existingAction = TitleBarActions
                .FirstOrDefault(item => item.Key == key);

            if (existingAction is not null)
                TitleBarActions.Remove(existingAction);
        }
        #endregion

        #region Global Progress
        private int _globalOperationCount;

        public bool IsGlobalProgressVisible =>
            _globalOperationCount > 0 ||
            _activeSelectionViewModel?.DisplayScreenType == SelectionScreenType.Execution;

        private void BeginGlobalOperation()
        {
            _globalOperationCount++;

            OnPropertyChanged(nameof(IsGlobalProgressVisible));
        }

        private void EndGlobalOperation()
        {
            if (_globalOperationCount > 0)
                _globalOperationCount--;

            OnPropertyChanged(nameof(IsGlobalProgressVisible));
        }
        #endregion

        public ObservableCollection<DockItem> Pages { get; }

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

        #region Page Title and Icon
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
        #endregion

        #region Dock Action Button UI (Text, Icon, Enabled)
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

        public bool IsDockActionEnabled =>
            _activeSelectionViewModel?.DisplayScreenType switch
            {
                SelectionScreenType.Selection => _activeSelectionViewModel.HasRunnableSelection,
                SelectionScreenType.Execution => true,
                SelectionScreenType.Result => true,
                _ => false
            };
        #endregion

        public MainViewModel()
        {
            _dockActionCommand = new AsyncRelayCommand(
                ExecuteDockAction,
                CanExecuteDockAction);

            Pages =
            [
                new DockItem("Home", PackIconKind.Home, new HomeViewModel()),
                new DockItem("Windows", PackIconKind.MicrosoftWindows, new WindowsViewModel("Windows Optimistion")),
                new DockItem("System", PackIconKind.Computer, new SystemViewModel("System Optimistion")),
            ];

            SelectedPage = Pages[0];
        }

        private void Navigate(DockItem page)
        {
            CurrentView = page.ViewModel;
            PageTitle = page.Title;
            PageIcon = page.Icon;

            GameBoostContext.Dock?.SetState(CanExecuteDockAction() ? DockState.Full : DockState.Compact);
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
        }

        private void OnSelectionStateChanged() => RefreshDockActionState();

        #region Dock Action Button
        private void RefreshDockActionState()
        {
            OnPropertyChanged(nameof(IsGlobalProgressVisible));

            OnPropertyChanged(nameof(DockActionText));
            OnPropertyChanged(nameof(DockActionIcon));
            OnPropertyChanged(nameof(IsDockActionEnabled));

            GameBoostContext.Dock?.SetState(CanExecuteDockAction() ? DockState.Full : DockState.Compact);

            _dockActionCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteDockAction()
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
        #endregion

        #region Title Bar Actions Admin and Restart
        private void OnExecutionRequirementsDetected(ExecutionRequirementsEventArgs args)
        {
            if (args.RequiresAdmin)
                AddAdminRequiredAction(args.AdminRequiredActions);

            if (args.RequiresRestart)
                AddRestartRequiredAction(args.RestartRequiredActions);
        }

        private void AddAdminRequiredAction(IReadOnlyList<string> actionTitles)
        {
            var message = actionTitles.Count == 0
                ? "Some optimisations require administrator permission."
                : "Some optimisations require administrator permission:\n\n" +
                  string.Join(Environment.NewLine, actionTitles.Select(title => $"• {title}"));

            TitleBarActionViewModel? action = null;
            action = new TitleBarActionViewModel
            {
                Key = "AdminRequired",
                Title = "Administrator required",
                Message = message,
                Icon = PackIconKind.ShieldAlert,
                Foreground = System.Windows.Application.Current?.TryFindResource("WarningColour") as Brush ?? Brushes.LightGoldenrodYellow,
                Command = new AsyncRelayCommand(async () =>
                {
                    if (action is null)
                        return;

                    await RunTitleBarActionAsync(
                        action,
                        async () => await GameBoostServices.ShowRestartAdministratorDialog(message));
                })
            };

            AddOrReplaceTitleBarAction(action);
        }
        private void AddRestartRequiredAction(IReadOnlyList<string> actionTitles)
        {
            var message = actionTitles.Count == 0
                ? "Some optimisations require a restart before they fully take effect."
                : "Some optimisations require a restart before they fully take effect:\n\n" +
                    string.Join(Environment.NewLine, actionTitles.Select(title => $"• {title}"));

            TitleBarActionViewModel? action = null;

            action = new TitleBarActionViewModel
            {
                Key = "RestartRequired",
                Title = "Restart required",
                Message = message,
                Icon = PackIconKind.AlertCircle,
                Foreground = System.Windows.Application.Current?.TryFindResource("WarningColour") as Brush ?? Brushes.LightGoldenrodYellow,
                Command = new AsyncRelayCommand(async () =>
                {
                    if (action is null)
                        return;

                    await RunTitleBarActionAsync(
                        action,
                        async () => await GameBoostServices.ShowRestartDialog(message));
                })
            };

            AddOrReplaceTitleBarAction(action);
        }
        #endregion

        #region Title Bar Actions Update and Restore Point
        internal void InitialiseStartupTitleBarActions()
        {
            AddUpdateAvailableAction(GameBoostContext.UpdateInfo);
            AddRestorePointAction();
        }

        private void AddUpdateAvailableAction(UpdateReleaseInfo updateInfo)
        {
            if (updateInfo is null || !updateInfo.IsUpdateAvailable)
                return;

            TitleBarActionViewModel? action = null;

            action = new TitleBarActionViewModel
            {
                Key = "UpdateAvailable",
                Title = "Update available",
                Message = $"A new update is available: {updateInfo.Version}\n\n{updateInfo.Notes}",
                Icon = PackIconKind.ArrowDownCircle,
                Foreground =
                    System.Windows.Application.Current?.TryFindResource("WarningColour") as Brush
                    ?? Brushes.LightGoldenrodYellow,

                Command = new AsyncRelayCommand(async () =>
                {
                    if (action is null)
                        return;

                    await RunTitleBarActionAsync(
                        action,
                        async () => await GameBoostServices.ShowUpdateDialog(updateInfo));
                })
            };

            AddOrReplaceTitleBarAction(action);
        }
        private void AddRestorePointAction()
        {
            if (GameBoostContext.HasActiveRestorePoint)
                return;

            TitleBarActionViewModel? action = null;

            action = new TitleBarActionViewModel
            {
                Key = "RestorePoint",
                Title = "Restore point recommended",
                Message = "Your system does not have an active GameBoost restore point.",
                Icon = PackIconKind.BackupRestore,
                Foreground =
                    System.Windows.Application.Current?.TryFindResource("WarningColour") as Brush
                    ?? Brushes.LightGoldenrodYellow,

                Command = new AsyncRelayCommand(async () =>
                {
                    if (action is null)
                        return;

                    await RunTitleBarActionAsync(
                        action,
                        async () => await GameBoostServices.ShowRestorePointDialog());
                })
            };

            AddOrReplaceTitleBarAction(action);
        }

        private async Task RunTitleBarActionAsync(
            TitleBarActionViewModel action,
            Func<Task<ModuleResult>> operation)
        {
            if (action.IsBusy)
                return;

            try
            {
                action.IsBusy = true;
                BeginGlobalOperation();

                var result = await operation();

                if (result.Success)
                    RemoveTitleBarAction(action.Key);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Title bar action '{action.Key}' failed: {ex.Message}");
#endif

                MessageBox.Show(
                    ex.Message,
                    "GameBoost Action Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            finally
            {
                EndGlobalOperation();
                action.IsBusy = false;
            }
        }
        #endregion
    }
}