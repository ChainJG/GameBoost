using GameBoost.Core.Interfaces;
using GameBoost.Features.RestorePoints;
using GameBoost.Shared.Results;
using System.Windows;

namespace GameBoost.Application.Startup
{
    public class CheckRestorePointStartupStep : IStartupStep
    {
        public string Name => "Check Restore Point";

        public async Task<ModuleResult> ExecuteAsync(IProgress<ProgressResult> progress)
        {
            // Check if we have an active restore point (GameBoost Description)
            var activeRestorePoint = await RestorePointService.HasActiveRestorePointAsync(progress);
            var result = ModuleResult.Successful();

            if (!activeRestorePoint)
            {
                // Prompt to create a restore point
                var wantsRestore = MessageBox.Show(
                    "Your system does not have an active restore point\n" +
                    "Do you want to create one now?",
                    "Restore Point",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes;

                if (wantsRestore)
                {
                    // Create a restore point
                    result = await RestorePointService.CreateRestorePointAsync(progress);
                    activeRestorePoint = result.Success;
                }
            }

            // Cache the result
            GameBoostContext.HasActiveRestorePoint = activeRestorePoint;

            return result;
        }
    }
}
