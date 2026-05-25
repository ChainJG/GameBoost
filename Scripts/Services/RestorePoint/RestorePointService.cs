using GameBoost.Core.Services;
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
        public static async Task<ModuleResult> CreateProtectionPointAsync(
            IProgress<ProgressInfo> progress)
        {
            try
            {
                // Check for Admin privileges 
                if (!AdminExecutionService.EnsureAdministrator())
                {
                    // Report progress
                    progress.Report(new ProgressInfo("You must be an admin to create a restore point", 25));
                    return new ModuleResult { Success = false, Message = "You must be an admin to create a restore point" };
                }

                await HandleSystemProtection(progress);

                // Report progress
                progress.Report(new ProgressInfo("Creating restore point", 80));

                // Create restore point
                var result = await Task.Run(() => RestorePointHelper.CreateRestorePoint());

                // Delay so the user can the result
                await Task.Delay(500);

                // Report progress
                progress.Report(new ProgressInfo($"{result.Message}", 100));

                return result;

            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error creating restore point: {ex.Message}");
#endif

                progress.Report(new ProgressInfo($"{ex.Message}", 100));
                return new ModuleResult { Success = false, Message = ex.Message };
            }
        }

        private static async Task HandleSystemProtection(IProgress<ProgressInfo> progress)
        {
            // Check if system protection is enabled
            var isSystemProtectionEnabled = RestorePointHelper.IsSystemProtectionEnabled();

            // System protection is not enabled
            if (!isSystemProtectionEnabled)
            {
                // Report progress
                progress.Report(new ProgressInfo("System protection is not enabled", 50));

                // Prompt to enable
                var wantEnabld = MessageBox.Show(
                    $"System protection is required to create a restore point\nDo you want to enable it now?",
                    "Restore Point",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes;

                if (wantEnabld)
                {
                    // Report progress
                    progress.Report(new ProgressInfo("Enabling system protection", 60));

                    // Enable system protection
                    var systemProtection = RestorePointHelper.EnableSystemProtection();

                    if (!systemProtection.Success)
                        throw new Exception("Failed to enable system protection");
                }
                else
                    throw new Exception("System protection has been declined");
            }
        }

        public static async Task<bool> CheckActiveRestorePoint(
            IProgress<ProgressInfo> progress)
        {
            // Report progress
            progress.Report(new ProgressInfo("Checking for restore point", 25));

            // Check for restore point
            return RestorePointHelper.HasGameBoostRestorePoint();
        }
    }
}
