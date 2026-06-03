using GameBoost.Application;
using GameBoost.Core;
using GameBoost.Infrastructure.Shell;
using GameBoost.Shared.Results;
using System.Diagnostics;
using System.Windows;

namespace GameBoost.Features.RestorePoints
{
    public class RestorePointService
    {
        private const int RestorePointCheckProgress = 25;
        private const int ProtectionCheckProgress = 50;
        private const int ProtectionEnableProgress = 60;
        private const int RestorePointCreateProgress = 80;
        private const int CompletedProgress = 100;

        private const int ResultDisplayDelayMilliseconds = 500;

        private const string ProtectionRequiredMessage = "System protection is required to create a restore point";
        private const string ProtectionDeclinedMessage = "System protection was not enabled";

        public static async Task<bool> HasActiveRestorePointAsync(
            IProgress<ProgressResult> progress)
        {
            // Report progress
            progress.Report(new ProgressResult("Checking for restore point", RestorePointCheckProgress));

            // Check for restore point
            return await Task.Run(RestorePointHelper.HasExistingGameBoostRestorePoint);
        }

        public static async Task<ModuleResult> CreateRestorePointAsync(
            IProgress<ProgressResult>? progress)
        {
            try
            {
                // Stop if the user is not an admin
                if (!GameBoostServices.IsAdministrator())
                    return await GameBoostServices.ShowRestartAdministratorDialog();

                var protectionResult = await EnsureSystemProtectionEnabled(progress);

                // Stop if system protection could not be enabled or was declined
                if (!protectionResult.Success)
                    return protectionResult;

                // Report progress
                progress?.Report(new ProgressResult("Creating restore point", RestorePointCreateProgress));

                // Restore point creation uses Windows APIs, so it runs off the UI thread
                var result = await Task.Run(() => RestorePointHelper.CreateRestorePoint());

                // Report progress
                progress?.Report(new ProgressResult(result.Message, CompletedProgress));

                // Update the global state
                GameBoostContext.HasActiveRestorePoint = result.Success;

                // Gives the user a short moment to see the final result
                await Task.Delay(ResultDisplayDelayMilliseconds);

                return result;

            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error creating restore point: {ex.Message}");
#endif

                progress?.Report(new ProgressResult($"{ex.Message}", CompletedProgress));

                return ModuleResult.Failed(ex.Message);
            }
        }

        private static async Task<ModuleResult> EnsureSystemProtectionEnabled(IProgress<ProgressResult>? progress)
        {
            // System protection is not enabled
            if (!RestorePointHelper.IsSystemProtectionEnabled())
            {
                // Report progress
                progress?.Report(new ProgressResult(ProtectionRequiredMessage, ProtectionCheckProgress));

                // Prompt to enable
                var wantsToEnableProtection = MessageBox.Show(
                    $"System protection is required to create a restore point\n" +
                    $"Do you want to enable it now?",
                    "Restore Point",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes;

                if (wantsToEnableProtection)
                {
                    // Report progress
                    progress?.Report(new ProgressResult("Enabling system protection", ProtectionEnableProgress));

                    // Enable system protection
                    return await RestorePointHelper.EnableSystemProtection();

                }

                return ModuleResult.Failed(ProtectionDeclinedMessage);
            }

            return ModuleResult.Successful("System protection is enabled");
        }

    }
}
