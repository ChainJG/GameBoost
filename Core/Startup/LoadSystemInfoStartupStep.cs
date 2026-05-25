using GameBoost.Core.Interfaces;
using GameBoost.Shared.Results;
using GameBoost.SystemInformation.Core;

namespace GameBoost.Core.Startup
{
    public class LoadSystemInfoStartupStep : IStartupStep
    {
        public async Task<ModuleResult> ExecuteAsync(IProgress<ProgressResult> progress)
        {
            var systemLoader = new SystemInfoLoader();
            var sysInfo = await systemLoader.LoadAsync(progress);

            GameBoostContext.SystemInfo = sysInfo;

            return ModuleResult.Successful();
        }
    }
}
