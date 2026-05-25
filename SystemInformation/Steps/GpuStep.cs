using GameBoost.Core.Interfaces;
using GameBoost.SystemInformation.Core;
using GameBoost.SystemInformation.Providers;

namespace GameBoost.SystemInformation.Steps
{
    internal class GpuStep : ISystemInfoStep
    {
        public string Name => "Reading GPU";

        public Task ExecuteAsync(SystemInfo info)
        {
            info.GPU = GpuInfoProvider.FetchGpuInformation();

            return Task.CompletedTask;
        }
    }
}
