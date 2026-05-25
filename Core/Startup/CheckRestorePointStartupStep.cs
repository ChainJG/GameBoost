using GameBoost.Core.Interfaces;
using GameBoost.Features.RestorePoints;
using GameBoost.Shared.Results;
using System.Windows;

namespace GameBoost.Core.Startup
{
    public class CheckRestorePointStartupStep : IStartupStep
    {
        public async Task<ModuleResult> ExecuteAsync(IProgress<ProgressResult> progress)
        {
            // Check if we have an active restore point (GameBoost Description)
            var activeRestorePoint = await RestorePointService.CheckActiveRestorePointAsync(progress);
            var result = ModuleResult.Successful();

            if (!activeRestorePoint)
            {
                // Prompt to create a restore point
                var wantsRestore = MessageBox.Show(
                    "Your system does not have an active restore point\nDo you want to create one now?",
                    "Restore Point",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes;

                if (wantsRestore)
                {
                    // Create a restore point
                    result = await RestorePointService.CreateProtectionPointAsync(progress);
                    activeRestorePoint = result.Success;
                }
            }

            // Cache the result
            GameBoostContext.HasActiveRestorePoint = activeRestorePoint;

            return result;
        }
    }
}
