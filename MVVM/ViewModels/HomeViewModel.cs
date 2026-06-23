using GameBoost.Application;
using GameBoost.Core.EventArguments;
using GameBoost.MVVM.Core;
using GameBoost.MVVM.ViewModels.Shared.Info;
using GameBoost.MVVM.ViewModels.Shared.Selection;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions;
using GameBoost.Shared.Helpers;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace GameBoost.MVVM.ViewModels
{
    public sealed class HomeViewModel
    {
        private readonly GameBoostUIServices _uiServices;

        public ObservableCollection<InfoCardViewModel> HardwareCards { get; } = [];
        public ObservableCollection<InfoCardViewModel> RecommendedActions { get; } = [];

        public HomeViewModel(GameBoostUIServices uiServices)
        {
            _uiServices = uiServices;

            _uiServices.RecommendedActions.RecommendationsChanged += RefreshRecommendedActionCards;
            _uiServices.StartupCompleted += NotifyScanComplete;
        }

        private void NotifyScanComplete()
        {
            BuildHardwareCards();
        }

        public void RefreshRecommendedActionCards()
        {
            RecommendedActions.Clear();

            foreach (var action in _uiServices.RecommendedActions.RecommendedActions)
                RecommendedActions.Add(CreateRecommendedActionCard(action));

            SortRecommendedActions();
        }


        #region recommendations Helper Methods
        private InfoCardViewModel CreateRecommendedActionCard(
            SelectionActionCardViewModelBase action)
        {
            return new InfoCardViewModel
            {
                State = GetRecommendationState(action),
                Title = action.Parent?.Title ?? "Unknown",
                Icon = action.Icon,
                Info = action.Title,
                ToolTip = action.RecommendationToolTip,
                Footer = $"{action.Status} → {action.RecommendedValue?.ToString() ?? "Unknown"}",
                Content = action,
                Command = new AsyncRelayCommand<InfoCardViewModel?>(
                    ExecuteActionCommandAsync,
                    card => card is not null && !card.IsBusy)
            };
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

        private async Task ExecuteActionCommandAsync(InfoCardViewModel? card)
        {
            if (card is null || card.IsBusy)
                return;

            if (card.Content is not SelectionActionCardViewModelBase action)
                return;

            card.IsBusy = true;
            _uiServices.GlobalOperations.BeginOperation();

            try
            {
                card.Footer = "Running...";
                card.State = InfoCardState.Running;

                var result = await action.ExecuteRecommendedAsync(CancellationToken.None);

                if (!result.Success)
                    throw new Exception(result.Message);


                card.Footer = result.Status.ToString();
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

                _uiServices.GlobalOperations.EndOperation();
                _uiServices.SelectionRequirements.RegisterExecutedAction(action);
            }

        }

        private static InfoCardState GetRecommendationState(SelectionActionCardViewModelBase action) =>
            action.RecommendationPriority switch
            {
                RecommendationPriority.High => InfoCardState.Error,
                RecommendationPriority.Medium => InfoCardState.Warning,
                RecommendationPriority.Low => InfoCardState.Notice,
                _ => InfoCardState.Info
            };
        #endregion

        private void BuildHardwareCards()
        {
            var systemInfo = GameBoostContext.SystemInfo;

            HardwareCards.Clear();

            HardwareCards.Add(new InfoCardViewModel
            {
                State = InfoCardState.Display,
                Icon = PackIconKind.Cpu64Bit,
                Title = "Processor",
                Info = systemInfo?.CPU?.Name ?? "Unknown processor",
                Footer = systemInfo?.CPU?.CurrentClockSpeed ?? "Unknown clock speed"
            });

            HardwareCards.Add(new InfoCardViewModel
            {
                State = InfoCardState.Display,
                Icon = PackIconKind.Monitor,
                Title = "Graphics",
                Info = systemInfo?.GPU?.Name ?? "Unknown graphics adapter",
                Footer = systemInfo?.GPU?.AdapterRAM ?? "Unknown video memory"
            });

            HardwareCards.Add(new InfoCardViewModel
            {
                State = InfoCardState.Display,
                Icon = PackIconKind.Memory,
                Title = "Installed RAM",
                Info = MathHelper.FormatBytes(systemInfo?.Memory?.TotalPhysicalMemory),
                Footer = systemInfo?.Memory?.PhysicalMemoryUsageText ?? "Unknown usage"
            });

            HardwareCards.Add(new InfoCardViewModel
            {
                State = InfoCardState.Display,
                Icon = PackIconKind.Harddisk,
                Title = "Storage",
                Info = GetTotalStorageText(),
                Footer = GetStorageUsageText()
            });

            HardwareCards.Add(new InfoCardViewModel
            {
                State = InfoCardState.Display,
                Icon = PackIconKind.MicrosoftWindows,
                Title = "Windows",
                Info = systemInfo?.OS?.Name ?? "Unknown Windows version",
                Footer = $"Build {systemInfo?.OS?.BuildNumber ?? "Unknown"}"
            });

            HardwareCards.Add(new InfoCardViewModel
            {
                State = InfoCardState.Display,
                Icon = PackIconKind.Chip,
                Title = "Motherboard",
                Info = systemInfo?.Motherboard?.MotherboardDisplayName ?? "Unknown motherboard",
                Footer = $"BIOS {systemInfo?.Motherboard?.BIOSVersion ?? "Unknown"}"
            });

        }

        #region Drive Storage Methods
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
