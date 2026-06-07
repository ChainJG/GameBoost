using GameBoost.Application;
using GameBoost.MVVM.ViewModels.Shared.Home;
using GameBoost.MVVM.ViewModels.Shared.Selection;
using GameBoost.Shared.Helpers;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.IO;

namespace GameBoost.MVVM.ViewModels
{
    public sealed class HomeViewModel
    {
        private readonly IReadOnlyList<SelectionViewModel> _selectionPages;

        public ObservableCollection<HomeInfoCardViewModel> HardwareCards { get; } = [];
        public ObservableCollection<HomeRecommendedActionViewModel> RecommendedActions { get; } = [];

        public HomeViewModel(IReadOnlyList<SelectionViewModel> selectionPages)
        {
            _selectionPages = selectionPages;

            BuildHardwareCards();
        }

        public async Task RefreshRecommendedActionAsync(CancellationToken token = default)
        {
            RecommendedActions.Clear();

            foreach (var page in _selectionPages)
            {
                foreach (var feature in page.FeatureCards)
                {
                    foreach (var action in feature.Actions)
                    {
                        await action.RefreshStatusSafeAsync(token);

                        if (!action.ShouldShowAsHomeRecommendation)
                            continue;

                        RecommendedActions.Add(new HomeRecommendedActionViewModel
                        {
                            FeatureTitle = feature.Title,
                            Action = action,
                        });
                    }
                }
            }

            SortRecommendedActions();


        }

        private void SortRecommendedActions()
        {
            var sorted = RecommendedActions
                .OrderByDescending(action => action.Priority)
                .ThenBy(action => action.FeatureTitle)
                .ThenBy(action => action.Title)
                .ToList();

            RecommendedActions.Clear();

            foreach (var action in sorted)
                RecommendedActions.Add(action);
        }

        private void BuildHardwareCards()
        {
            var systemInfo = GameBoostContext.SystemInfo;

            HardwareCards.Clear();

            HardwareCards.Add(new HomeInfoCardViewModel
            {
                Icon = PackIconKind.Cpu64Bit,
                Title = "Processor",
                Info = systemInfo?.CPU?.Name ?? "Unknown processor",
                Footer = systemInfo?.CPU?.CurrentClockSpeed ?? "Unknown clock speed"
            });

            HardwareCards.Add(new HomeInfoCardViewModel
            {
                Icon = PackIconKind.Monitor,
                Title = "Graphics",
                Info = systemInfo?.GPU?.Name ?? "Unknown graphics adapter",
                Footer = systemInfo?.GPU?.AdapterRAM ?? "Unknown video memory"
            });

            HardwareCards.Add(new HomeInfoCardViewModel
            {
                Icon = PackIconKind.Memory,
                Title = "Installed RAM",
                Info = MathHelper.FormatBytes(systemInfo?.Memory?.TotalPhysicalMemory),
                Footer = systemInfo?.Memory?.PhysicalMemoryUsageText ?? "Unknown usage"
            });

            HardwareCards.Add(new HomeInfoCardViewModel
            {
                Icon = PackIconKind.Harddisk,
                Title = "Storage",
                Info = GetTotalStorageText(),
                Footer = GetStorageUsageText()
            });

            HardwareCards.Add(new HomeInfoCardViewModel
            {
                Icon = PackIconKind.MicrosoftWindows,
                Title = "Windows",
                Info = systemInfo?.OS?.Name ?? "Unknown Windows version",
                Footer = $"Build {systemInfo?.OS?.BuildNumber ?? "Unknown"}"
            });

            HardwareCards.Add(new HomeInfoCardViewModel
            {
                Icon = PackIconKind.Chip,
                Title = "Motherboard",
                Info = systemInfo?.Motherboard?.MotherboardDisplayName ?? "Unknown motherboard",
                Footer = $"BIOS {systemInfo?.Motherboard?.BIOSVersion ?? "Unknown"}"
            });

        }

        #region Drive Stronge Methods
        private static string GetTotalStorageText()
        {
            var drives = GetFixedReadyDrives();

            var totalBytes = drives.Sum(drive => drive.TotalSize);

            return MathHelper.FormatBytes(totalBytes);
        }

        private static string GetStorageUsageText()
        {
            var drives = GetFixedReadyDrives();

            var totalBytes = drives.Sum(drive => drive.TotalSize);
            var freeBytes = drives.Sum(drive => drive.AvailableFreeSpace);
            var usedBytes = totalBytes - freeBytes;

            return $"{MathHelper.FormatBytes(usedBytes)} of {MathHelper.FormatBytes(totalBytes)} used";
        }

        private static IReadOnlyList<DriveInfo> GetFixedReadyDrives()
        {
            return DriveInfo.GetDrives()
                .Where(drive => drive.IsReady && drive.DriveType == DriveType.Fixed)
                .ToList();
        }
        #endregion
    }
}
