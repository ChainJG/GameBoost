using GameBoost.Application;
using GameBoost.Core.Debugger;
using GameBoost.Features.Modules.WindowsModules.Gaming.GameFocus;
using GameBoost.MVVM.Core;
using GameBoost.MVVM.ViewModels.Shared.Info;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Helpers.ProcessHelpers;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;
using System.IO;

namespace GameBoost.MVVM.ViewModels
{
    public sealed class HomeViewModel : ObservableObject
    {
        private readonly GameBoostUIServices _uiServices;
        private readonly GamingFocusProcessModule _gamingFocus = new();

        public int TotalRecommendedActions => RecommendedActions.Count + GameFocusActions.Count;

        public ObservableCollection<InfoCardViewModel> HardwareCards { get; } = [];
        public ObservableCollection<InfoCardViewModel> RecommendedActions { get; } = [];
        public ObservableCollection<InfoCardViewModel> GameFocusActions { get; } = [];

        public HomeViewModel(GameBoostUIServices uiServices)
        {
            _uiServices = uiServices;

            RecommendedActions.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalRecommendedActions));
            GameFocusActions.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalRecommendedActions));

            _uiServices.RecommendedActions.RecommendationsChanged += NotifyRefreshRecommendedActionCards;
            _uiServices.StartupCompleted += NotifyScanComplete;
        }

        #region Notify Methods
        private void NotifyScanComplete()
        {
            BuildHardwareCards();
            BuildGamingFocusCards();
        }

        public void NotifyRefreshRecommendedActionCards()
        {
            RecommendedActions.Clear();

            foreach (var action in _uiServices.RecommendedActions.RecommendedActions)
                RecommendedActions.Add(CreateRecommendedActionCard(action));

            SortRecommendedActions();
        }
        #endregion

        #region Gaming Focus Action Methods
        private async Task ExecuteGamingFocusActionAsync(InfoCardViewModel? card)
        {
            if (card is null || card.IsBusy)
                return;

            if (card.Content is not GamingFocusProcessDefinition definition)
                return;

            card.IsBusy = true;
            _uiServices.GlobalOperations.BeginOperation();

            try
            {
                card.BeginOperation();

                var result = await Task.Run(() => GamingFocusProcessModule.ApplyFocusAction(definition));

                if (!result.Success)
                    throw new Exception(result.Message);

                card.CompleteOperation(result.Message);

                // Wait for X seconds to displayed the success state
                await Task.Delay(2000);

                GameFocusActions.Remove(card);
            }
            catch (Exception ex)
            {
                card.FailedOperation(ex.Message);
                GameBoostDebug.Error($"Error Executing {definition.DisplayName}: {ex.Message}");
            }
            finally
            {
                card.IsBusy = false;

                _uiServices.GlobalOperations.EndOperation();
            }
        }
        #endregion
        #region Gaming Focus Helper Methods
        private void BuildGamingFocusCards()
        {
            GameFocusActions.Clear();
            var detectedGroups = _gamingFocus.GetDetectedProcessGroups();

            foreach (var definition in detectedGroups)
                GameFocusActions.Add(CreateGamingFocusAction(definition));
        }

        private InfoCardViewModel CreateGamingFocusAction(GamingFocusProcessDefinition definition) =>
            new()
            {
                State = GetGamingFocusState(definition),
                Title = $"Gaming Focus",
                Info = $"{definition.DisplayName} ({MathHelper.FormatBytes(ProcessHelper.GetTotalMemoryUsageForProcess(definition.ProcessName))})",
                Icon = PackIconKind.DragVariant,
                ToolTip = definition.Reason,
                Footer = $"→ {definition.Action}",
                Content = definition,
                Command = new AsyncRelayCommand<InfoCardViewModel?>(
                    ExecuteGamingFocusActionAsync,
                    card => card is not null && !card.IsBusy)
            };

        private static InfoCardState GetGamingFocusState(GamingFocusProcessDefinition definition)
        {
            long memoryUsageBytes = ProcessHelper.GetTotalMemoryUsageForProcess(definition.ProcessName);

            long priorityHighThreshold = MathHelper.GigabytesToBytes(1);
            long priorityMediumThreshold = MathHelper.MegabytesToBytes(600);

            return memoryUsageBytes switch
            {
                var bytes when bytes >= priorityHighThreshold => InfoCardState.Error,
                var bytes when bytes >= priorityMediumThreshold => InfoCardState.Warning,
                var bytes when bytes > 0 => InfoCardState.Notice,
                _ => InfoCardState.Notice
            };
        }
        #endregion

        #region Recommendation Info Cards Methods
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
                card.BeginOperation();

                var result = await action.ExecuteRecommendedAsync(CancellationToken.None);

                if (!result.Success)
                    throw new Exception(result.Message);

                // Set success state
                card.CompleteOperation(result.Message);

                // Wait for X seconds to displayed the success state
                await Task.Delay(2000);

                RecommendedActions.Remove(card);
            }
            catch (Exception ex)
            {
                card.FailedOperation(ex.Message);
                GameBoostDebug.Error($"Error Executing {action.Title}: {ex.Message}");
            }
            finally
            {
                card.IsBusy = false;

                _uiServices.GlobalOperations.EndOperation();
                _uiServices.SelectionRequirements.RegisterExecutedAction(action);
            }

        }
        #endregion
        #region Recommendations Helper Methods
        private InfoCardViewModel CreateRecommendedActionCard(SelectionActionCardViewModelBase action) =>
             new()
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


        private static InfoCardState GetRecommendationState(SelectionActionCardViewModelBase action) =>
            action.RecommendationPriority switch
            {
                RecommendationPriority.High => InfoCardState.Error,
                RecommendationPriority.Medium => InfoCardState.Warning,
                RecommendationPriority.Low => InfoCardState.Notice,
                _ => InfoCardState.Info
            };
        #endregion 

        #region Hardware Info Cards
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
        #endregion

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

        private static IReadOnlyList<DriveInfo> GetFixedReadyDrives() =>
             [.. DriveInfo.GetDrives().Where(drive => drive.IsReady && drive.DriveType == DriveType.Fixed)];
        #endregion
    }
}
