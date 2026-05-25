using GameBoost.Core.Interfaces;
using GameBoost.SystemInformation.Core;
using GameBoost.SystemInformation.Providers;
using System.Diagnostics;

namespace GameBoost.SystemInformation.Steps
{
    class CpuStep : ISystemInfoStep
    {
        public string Name => "Reading CPU";

        public Task ExecuteAsync(SystemInfo info)
        {
            info.CPU = CpuInfoProvider.FetchCpuInformation();

            return Task.CompletedTask;
        }
    }
}
