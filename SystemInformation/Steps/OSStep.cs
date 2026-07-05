using GameBoost.Core.Interfaces;
using GameBoost.SystemInformation.Core;
using GameBoost.SystemInformation.Providers;

namespace GameBoost.SystemInformation.Steps
{
    internal class OSStep : ISystemInfoStep
    {
        public string Name => "Reading OS";

        public Task ExecuteAsync(SystemInfo info, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            info.OS = OSInfoProvider.FetchOSInformation();

            return Task.CompletedTask;
        }
    }
}
