using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.Shell;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;
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

        public async Task<SystemInfo> LoadAsync(IProgress<ProgressResult> progress)
        {
            try
            {
                if (_cachedInfo != null)
                    return _cachedInfo;

                var systemInfo = new SystemInfo
                {
                    IsAdministrator = AdminAccessService.IsAdministrator()
                };


                for (var i = 0; i < _steps.Count; i++)
                {
                    var step = _steps[i];

                    progress.Report(
                        new ProgressResult(
                            step.Name,
                            MathHelper.ToPercentageInt(i + 1, _steps.Count)
                            ));

                    await step.ExecuteAsync(systemInfo);
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
