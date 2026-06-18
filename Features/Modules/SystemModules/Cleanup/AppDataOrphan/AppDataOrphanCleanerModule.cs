using GameBoost.Features.Modules.Base;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;

namespace GameBoost.Features.Modules.SystemModules.Cleanup.AppDataOrphan
{
    public sealed class AppDataOrphanCleanerModule : ActionModuleBase
    {
        private readonly AppDataOrphanCleanerService _service = new();
        private DirectoryScanResult? _lastScan;

        private static readonly long HighThreshold = MathHelper.GigabytesToBytes(2);
        private static readonly long MediumThreshold = MathHelper.GigabytesToBytes(1);
        private static readonly long LowThreshold = MathHelper.MegabytesToBytes(250);

        public override string Name => "AppData Orphan Cleaner";

        #region IRecommendedModule
        public override object? RecommendedValue => "Clean";
        public override string RecommendationReason =>
            "Removes old AppData folders left behind by uninstalled apps or games, helping reduce storage clutter while avoiding active, protected, or recently used folders.";
        public override RecommendationPriority RecommendationPriority =>
            GetRecommendationPriority(_lastScan?.TotalBytes ?? 0);

        public override bool IsRecommendedValue(object? currentValue) =>
            (_lastScan?.CandidateCount ?? 0) == 0;

        private static RecommendationPriority GetRecommendationPriority(long bytes)
        {
            if (bytes >= HighThreshold)
                return RecommendationPriority.High;

            if (bytes >= MediumThreshold)
                return RecommendationPriority.Medium;

            if (bytes >= LowThreshold)
                return RecommendationPriority.Low;

            return RecommendationPriority.None;
        }
        #endregion


        protected override string FormatStatus(ToggleType status) => status.ToString();
        public override async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            _lastScan = await _service.ScanAsync(token);

            if (_lastScan.CandidateCount == 0)
                return ActionRefreshResult.Status($"Clean");

            var statusText =
                $"{MathHelper.FormatBytes(_lastScan.TotalBytes)} removable • " +
                $"{_lastScan.CandidateCount} folders";

            return ActionRefreshResult.Status(statusText);
        }


        public async override Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            try
            {
                var scan = _lastScan ?? await _service.ScanAsync(token);

                if (scan.CandidateCount == 0)
                    return ModuleResult.Successful("No orphaned AppData folders found");

                var deleteResult = await _service.DeleteAsync(scan.Candidates, token);

                _lastScan = await _service.ScanAsync(token);

                var message =
                    $"Moved {deleteResult.DeletedDirectories} orphaned AppData folders to Recycle Bin";

                if (deleteResult.DeletedBytes > 0)
                    message += $" • {MathHelper.FormatBytes(deleteResult.DeletedBytes)}";

                if (deleteResult.FailedFiles > 0)
                    message += $" • {deleteResult.FaildedDirectories} failed";

                if (deleteResult.SkippedDirectories > 0)
                    message += $" • {deleteResult.SkippedDirectories} skipped";

                return ModuleResult.Successful(message);
            }
            catch (OperationCanceledException)
            {
                return ModuleResult.Failed("AppData orphan cleanup was cancelled");
            }
            catch
            {
                return ModuleResult.Failed("Failed to clean AppData orphan folders");
            }
        }

    }
}
