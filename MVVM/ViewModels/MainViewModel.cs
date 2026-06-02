using GameBoost.Application;
using GameBoost.Core.Dock;
using GameBoost.MVVM.Core;
using GameBoost.MVVM.ViewModels.Shared.Selection;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace GameBoost.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly AsyncRelayCommand _dockActionCommand;

        private SelectionViewModel? _activeSelectionViewModel;

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

        public bool IsDockActionEnabled =>
            _activeSelectionViewModel?.DisplayScreenType switch
            {
                SelectionScreenType.Selection => _activeSelectionViewModel.HasRunnableSelection,
                SelectionScreenType.Execution => true,
                SelectionScreenType.Result => true,
                _ => false
            };

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
                _activeSelectionViewModel.RunnableSelectionChanged -= OnRunnableSelectionChanged;
            }

            _activeSelectionViewModel = viewModel as SelectionViewModel;

            if (_activeSelectionViewModel is not null)
            {
                _activeSelectionViewModel.StateChanged += OnSelectionStateChanged;
                _activeSelectionViewModel.RunnableSelectionChanged += OnRunnableSelectionChanged;
            }
        }

        private void OnRunnableSelectionChanged()
        {
            RefreshDockActionState();
        }

        private void OnSelectionStateChanged(SelectionScreenType screenType) => RefreshDockActionState();

        private void RefreshDockActionState()
        {
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
    }
}