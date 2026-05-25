using GameBoost.Core.Interfaces;
using GameBoost.SystemInformation.Core;
using GameBoost.SystemInformation.Providers;

namespace GameBoost.SystemInformation.Steps
{
    internal class MotherboardStep : ISystemInfoStep
    {
        public string Name => "Read Motherboard";

        public Task EcecuteAsync(SystemInfo info)
        {
            info.Motherboard = MotherboardInfoProvider.FetchMotherboardInformation();

            return Task.CompletedTask;
        }
    }
}
