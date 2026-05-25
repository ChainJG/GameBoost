using GameBoost.Core.Interfaces;
using GameBoost.SystemInformation.Core;
using GameBoost.SystemInformation.Providers;

namespace GameBoost.SystemInformation.Steps
{
    internal class MemoryStep : ISystemInfoStep
    {
        public string Name => "Reading Memory";

        public Task EcecuteAsync(SystemInfo info)
        {
            info.Memory = MemoryInfoProvider.FetchMemoryInformation();

            return Task.CompletedTask;
        }
    }
}
