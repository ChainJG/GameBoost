using GameBoost.Core.Interfaces;
using GameBoost.Features.Updates;
using GameBoost.Shared.Results;

namespace GameBoost.Core.Startup
{
    public class CheckForUpdatesStartupStep : IStartupStep
    {
        public async Task<ModuleResult> ExecuteAsync(IProgress<ProgressResult> progress)
        {
            // Checking GitHub releases for updates
            var update = await GitHubUpdateChecker.CheckForUpdatesAsync(progress);

            // Cache the result
            GameBoostContext.UpdateInfo = update;

            return ModuleResult.Successful();
        }
    }
}
