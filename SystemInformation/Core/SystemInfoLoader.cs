using GameBoost.Core;
using GameBoost.Core.Interfaces;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;
using GameBoost.SystemInformation.Steps;
using System.Diagnostics;

namespace GameBoost.SystemInformation.Core
{
    public class SystemInfoLoader
    {
        private readonly List<ISystemInfoStep> _steps;

        private static SystemInfo? _cachedInfo;

        public SystemInfoLoader()
        {
            _steps =
            [
                new CpuStep(),
                new GpuStep(),
                new MemoryStep(),
                new MotherboardStep(),
                new OSStep(),
            ];
        }

        public async Task<SystemInfo> LoadAsync(IProgress<ProgressResult> progress, CancellationToken token)
        {
            if (_cachedInfo != null)
                return _cachedInfo;

            var systemInfo = new SystemInfo
            {
                IsAdministrator = GameBoostServices.IsAdministrator()
            };

            for (var i = 0; i < _steps.Count; i++)
            {
                token.ThrowIfCancellationRequested();

                var step = _steps[i];

                progress.Report(
                    new ProgressResult(
                        step.Name,
                        MathHelper.ToPercentageInt(i + 1, _steps.Count)
                        ));
                try
                {
                    // Steps run synchronous WMI/registry queries; push them off the UI
                    // thread here so the splash screen stays responsive during startup.
                    await Task.Run(() => step.ExecuteAsync(systemInfo, token), token);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debug.WriteLine($"Error executing step {step.Name}: {ex.Message}");
#endif
                }
            }

            _cachedInfo = systemInfo;
            return systemInfo;
        }
    }
}
