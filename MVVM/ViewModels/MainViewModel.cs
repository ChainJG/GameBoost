using GameBoost.Application;
using GameBoost.Core.Dock;
using GameBoost.MVVM.Core;
using GameBoost.MVVM.ViewModels.Shared;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GameBoost.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public ObservableCollection<DockItem> Pages { get; }

        private readonly AsyncRelayCommand _applyCommand;
        public ICommand ApplyCommand => _applyCommand;

        private object? _currentView;
        public object? CurrentView
        {
            get => _currentView;
            set
            {
                if (!Set(ref _currentView, value))
                    return;

                _applyCommand.RaiseCanExecuteChanged();
            }
        }

        private DockItem _selectedPage;
        public DockItem SelectedPage 
        {
            get => _selectedPage; 
            set
            {
                Set(ref _selectedPage, value);
                Navigate(value);
            }
        }

        private string _pageTitle;
        public string PageTitle { get => _pageTitle; set => Set(ref _pageTitle, value); }

        private PackIconKind _pageIcon;
        public PackIconKind PageIcon { get => _pageIcon; set => Set(ref _pageIcon, value); }

        public MainViewModel()
        {
            _applyCommand = new AsyncRelayCommand(
                ApplyCurrentPageAsync,
                CanApplyCurrentPage);

            Pages =
            [
                new DockItem("Home", PackIconKind.Home, new HomeViewModel()),
                new DockItem("Windows", PackIconKind.MicrosoftWindows, new WindowsViewModel("Windows Optimistion")),
                new DockItem("System", PackIconKind.Computer, new SystemViewModel("System Optimistion")),

            ];

            Navigate(Pages[0]);
        }

        private void Navigate(DockItem page)
        {
            CurrentView = page.ViewModel;
            PageTitle = page.Title;
            PageIcon = page.Icon;

            //GameBoostContext.Dock?.SetState(page.ViewModel is SelectionViewModel ? DockState.Full : DockState.Compact);
        }

        private bool CanApplyCurrentPage() => CurrentView is SelectionViewModel;
        private async Task ApplyCurrentPageAsync()
        {
            if (CurrentView is not SelectionViewModel selectionViewModel)
                return;

            await selectionViewModel.ExecuteSelectedActionsAsync();
        }
    }
}
