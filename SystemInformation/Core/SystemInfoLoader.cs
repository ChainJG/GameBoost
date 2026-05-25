using GameBoost.Core.Interfaces;
using GameBoost.Scripts.Helper;
using GameBoost.Scripts.Services.Models;
using GameBoost.Scripts.Services.Shell;
using GameBoost.SystemInformation.Steps;
using System.Diagnostics;

namespace GameBoost.SystemInformation.Core
{
    public class SystemInfoLoader
    {
        private readonly List<ISystemInfoStep> _steps;

        private static SystemInfo _cachedInfo;
        public static SystemInfo GetCachedInfo() => _cachedInfo;

        public SystemInfoLoader()
        {
            _steps =
            [
                new OSStep(),
                new CpuStep(),
                new GpuStep(),
                new MemoryStep(),
                new MotherboardStep(),
            ];
        }

        public async Task<SystemInfo> LoadAsync(IProgress<ProgressInfo> progress)
        {
            try
            {
                if (_cachedInfo != null)
                    return _cachedInfo;

                var systemInfo = new SystemInfo
                {
                    IsAdministrator = AdminExecutionService.IsAdministrator()
                };

                foreach (var step in _steps)
                {
                    progress.Report(
                        new ProgressInfo(
                            step.Name,
                            MathHelper.ToPercentageInt(_steps.IndexOf(step) + 1, _steps.Count)
                            ));

                    await step.EcecuteAsync(systemInfo);
                }

                _cachedInfo = systemInfo;
                return systemInfo;

            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error loading system info: {ex.Message}");
#endif

                return _cachedInfo;
            }
        }
    }
}
