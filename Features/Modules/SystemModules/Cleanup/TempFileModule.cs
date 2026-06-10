using GameBoost.Core.Interfaces;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace GameBoost.Features.Modules.SystemModules.Cleanup
{
    internal class TempFileModule : IActionModule, IRecommendedActionModule, IRequireModule
    {
        private static readonly long PriorityHighThreshold = MathHelper.GigabytesToBytes(6);
        private static readonly long PriorityMediumThreshold = MathHelper.GigabytesToBytes(4);
        private static readonly long PriorityLowThreshold = MathHelper.GigabytesToBytes(2);

        private static readonly IReadOnlyList<DirectoryInfo> TemporaryDirectories = new ReadOnlyCollection<DirectoryInfo>(
            new List<DirectoryInfo>
            {
                // Default Temp Directory
                new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp")),
                new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp")),
            
                // Windows Directory
                new(Path.Combine(Environment.GetEnvironmentVariable("windir"), @"Logs")),
                new(Path.Combine(Environment.GetEnvironmentVariable("windir"), @"SoftwareDistribution\Download")),
                new(Path.Combine(Environment.GetEnvironmentVariable("windir"), @"SoftwareDistribution\DataStore\Logs")),
                new(Path.Combine(Environment.GetEnvironmentVariable("windir"), @"SoftwareDistribution\SharedFileCache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("windir"), @"System32\catroot2")),
                new(Path.Combine(Environment.GetEnvironmentVariable("windir"), @"System32\config\systemprofile\AppData\Local\Microsoft\Windows\INetCache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("windir"), @"ServiceProfiles\NetworkService\AppData\Local\Microsoft\Windows\DeliveryOptimization\Cache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("windir"), @"ServiceProfiles\NetworkService\AppData\Local\Microsoft\Windows\DeliveryOptimization\Logs")),
            
                // Google Chrome
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\Default\Cache\Cache_Data")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\Default\Code Cache\wasm")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\Default\Code Cache\js")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\Default\Cache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\ShaderCache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\GrShaderCache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\Default\GPUCache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\Default\Service Worker\CacheStorage")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\Default\Service Worker\ScriptCache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\Default\Service Worker\Database")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Google\Chrome\User Data\Default\Session Storage")),
            
                // Microsoft Edge
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Microsoft\Edge\User Data\Default\Cache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Microsoft\Edge\User Data\Default\Media Cache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Microsoft\Edge\User Data\BrowserMetrics")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Microsoft\Edge\User Data\Default\Session Storage")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Microsoft\Edge\User Data\Default\Extension State")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Microsoft\Edge\User Data\Default\Extension State")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Microsoft\Edge\User Data\ShaderCache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Microsoft\Edge\Microsoft\Edge\User Data\GraphiteDawnCache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Microsoft\Edge\User Data\Default\Extension State")),
                new(Path.Combine(Environment.GetEnvironmentVariable("ProgramData"), @"Microsoft\EdgeUpdate\Log")),
            
                // OneDrive
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"OneDrive\cache\qmlcache")),
                
                // Spotify
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Spotify\ShaderCache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Spotify\GrShaderCache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Spotify\GraphiteDawnCache")),
                
                // Valorant
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"VALORANT\Saved\webcache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"VALORANT\Saved\Logs")),
                
                // Battle Net
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Battle.net\Logs")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Battle.net\Cache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Battle.net\BrowserCaches\common")),
                
                //Discord
                new(Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), @"discord\Cache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), @"discord\Code Cache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), @"discord\GPUCache")),
            
                // Steam
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Steam\htmlcache\Cache\Cache_Data")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Steam\htmlcache\Code Cache\js")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Steam\htmlcache\DawnCache")),
                new(Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), @"Steam\htmlcache\GPUCache")),
        });

        public string Name => "Temporary Files";

        private long? CacheSize;

        private string TempDirectorySizeText => MathHelper.FormatBytes(CacheSize ?? 0);

        #region IRecommendedActionModule
        public RecommendationPriority RecommendationPriority => GetRecommendationPriority(CacheSize ?? 0);
        public object? RecommendedValue => "Delete";
        public string RecommendationReason => "Removes unused temporary system/app files from the PC, which can free up storage space, reduce clutter, and help keep Windows running cleaner";
        public bool IsRecommendedValue(object? currentValue)
        {
            return false;
        }
        #endregion

        #region IRequireModule
        public bool SystemReboot => false;
        public bool Admin => false;
        #endregion

        public async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            CacheSize = CalculateTemporaryDirectorySize();

            return await Task.FromResult(
                ActionRefreshResult.Status(TempDirectorySizeText));
        }

        public async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            try
            {
                long previousCacheSize = CacheSize ?? await Task.Run(CalculateTemporaryDirectorySize);

                foreach (var directory in TemporaryDirectories)
                {
                    try
                    {
                        token.ThrowIfCancellationRequested();

                        await Task.Run(() => DirectoryHelper.DeleteDirectory(directory, token), token);
                    }
                    catch (OperationCanceledException)
                    {
                        // ignored
                    }
                    catch(Exception ex)
                    {
#if DEBUG
                        Debug.WriteLine($"Failed to delete temporary directory: {directory}. Reason: {ex.Message}");
#endif
                    }
                }

                CacheSize = await Task.Run(CalculateTemporaryDirectorySize);
                long freeedSpace = previousCacheSize - (CacheSize ?? 0);

                return ModuleResult.Successful($"Deleted {MathHelper.FormatBytes(freeedSpace)} of temporary files");
            }
            catch
            {
                return ModuleResult.Failed("Failed to delete temporary files");
            }
        }

        private static long CalculateTemporaryDirectorySize() =>
            TemporaryDirectories.Sum(directory =>
                DirectoryHelper.GetDirectorySize(directory));

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

        public Task<ModuleResult> ExecuteRecommendedAsync(CancellationToken token) => ExecuteAsync(token);
    }
}
