using GameBoost.Core.Dock;
using GameBoost.MVVM.Core;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;

namespace GameBoost.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public ObservableCollection<DockItem> Pages { get; }

        private object _currentView;
        public object CurrentView { get => _currentView; set => Set(ref _currentView, value); }

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
            Pages =
            [
                new DockItem("Home", PackIconKind.Home, new HomeViewModel()),
                new DockItem("Windows", PackIconKind.MicrosoftWindows, new WindowsViewModel()),
                new DockItem("System", PackIconKind.MicrosoftWindows, new WindowsViewModel()),
            ];

            SelectedPage = Pages[0];
        }

        private void Navigate(DockItem page)
        {
            CurrentView = page.ViewModel;
            PageTitle = page.Title;
            PageIcon = page.Icon;
        }
    }
}
