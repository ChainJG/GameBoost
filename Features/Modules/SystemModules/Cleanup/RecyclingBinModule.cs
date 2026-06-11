using GameBoost.Core.Interfaces;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GameBoost.Features.Modules.SystemModules.Cleanup
{
    public sealed class RecyclingBinModule : IActionModule, IRecommendedActionModule, IRequiredModule
    {
        public string Name => "Recycling Bin";
        private long? CacheSize { get; set; }
        private string RecyclingBinSizeText => MathHelper.FormatBytes(CacheSize ?? 0);

        #region IRequireModule
        public bool SystemReboot => false;
        public bool Admin => false;
        #endregion

        #region IRecommendedActionModule
        private static readonly long PriorityHighThreshold = MathHelper.GigabytesToBytes(10);
        private static readonly long PriorityMediumThreshold = MathHelper.GigabytesToBytes(5);
        private static readonly long PriorityLowThreshold = MathHelper.GigabytesToBytes(2);

        public bool IsRecommendedValue(object? currentValue)
        {
            return false;
        }
        public RecommendationPriority RecommendationPriority => GetRecommendationPriority(CacheSize ?? 0);

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

        public object? RecommendedValue => "Delete";
        public string RecommendationReason => "Removes files stored in the Recycle Bin, which can free up storage space and permanently clear deleted items that are no longer needed";
        #endregion

        // Importing the SHQueryRecycleBin function from shell32.dll
        [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int SHQueryRecycleBin(string pszRootPath, ref SHQUERYRBINFO pSHQueryRBInfo);

        // Importing the SHEmptyRecycleBin function from shell32.dll
        [DllImport("shell32.dll", SetLastError = true)]
        private static extern long SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, uint dwFlags);

        // Constants for SHEmptyRecycleBin
        private const uint SHERB_NOCONFIRMATION = 0x00000001; // No confirmation dialog
        private const uint SHERB_NOPROGRESSUI = 0x00000002;   // No progress UI
        private const uint SHERB_NOSOUND = 0x00000004;        // No sound

        [StructLayout(LayoutKind.Sequential)]
        private struct SHQUERYRBINFO
        {
            public uint cbSize;
            public long i64Size;
            public long i64NumItems;
        }


        public async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            try
            {
                if (CacheSize == null || CacheSize == 0)
                    return ModuleResult.Successful("The Recycle Bin is empty");

                token.ThrowIfCancellationRequested();

                await EmptyRecyclingBin(token);

                return ModuleResult.Successful($"Successfully removed {RecyclingBinSizeText} from the Recycle Bin");
            }
            catch (Exception ex)
            {
                return ModuleResult.Failed($"Failed to empty the Recycle Bin: {ex.Message}");
            }
        }

        public async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            CacheSize = await CalculateRecyclingBinSize(token);

            return await Task.FromResult(ActionRefreshResult.Status(RecyclingBinSizeText));
        }

        private static async Task EmptyRecyclingBin(CancellationToken token) =>
            await Task.Run(() => SHEmptyRecycleBin(IntPtr.Zero, null, SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND), token);

        private static async Task<long> CalculateRecyclingBinSize(CancellationToken token)
        {
            try
            {
                SHQUERYRBINFO queryInfo = new()
                {
                    cbSize = (uint)Marshal.SizeOf<SHQUERYRBINFO>()
                };
                
                var result = await Task.Run(() => SHQueryRecycleBin(null, ref queryInfo), token);

                token.ThrowIfCancellationRequested();

                if (result != 0)
                {
                    var error = Marshal.GetLastWin32Error();
#if DEBUG
                    Debug.WriteLine($"SHQueryRecycleBin failed. Error Code: {error}");
#endif
                    return 0;
                }

                return queryInfo.i64Size;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Exception querying recycle bin: {ex.Message}");
#endif
                return 0;
            }
        }

        public Task<ModuleResult> ExecuteRecommendedAsync(CancellationToken token) => ExecuteAsync(token);
    }
}
