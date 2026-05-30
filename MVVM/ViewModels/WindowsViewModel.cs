using GameBoost.Features.Modules.Windows.Gaming;
using GameBoost.Features.Modules.Windows.Taskbar;
using GameBoost.Features.Modules.Windows.VisualEffects;
using GameBoost.MVVM.ViewModels.Shared;
using GameBoost.MVVM.ViewModels.Shared.Selection;
using MaterialDesignThemes.Wpf;

namespace GameBoost.MVVM.ViewModels
{
    public class WindowsViewModel : SelectionViewModel
    {

        public WindowsViewModel(string pageTitle)
        {
            PageTitle = pageTitle;

            var diskCleanup = new SelectionFeatureViewModel
            {
                Title = "Disk Cleanup",
                Description = "Clean temporary data and optimise disk usage for improved responsiveness",
                Icon = PackIconKind.Broom,
            };
            diskCleanup.AddActions(
            [
                 new()
                 {
                     Title = "Clean Temporary Files",
                     Icon = PackIconKind.FolderRemove,
                     //Module = new TempFileCleanupModule(),
                 
                 },
                 new()
                 {
                     Title = "Recycle Bin Cleanup",
                     Icon = PackIconKind.TrashCanOutline,
                     //Module = new RecyclingBinCleanupModule(),
                 },
                 new()
                 {
                     Title = "Clear System Logs",
                     Icon = PackIconKind.FileDocumentRemove,
                     //Module = new LogFileCleanupModule(),
                 }
             ]);


            var gaming = new SelectionFeatureViewModel
            {
                Title = "Gaming",
                Description = "Optimise Windows 11 for gaming performance with Game Mode, VRR, DirectStorage, and controller optimizations",
                Icon = PackIconKind.Gamepad,
                IsChecked = true,
            };
            gaming.AddActions(
            [
                new()
                {
                    Title = "Game Mode",
                    Icon = PackIconKind.Controller,
                    Module = new GameModeModule(),
                    IsChecked = true,
                },
                new()
                {
                    Title = "Hardware Acceleration GPU Scheduling",
                    Icon = PackIconKind.WindowRestore,
                    IsChecked = true,
                    //Module = new HardwareAcceleratedGpuScheduling(),
                },
                new()
                {
                    Title = "Windowed Game Optimization",
                    Icon = PackIconKind.WindowRestore,
                    IsChecked = true,
                    //Module = new WindowedGameOptimizationModule(),
                },
                new()
                {
                    Title = "Variable Refresh Rate (VRR)",
                    Icon = PackIconKind.MonitorShimmer,
                    IsChecked = true,
                    //Module = new VariableRefreshRateModule(),
                },
            ]);

            var visualEffects = new SelectionFeatureViewModel
            {
                Title = "Visual Effects",
                Description = "Manage and customize system and application themes, including dark mode settings and other appearance options to enhance your user experience",
                Icon = PackIconKind.Theme,
                IsChecked = true,
            };
            visualEffects.AddActions(
            [
                new()
                {
                    Title = "Preference Options",
                    Icon = PackIconKind.VectorPolyline,
                    IsChecked = true,
                    //Module = new PreferenceOptionsModule(),
                },
                new()
                {
                    Title = "System Theme Mode",
                    Icon = PackIconKind.Computer,
                    IsChecked = true,
                    Module = new SystemThemeModeModule(),
                },
                new()
                {
                    Title = "Transparency Effects",
                    Icon = PackIconKind.VectorUnion,
                    IsChecked = true,
                    //Module = new TransparencyEffectModule(),
                },
            ]);

            var taskbar = new SelectionFeatureViewModel
            {
                Title = "Taskbar",
                Description = "Customize which features appear on your Windows taskbar, such as the search bar, widgets, and system buttons",
                Icon = PackIconKind.TableRow,
                IsChecked = true,
            };
            taskbar.AddActions(
            [
                new()
                {
                    Title = "Search Bar",
                    Icon = PackIconKind.Search,
                    //Module = new SearchboxTaskbarButton(),
                },
                new()
                {
                    Title = "Task View",
                    Icon = PackIconKind.ImageFilterNone,
                    //Module = new TaskViewTaskbarButton(),
                },
                new()
                {
                    Title = "Widgets",
                    Icon = PackIconKind.Widgets,
                    Module = new WidgetsTaskbarModule(),
                    IsChecked = true,
                },
                new()
                {
                    Title = "End Task",
                    Icon = PackIconKind.ContainEnd,
                    //Module = new EndTaskTaskbarOption(),
                }
            ]);

            FeatureCards =
            [
                diskCleanup,
                gaming,
                visualEffects,
                taskbar
            ];
        }
    }
}
