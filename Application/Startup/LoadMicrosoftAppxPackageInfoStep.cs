using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.MicrosoftApps.Services;
using GameBoost.Shared.Results;

namespace GameBoost.Application.Startup
{
    public class LoadMicrosoftAppxPackageInfoStep : IStartupStep
    {
        public string Name => "Load Microsoft Appx Package Info";

        public async Task<ModuleResult> ExecuteAsync(IProgress<ProgressResult> progress, CancellationToken token)
        {
            progress?.Report(new ProgressResult("Fetching Microsoft packages...", 70));

            var packages = await AppxPackageOperationService.GetInstalledPackagesAsync(token);

            GameBoostContext.MicrosoftInstalledPackages = packages;

            return ModuleResult.Successful();
        }
    }
}
