using GameBoost.Features.Modules.Base;
using GameBoost.Features.Modules.SystemModules.Cleanup.Options;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;
using System.Collections.ObjectModel;
using System.IO;

namespace GameBoost.Features.Modules.SystemModules.Cleanup
{
    public sealed class TempFileModule : ActionModuleBase
    {
        public override string Name => "Temporary Files";

        private CleanupScanResult? LastScan;

        private string TempDirectorySizeText
        {
            get
            {
                if (LastScan is null)
                    return MathHelper.FormatBytes(CacheSize ?? 0);

                if (LastScan.EstimatedDeletableBytes == 0)
                    return "Empty";

                return
                    $"{MathHelper.FormatBytes(LastScan.EstimatedDeletableBytes)} deletable • " +
                    $"{LastScan.SkippedFiles} skipped";
            }
        }

        private long? CacheSize;
        private static readonly long PriorityHighThreshold = MathHelper.GigabytesToBytes(6);
        private static readonly long PriorityMediumThreshold = MathHelper.GigabytesToBytes(4);
        private static readonly long PriorityLowThreshold = MathHelper.GigabytesToBytes(2);

        private static readonly IReadOnlyList<DirectoryInfo> TemporaryDirectories = new ReadOnlyCollection<DirectoryInfo>(
        [
            // Default Temp Directory
            new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp")),
            new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp")),
            
            // Windows Directory
            new(Path.Combine(Environment.GetEnvironmentVariable("windir"), @"ServiceProfiles\NetworkService\AppData\Local\Microsoft\Windows\DeliveryOptimization\Cache")),
            new(Path.Combine(Environment.GetEnvironmentVariable("windir"), @"ServiceProfiles\NetworkService\AppData\Local\Microsoft\Windows\DeliveryOptimization\Logs")),
            new(Path.Combine(Environment.GetEnvironmentVariable("windir"), @"System32\config\systemprofile\AppData\Local\Microsoft\Windows\INetCache")),
            new(Path.Combine(Environment.GetEnvironmentVariable("windir"), @"SoftwareDistribution\DataStore\Logs")),
            new(Path.Combine(Environment.GetEnvironmentVariable("windir"), @"SoftwareDistribution\SharedFileCache")),
            new(Path.Combine(Environment.GetEnvironmentVariable("windir"), @"SoftwareDistribution\Download")),
            new(Path.Combine(Environment.GetEnvironmentVariable("windir"), @"System32\catroot2")),
            new(Path.Combine(Environment.GetEnvironmentVariable("windir"), @"Logs")),
            
            // Google Chrome
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\Default\Service Worker\CacheStorage")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\Default\Service Worker\ScriptCache")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\Default\Service Worker\Database")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\Default\Cache\Cache_Data")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\Default\Code Cache\wasm")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\Default\Session Storage")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\Default\Code Cache\js")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\Default\GPUCache")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\Default\Cache")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\GrShaderCache")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\ShaderCache")),
            
            // Microsoft Edge
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Microsoft\Edge\Microsoft\Edge\User Data\GraphiteDawnCache")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Microsoft\Edge\User Data\Default\Session Storage")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Microsoft\Edge\User Data\Default\Extension State")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Microsoft\Edge\User Data\Default\Media Cache")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Microsoft\Edge\User Data\BrowserMetrics")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Microsoft\Edge\User Data\Default\Cache")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Microsoft\Edge\User Data\ShaderCache")),
            new(Path.Combine(Environment.GetEnvironmentVariable("ProgramData"), @"Microsoft\EdgeUpdate\Log")),
            
            // Spotify
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Spotify\GraphiteDawnCache")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Spotify\GrShaderCache")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Spotify\ShaderCache")),
            
            // Valorant
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"VALORANT\Saved\webcache")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"VALORANT\Saved\Logs")),
            
            // Battle Net
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Battle.net\BrowserCaches\common")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Battle.net\Logs")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Battle.net\Cache")),
            
            //Discord
            new(Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), @"discord\Code Cache")),
            new(Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), @"discord\GPUCache")),
            new(Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), @"discord\Cache")),
            
            // Steam
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Steam\htmlcache\Cache\Cache_Data")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Steam\htmlcache\Code Cache\js")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Steam\htmlcache\DawnCache")),
            new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Steam\htmlcache\GPUCache")),
        ]);

        #region IRecommendedActionModule
        public override RecommendationPriority RecommendationPriority => GetRecommendationPriority(CacheSize ?? 0);
        public override object? RecommendedValue => "Delete";
        public override string RecommendationReason => "Removes unused temporary system/app files from the PC, which can free up storage space, reduce clutter, and help keep Windows running cleaner";
        public override bool IsRecommendedValue(object? currentValue)
        {
            return false;
        }
        #endregion

        public override async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            LastScan = await CalculateTemporaryDirectoryScanAsync(token, accurate: false);

            CacheSize = LastScan.EstimatedDeletableBytes;

            return ActionRefreshResult.Status(TempDirectorySizeText);
        }

        public override Task<ModuleResult> ExecuteRecommendedAsync(CancellationToken token) => ExecuteAsync(token);
        public override async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            try
            {
                var deleteResult = await DirectoryCleanupHelper.DeleteDeletableFilesAsync(TemporaryDirectories, token, ignoreFilesNewerThan: TimeSpan.FromMinutes(10));

                LastScan = await CalculateTemporaryDirectoryScanAsync(token, accurate: false);

                CacheSize = LastScan.EstimatedDeletableBytes;

                var message =
                    $"Deleted {MathHelper.FormatBytes(deleteResult.DeletedBytes)} of temporary files";

                if (deleteResult.FailedFiles > 0)
                    message += $" • Skipped {deleteResult.FailedFiles} files";

                return ModuleResult.Successful(message);
            }
            catch (OperationCanceledException)
            {
                return ModuleResult.Failed("Temporary file cleanup was cancelled");
            }
            catch
            {
                return ModuleResult.Failed("Failed to delete temporary files");
            }
        }

        private static Task<CleanupScanResult> CalculateTemporaryDirectoryScanAsync(CancellationToken token, bool accurate)
        {
            return DirectoryCleanupHelper.ScanDeletableFilesAsync(
                TemporaryDirectories,
                    new CleanupScanOptions
                    {
                        ProbeDeleteAccess = accurate,
                        IgnoreFilesNewerThan = TimeSpan.FromMinutes(10),
                        MaxDegreeOfParallelism = accurate ? 4 : 2
                    },
                token);
        }

        private static RecommendationPriority GetRecommendationPriority(long cacheSize)
        {
            if (cacheSize >= PriorityHighThreshold)
                return RecommendationPriority.High;

            if (cacheSize >= PriorityMediumThreshold)
                return RecommendationPriority.Medium;

            if (cacheSize >= PriorityLowThreshold)
                return RecommendationPriority.Low;

            return RecommendationPriority.None;
        }

        protected override string FormatStatus(ToggleType status) => TempDirectorySizeText;
    }
}
