using GameBoost.Application;
using GameBoost.MVVM.Core;
using GameBoost.MVVM.ViewModels.Shared.Info;
using GameBoost.MVVM.ViewModels.Shared.Selection;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions;
using GameBoost.Shared.Helpers;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace GameBoost.MVVM.ViewModels
{
    public sealed class HomeViewModel
    {
        private readonly IReadOnlyList<SelectionViewModel> _selectionPages;

        public ObservableCollection<InfoCardViewModel> HardwareCards { get; } = [];
        public ObservableCollection<InfoCardViewModel> RecommendedActions { get; } = [];
        private ICommand OpenRecommendedActionCommand { get; }

        public HomeViewModel(IReadOnlyList<SelectionViewModel> selectionPages)
        {
            _selectionPages = selectionPages;

            OpenRecommendedActionCommand = new AsyncRelayCommand<InfoCardViewModel?>(OpenRecommendedAction);

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

                        action.IsChecked = true;

                        RecommendedActions.Add(new InfoCardViewModel
                        {
                            State = action.RecommendationPriority == RecommendationPriority.High ? InfoCardState.Error : InfoCardState.Warning,
                            Title = feature.Title,
                            Icon = action.Icon,
                            Info = action.Title,
                            ToolTip = action.RecommendationToolTip,
                            Footer = $"{action.Status} → {action.RecommendedValue?.ToString() ?? "Unknown"}",
                            Content = action,
                            Command = OpenRecommendedActionCommand
                        });
                    }
                }
            }

            SortRecommendedActions();
        }
        private async Task OpenRecommendedAction(InfoCardViewModel? card)
        {
            if (card is null || card.IsBusy)
                return;

            card.IsBusy = true;

            try
            {
                card.Footer = "Running...";
                card.State = InfoCardState.Info;

                if (card.Content is not SelectionActionCardViewModelBase action)
                    throw new InvalidOperationException();

                var result = await action.ExecuteSafeAsync(CancellationToken.None);

                if (!result.Success)
                    throw new Exception(result.Message);

                card.Footer = result.Message;
                card.State = InfoCardState.Success;

                await Task.Delay(2000);

                RecommendedActions.Remove(card);
            }
            catch (Exception ex)
            {
                card.Footer = ex.Message;
                card.State = InfoCardState.Error;
#if DEBUG
                Debug.WriteLine($"Error Executing {card.Info}: {ex.Message}");
#endif
            }
            finally
            {
                card.IsBusy = false;
            }

        }

        private void SortRecommendedActions()
        {
            var sorted = RecommendedActions
                .OrderByDescending(card =>
                    card.Content is SelectionActionCardViewModelBase action
                        ? action.RecommendationPriority
                        : RecommendationPriority.None)
                .ThenBy(card => card.Title)
                .ThenBy(card => card.Info)
                .ToList();

            RecommendedActions.Clear();

            foreach (var card in sorted)
                RecommendedActions.Add(card);
        }

        private void BuildHardwareCards()
        {
            var systemInfo = GameBoostContext.SystemInfo;

            HardwareCards.Clear();

            HardwareCards.Add(new InfoCardViewModel
            {
                Icon = PackIconKind.Cpu64Bit,
                Title = "Processor",
                Info = systemInfo?.CPU?.Name ?? "Unknown processor",
                Footer = systemInfo?.CPU?.CurrentClockSpeed ?? "Unknown clock speed"
            });

            HardwareCards.Add(new InfoCardViewModel
            {
                Icon = PackIconKind.Monitor,
                Title = "Graphics",
                Info = systemInfo?.GPU?.Name ?? "Unknown graphics adapter",
                Footer = systemInfo?.GPU?.AdapterRAM ?? "Unknown video memory"
            });

            HardwareCards.Add(new InfoCardViewModel
            {
                Icon = PackIconKind.Memory,
                Title = "Installed RAM",
                Info = MathHelper.FormatBytes(systemInfo?.Memory?.TotalPhysicalMemory),
                Footer = systemInfo?.Memory?.PhysicalMemoryUsageText ?? "Unknown usage"
            });

            HardwareCards.Add(new InfoCardViewModel
            {
                Icon = PackIconKind.Harddisk,
                Title = "Storage",
                Info = GetTotalStorageText(),
                Footer = GetStorageUsageText()
            });

            HardwareCards.Add(new InfoCardViewModel
            {
                Icon = PackIconKind.MicrosoftWindows,
                Title = "Windows",
                Info = systemInfo?.OS?.Name ?? "Unknown Windows version",
                Footer = $"Build {systemInfo?.OS?.BuildNumber ?? "Unknown"}"
            });

            HardwareCards.Add(new InfoCardViewModel
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
