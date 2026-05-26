using GameBoost.MVVM.UserControls.Models;
using GameBoost.MVVM.ViewModels.Shared;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoost.MVVM.ViewModels
{
    public class WindowsViewModel : SelectionViewModel
    {
        public WindowsViewModel()
        {
            PageTitle = "Windows Optimistion";

            FeatureCards =
            [
                 new SelectionFeatureCard
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
                 }
            ];
        }
    }
}
