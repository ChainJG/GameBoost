using GameBoost.Core.Interfaces;
using GameBoost.Shared.Results;
using GameBoost.SystemInformation.Core;

namespace GameBoost.Application.Startup
{
    public class LoadSystemInfoStartupStep : IStartupStep
    {
        public string Name => "Load System Information";

        public async Task<ModuleResult> ExecuteAsync(IProgress<ProgressResult> progress, CancellationToken token)
        {
            var systemLoader = new SystemInfoLoader();

            var sysInfo = await systemLoader.LoadAsync(progress, token);

            GameBoostContext.SystemInfo = sysInfo;

            return ModuleResult.Successful();
        }
    }
}
