using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.MicrosoftApps.Services;
using GameBoost.Shared.Results;

namespace GameBoost.Application.Startup
{
    public class LoadMicrosoftAppxPackageInfoStep : IStartupStep
    {
        public string Name => "Load Microsoft Appx Package Info";

        public async Task<ModuleResult> ExecuteAsync(IProgress<ProgressResult> progress)
        {
            var packages = await AppxPackageService.GetInstalledPackagesAsync(progress);

            GameBoostContext.MicrosoftInstalledPackages = packages;

            return ModuleResult.Successful();
        }
    }
}
