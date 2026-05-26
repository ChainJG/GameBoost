using GameBoost.Core.Interfaces;
using GameBoost.Features.Updates;
using GameBoost.Shared.Results;

namespace GameBoost.Application.Startup
{
    public class CheckForUpdatesStartupStep : IStartupStep
    {
        public string Name => "Check for updates";

        public async Task<ModuleResult> ExecuteAsync(IProgress<ProgressResult> progress)
        {
            // Checking GitHub releases for updates
            var update = await GitHubReleaseService.CheckForUpdatesAsync(progress);

            // Cache the result
            GameBoostContext.UpdateInfo = update;

            return ModuleResult.Successful();
        }
    }
}
