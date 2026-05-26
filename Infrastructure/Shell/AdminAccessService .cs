using GameBoost.Application;
using GameBoost.Core;
using GameBoost.Shared.Results;
using System.Security.Principal;
using System.Windows;

namespace GameBoost.Infrastructure.Shell
{
    public class AdminAccessService 
    {
        public static ModuleResult EnsureAdministrator(IProgress<ProgressResult> progress, int progressPercent)
        {
            if (IsAdministrator())
                return ModuleResult.Successful("Administrator privileges already granted");

            progress.Report(new ProgressResult("You must be have administrator privileges", progressPercent));

            var result = MessageBox.Show(
                "Administrator permission is required.\n" +
                "Restart as administrator?",
                "Administrator",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes;


            if (!result)
                return ModuleResult.Failed("Administrator permission was declined");

            return GameBoostServices.RestartAsAdministrator();
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
