using GameBoost.Features.Modules.Base;
using GameBoost.Shared.Helpers.ProcessHelpers;
using GameBoost.Shared.Results;
using System.Diagnostics;

namespace GameBoost.Features.Modules.WindowsModules.Gaming.GameFocus
{
    public sealed class GamingFocusProcessModule : ActionModuleBase
    {
        public override string Name => "Game Focus";
        private const string ReadyStatusText = "Game Ready";
        private int? _gameFocusCount;

        #region IRecommandedModule
        public override RecommendationPriority RecommendationPriority => GetRecommendedPriority();
        public override object? RecommendedValue => ReadyStatusText;
        public override string RecommendationReason =>
            "Recommended before gaming because it closes optional background apps that may use CPU, memory, disk, network, or create notifications during gameplay";
        private RecommendationPriority GetRecommendedPriority()
        {
            _gameFocusCount ??= GetDetectedProcessGroups().Count;

            if (_gameFocusCount <= 0)
                return RecommendationPriority.None;

            if (_gameFocusCount <= 2)
                return RecommendationPriority.Low;

            return RecommendationPriority.Medium;
        }
        #endregion

        protected override string FormatStatus(ToggleType status) => status.ToString();
        public async override Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var detectedGroups = GetDetectedProcessGroups();
            _gameFocusCount = detectedGroups.Count;

            foreach (var definition in detectedGroups)
                Debug.WriteLine($"Game Focus Detected: {definition.DisplayName}");

            ActionCard?.InfoToolTip = String.Join(Environment.NewLine, detectedGroups.Select(d => $"{d.DisplayName} ({d.Reason})"));

            var statusText = _gameFocusCount == 0
                ? ReadyStatusText
                : $"{_gameFocusCount} Obstacle Detected";

                return ActionRefreshResult.Status(statusText);
        }



        public override async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var cleanupResult = await Task.Run(
                    () => RunGamingFocusCleanup(token),
                    token);

                return BuildModuleResult(cleanupResult);
            }
            catch (OperationCanceledException)
            {
                return ModuleResult.Failed("Gaming Focus was cancelled");
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Gaming Focus failed: {ex.Message}");
#endif

                return ModuleResult.Failed("Gaming Focus failed");
            }
        }


        private static ModuleResult BuildModuleResult(ProcessResult result)
        {
            if (result.ClosedCount == 0 &&
                result.KilledCount == 0 &&
                result.FailedCount == 0)
            {
                return ModuleResult.Successful("No optional background apps needed closing");
            }

            var message =
                $"Game Focus complete: " +
                $"{result.ClosedCount} closed, " +
                $"{result.KilledCount} killed, " +
                $"{result.SkippedCount} skipped";

            if (result.FailedCount > 0)
                message += $", {result.FailedCount} failed";

            return ModuleResult.Successful(message);
        }

        private static ProcessResult RunGamingFocusCleanup(CancellationToken token)
        {
            var result = new ProcessResult();
            var detectedGroups = GetDetectedProcessGroups();

            foreach (var definition in detectedGroups)
            {
                token.ThrowIfCancellationRequested();

                result.DetectedCount++;

                ApplyFocusAction(definition, result);

            }

            return result;
        }

        private static void ApplyFocusAction(
            GamingFocusProcessDefinition definition,
            ProcessResult result)
        {
            if (definition.TryGracefulClose &&
                TryCloseProcess(definition, result))
            {
                return;
            }

            if (definition.AllowForceKill &&
                TryKillProcess(definition, result))
            {
                return;
            }

            result.SkippedCount++;
        }

        private static bool TryCloseProcess(GamingFocusProcessDefinition definition, ProcessResult result)
        {
            Debug.WriteLine($"Try to close {definition.DisplayName}");

            var closeResult = ProcessHelper.TryCloseProcess(definition.ProcessName);

            if (!closeResult.Success)
                return false;

            result.ClosedCount++;
            return true;
        }

        private static bool TryKillProcess(GamingFocusProcessDefinition definition, ProcessResult result)
        {
            Debug.WriteLine($"Try to kill {definition.DisplayName}");

            var killResult = ProcessHelper.TryEndProcess(definition.ProcessName);

            if (!killResult.Success)
            {
                result.FailedCount++;
                return false;
            }

            result.KilledCount++;
            return true;
        }

        #region Detection

        private static IReadOnlyList<GamingFocusProcessDefinition> GetDetectedProcessGroups() =>
             [.. GamingFocusProcessCatalog.Processes
                .Where(definition => definition.EnabledByDefault)
                .Where(definition => IsProcessGroupRunning(definition))];

        private static bool IsProcessGroupRunning(GamingFocusProcessDefinition definition) =>
             Process
                .GetProcessesByName(definition.ProcessName)
                .Any(process =>
                {
                    using (process)
                    {
                        return IsDetectableProcess(
                            process,
                            definition);
                    }
                });

        private static bool IsDetectableProcess(Process process, GamingFocusProcessDefinition definition)
        {
            if (!ProcessHelper.CanTouchProcess(process))
                return false;

            // If force kill is allowed, the process can be detected even without a window
            if (definition.AllowForceKill)
                return true;

            // If force kill is NOT allowed, only detect it when it can be gracefully closed
            if (!definition.TryGracefulClose)
                return false;

            return ProcessHelper.HasMainWindow(process);
        }
        #endregion
    }
}
