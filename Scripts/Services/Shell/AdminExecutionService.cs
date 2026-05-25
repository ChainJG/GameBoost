using GameBoost.Core.Services;
using GameBoost.Scripts.Services.Models;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows;

namespace GameBoost.Scripts.Services.Shell
{
    public static class AdminExecutionService
    {
        public static ProcessResult RunAsAdmin(string fileName, string arguments, bool createNoWindow = true)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,

                    Verb = "runas",
                    CreateNoWindow = createNoWindow,
                    UseShellExecute = true
                };

                using var process = Process.Start(psi);

                process.WaitForExit();

                return new ProcessResult
                {
                    Success = process.ExitCode == 0,
                    ExitCode = process.ExitCode
                };
            }
            catch
            {
                return new ProcessResult
                {
                    Success = false,
                    ExitCode = -1
                };
            }
        }

        public static bool EnsureAdministrator(IProgress<ProgressInfo> progress = default, int percent = 0)
        {
            if (AppServices.SystemInfo != null && AppServices.SystemInfo.IsAdministrator)
                return true;

            progress.Report(new ProgressInfo("You must be have administrator privileges", percent));

            var result = MessageBox.Show(
                "Administrator permission is required.\nRestart as administrator?",
                "Administrator",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                RestartAsAdmin();
            }

            return false;
        }

        private static void RestartAsAdmin()
        {
            var exePath =
                Environment.ProcessPath;

            if (string.IsNullOrWhiteSpace(exePath))
                return;

            Application.Current.Shutdown();

            RunAsAdmin(exePath, string.Empty);
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
    }
}
