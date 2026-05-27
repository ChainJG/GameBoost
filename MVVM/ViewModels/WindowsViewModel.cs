using GameBoost.Features.Modules.Windows.Gaming;
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

            FeatureCards =
            [
                 new SelectionFeatureViewModel
                 {
                    Title = "Disk Cleanup",
                    Description = "Clean temporary data and optimise disk usage for improved responsiveness",
                    Icon = PackIconKind.Broom,
                    Actions =
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
                    ]
                 },
                 new SelectionFeatureViewModel
                 {
                    Title = "Gaming",
                    Description = "Optimise Windows 11 for gaming performance with Game Mode, VRR, DirectStorage, and controller optimizations",
                    Icon = PackIconKind.Gamepad,
                    Actions =
                    [
                        new()
                        {
                            Title = "Game Mode",
                            Icon = PackIconKind.Controller,
                            Module = new GameModeModule(),
                        },
                        new()
                        {
                            Title = "Hardware Acceleration GPU Scheduling",
                            Icon = PackIconKind.WindowRestore,
                            //Module = new HardwareAcceleratedGpuScheduling(),
                        },
                        new()
                        {
                            Title = "Windowed Game Optimization",
                            Icon = PackIconKind.WindowRestore,
                            //Module = new WindowedGameOptimizationModule(),
                        },
                        new()
                        {
                            Title = "Variable Refresh Rate (VRR)",
                            Icon = PackIconKind.MonitorShimmer,
                            //Module = new VariableRefreshRateModule(),
                        },
                    ]
                 },
                 new SelectionFeatureViewModel
                 {
                    Title = "Visual Effects",
                    Description = "Manage and customize system and application themes, including dark mode settings and other appearance options to enhance your user experience",
                    Icon = PackIconKind.Theme,
                    Actions =
                    [
                        new()
                        {
                            Title = "Preference Options",
                            Icon = PackIconKind.VectorPolyline,
                            //Module = new PreferenceOptionsModule(),
                        },
                        new()
                        {
                            Title = "System Theme Mode",
                            Icon = PackIconKind.Computer,
                            Module = new SystemThemeModeModule(),
                        },
                        new()
                        {
                            Title = "Transparency Effects",
                            Icon = PackIconKind.VectorUnion,
                            //Module = new TransparencyEffectModule(),
                        }
                    ]
                 },
            ];
        }
    }
}
