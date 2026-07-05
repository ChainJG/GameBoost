using GameBoost.Core;
using GameBoost.Core.Interfaces;
using GameBoost.Features.RestorePoints;
using GameBoost.Shared.Results;

namespace GameBoost.Application.Startup
{
    public class CheckRestorePointStartupStep : IStartupStep
    {
        public string Name => "Check Restore Point";

        public async Task<ModuleResult> ExecuteAsync(IProgress<ProgressResult> progress, CancellationToken token)
        {
            // Check if we have an active restore point (GameBoost InfoToolTip)
            GameBoostContext.HasActiveRestorePoint = await RestorePointService.HasActiveRestorePointAsync(progress, token);

            var result = ModuleResult.Successful();

            if (!GameBoostContext.HasActiveRestorePoint)
            {
                result = await GameBoostServices.ShowRestorePointDialog(progress, token);
            }

            return result;
        }
    }
}
