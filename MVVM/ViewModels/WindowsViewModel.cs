using GameBoost.MVVM.ViewModels.Shared;
using GameBoost.MVVM.ViewModels.Shared.Selection;
using MaterialDesignThemes.Wpf;

namespace GameBoost.MVVM.ViewModels
{
    public class WindowsViewModel : SelectionViewModel
    {
        public WindowsViewModel()
        {
            PageTitle = "Windows Optimistion";

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
                            //Module = new GameModeModule(),
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
            ];
        }
    }
}
