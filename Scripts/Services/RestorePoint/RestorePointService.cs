using GameBoost.Core.Services;
using GameBoost.Scripts.Helpers;
using GameBoost.Scripts.Models;
using GameBoost.Scripts.Services.AppState;
using GameBoost.Scripts.Services.Models;
using GameBoost.Scripts.Services.Shell;
using GameBoost.SystemInformation.Core;
using System.Diagnostics;
using System.Windows;

namespace GameBoost.Scripts.Services.RestorePoint
{
    public class RestorePointService
    {
        private const int RestorePointCheckProgress = 25;
        private const int AdminCheckProgress = 30;
        private const int ProtectionCheckProgress = 50;
        private const int ProtectionEnableProgress = 60;
        private const int RestorePointCreateProgress = 80;
        private const int CompletedProgress = 100;

        private const int ResultDisplayDelayMilliseconds = 500;

        private const string AdminRequiredMessage = "You must be an admin to create a restore point";
        private const string ProtectionRequiredMessage = "System protection is required to create a restore point";
        private const string ProtectionDeclinedMessage = "System protection was not enabled";

        public static async Task<bool> CheckActiveRestorePointAsync(
            IProgress<ProgressInfo> progress)
        {
            // Report progress
            progress.Report(new ProgressInfo("Checking for restore point", RestorePointCheckProgress));

            // Check for restore point
            return await Task.Run(RestorePointHelper.HasGameBoostRestorePoint);
        }

        public static async Task<ModuleResult> CreateProtectionPointAsync(
            IProgress<ProgressInfo> progress)
        {
            try
            {
                // Stop if the user is not an admin
                if (!AdminExecutionService.EnsureAdministrator(progress, AdminCheckProgress))
                    return ModuleHelper.CreateFailedResult(AdminRequiredMessage, ResultType.AdministratorProtection);

                var protectionResult = EnsureSystemProtectionEnabled(progress);

                // Stop if system protection could not be enabled or was declined
                if (!protectionResult.Success)
                    return protectionResult;

                // Report progress
                progress.Report(new ProgressInfo("Creating restore point", RestorePointCreateProgress));

                // Restore point creation uses Windows APIs, so it runs off the UI thread
                var result = await Task.Run(() => RestorePointHelper.CreateRestorePoint());

                // Report progress
                progress.Report(new ProgressInfo(result.Message, CompletedProgress));

                // Gives the user a short moment to see the final result
                await Task.Delay(ResultDisplayDelayMilliseconds);

                return result;

            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error creating restore point: {ex.Message}");
#endif

                progress.Report(new ProgressInfo($"{ex.Message}", CompletedProgress));

                return ModuleHelper.CreateFailedResult(ex.Message);
            }
        }

        private static ModuleResult EnsureSystemProtectionEnabled(IProgress<ProgressInfo> progress)
        {
            // System protection is not enabled
            if (!RestorePointHelper.IsSystemProtectionEnabled())
            {
                // Report progress
                progress.Report(new ProgressInfo(ProtectionRequiredMessage, ProtectionCheckProgress));

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
                    progress.Report(new ProgressInfo("Enabling system protection", ProtectionEnableProgress));

                    // Enable system protection
                    return RestorePointHelper.EnableSystemProtection();

                }

                return ModuleHelper.CreateFailedResult(ProtectionDeclinedMessage, ResultType.Failed);
            }

            return ModuleHelper.CreateSuccessfulResult("System protection is enabled");
        }

    }
}
