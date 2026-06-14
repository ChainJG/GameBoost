using GameBoost.Application;
using GameBoost.Features.RestorePoints;
using GameBoost.Features.Updates;
using GameBoost.Shared.Results;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows;

namespace GameBoost.Core
{
    public static class GameBoostServices
    {
        private const int UacCancelledErrorCode = 1223;

        public static readonly string ExePath = Environment.ProcessPath!;

        public static void Shutdown() =>
            System.Windows.Application.Current?.Shutdown();

        public static string GetSystemDrive() => Environment.GetEnvironmentVariable("SystemDrive") ?? "C:";

        #region Restart Administrator
        public static Task<ModuleResult> ShowRestartAdministratorDialog(
            string message = "Administrator permission is required.\nRestart as administrator?")
        {
            var result = MessageBox.Show(
                message,
                "Administrator",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result != MessageBoxResult.Yes)
                return Task.FromResult(ModuleResult.Failed("Administrator restart was declined."));

            return RestartAsAdministratorAsync();
        }
        public static Task<ModuleResult> RestartAsAdministratorAsync()
        {
            try
            {
                var executablePath = Environment.ProcessPath;

                if (string.IsNullOrWhiteSpace(executablePath))
                {
                    return Task.FromResult(
                        ModuleResult.Failed("Unable to restart as administrator. Process path is null or empty."));
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    Verb = "runas",
                    UseShellExecute = true,
                    WorkingDirectory = AppContext.BaseDirectory
                };

                var process = Process.Start(startInfo);

                if (process is null)
                {
                    return Task.FromResult(
                        ModuleResult.Failed("Failed to start GameBoost as administrator."));
                }

                Shutdown();

                return Task.FromResult(
                    ModuleResult.Successful("GameBoost restarted as administrator."));
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == UacCancelledErrorCode)
            {
                return Task.FromResult(
                    ModuleResult.Failed("Administrator permission was declined."));
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Failed to restart as administrator: {ex.Message}");
#endif
                return Task.FromResult(
                    ModuleResult.Failed($"Failed to restart as administrator. {ex.Message}"));
            }
        }
        public static bool IsAdministrator()
        {
            using var identity =
                WindowsIdentity.GetCurrent();

            var principal =
                new WindowsPrincipal(identity);

            return principal.IsInRole(
                WindowsBuiltInRole.Administrator);
        }
        #endregion

        #region Restart System
        public static Task<ModuleResult> ShowRestartDialog(
            string message = "GameBoost needs to restart your PC.\n\nWould you like to restart now?")
        {
            var result = MessageBox.Show(
                message,
                "Restart Required",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result != MessageBoxResult.Yes)
                return Task.FromResult(ModuleResult.Failed("System restart was declined."));

            return RestartComputerAsync();
        }
        public static Task<ModuleResult> RestartComputerAsync()
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "shutdown",
                    Arguments = "/r /t 0",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                var process = Process.Start(startInfo);

                if (process is null)
                {
                    return Task.FromResult(
                        ModuleResult.Failed("Failed to start Windows restart command."));
                }

                return Task.FromResult(
                    ModuleResult.Successful("Windows restart command started."));
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Failed to restart computer: {ex.Message}");
#endif
                MessageBox.Show(
                    "GameBoost could not restart your PC automatically.",
                    "Restart Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return Task.FromResult(
                    ModuleResult.Failed($"Failed to restart computer. {ex.Message}"));
            }
        }
        #endregion

        #region Update Newest Version
        public static async Task<ModuleResult> ShowUpdateDialog(UpdateReleaseInfo releaseInfo, IProgress<ProgressResult>? progress = default)
        {
            // Ask user if they want to install update
            var wantsUpdate = MessageBox.Show(
                "A new update is available\nDo you want to update now?",
                "Update Available",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information)

                == MessageBoxResult.Yes;

            // User declined update
            if (!wantsUpdate)
                return ModuleResult.Failed("User declined update");

            // Download and launch installer
            await releaseInfo.DownloadAndInstallAsync(progress);

            // Close current application instance
            Shutdown();

            return ModuleResult.Successful("Update installed successfully");
        }
        #endregion

        #region Restore Point 
        public static async Task<ModuleResult> ShowRestorePointDialog(IProgress<ProgressResult>? progress = default)
        {
            var result = ModuleResult.Failed();

            var wantsRestorePoint = MessageBox.Show(
                "Your system does not have an active restore point\n" +
                "Do you want to create one now?",
                "Restore Point",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes;

            if (wantsRestorePoint)
                result = await RestorePointService.CreateRestorePointAsync(progress);

            return result;
        }
        #endregion
    }
}