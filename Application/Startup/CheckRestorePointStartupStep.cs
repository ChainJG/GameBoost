using GameBoost.Core.Interfaces;
using GameBoost.Features.RestorePoints;
using GameBoost.Shared.Results;

namespace GameBoost.Application.Startup
{
    public class CheckRestorePointStartupStep : IStartupStep
    {
        public string Name => "Check Restore Point";

        /// <summary>
        /// Detects restore-point state only. If no restore point exists the user is
        /// offered one through the "Restore point recommended" title bar action
        /// (see <see cref="StartupNotificationService"/>) rather than a modal dialog,
        /// so startup is never blocked waiting for input.
        /// </summary>
        public async Task<ModuleResult> ExecuteAsync(IProgress<ProgressResult> progress, CancellationToken token)
        {
            GameBoostContext.HasActiveRestorePoint = await RestorePointService.HasActiveRestorePointAsync(progress, token);

            return ModuleResult.Successful();
        }
    }
}
