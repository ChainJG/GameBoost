using GameBoost.Infrastructure.Shell;
using GameBoost.Shared.Results;

namespace GameBoost.Core
{
    public class GameBoostServices
    {
        public static ModuleResult RestartAsAdministrator()
        {
            var executionPath = Environment.ProcessPath;

            if (string.IsNullOrWhiteSpace(executionPath))
                return ModuleResult.Failed("Unable to restart as administrator. Process path is null or empty");

            var result = ElevatedPowerShellService.RunPowerShellAsAdmin(
                executionPath);

            if (!result.Success)
                return ModuleResult.Failed($"Failed to restart as administrator. {result.Output}");

            Shutdown();

            return ModuleResult.Successful();
        }

        public static void Shutdown() => System.Windows.Application.Current?.Shutdown();
    }
}
