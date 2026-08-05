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

            // Warms the installed-package cache inside AppxPackageOperationService,
            // which is what the Microsoft app modules read from.
            await AppxPackageOperationService.GetInstalledPackagesAsync(token);

            return ModuleResult.Successful();
        }
    }
}
