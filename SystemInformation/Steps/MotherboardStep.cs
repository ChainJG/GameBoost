using GameBoost.Core.Interfaces;
using GameBoost.SystemInformation.Core;
using GameBoost.SystemInformation.Providers;

namespace GameBoost.SystemInformation.Steps
{
    internal class MotherboardStep : ISystemInfoStep
    {
        public string Name => "Read Motherboard";

        public Task ExecuteAsync(SystemInfo info, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            info.Motherboard = MotherboardInfoProvider.FetchMotherboardInformation();

            return Task.CompletedTask;
        }
    }
}
