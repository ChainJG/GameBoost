using GameBoost.Core.Interfaces;
using GameBoost.Shared.Results;
using System.Diagnostics;

namespace GameBoost.Features.Modules.Windows.PowerPlan
{
    public sealed class SetPowerPlanModule : IInputActionModule<object>
    {
        public string Name => "Set Power Plan";

        public Task<string> RefreshStatusAsync(object input, CancellationToken token)
        {
            return Task.FromResult("Ready");
        }

        public async Task<ModuleResult> ExecuteAsync(object input, CancellationToken token)
        {
            if (input is not string powerPlanGuid || string.IsNullOrWhiteSpace(powerPlanGuid))
                return ModuleResult.Failed("No power plan selected.");

            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "powercfg",
                Arguments = $"/setactive {powerPlanGuid}",
                UseShellExecute = false,
                CreateNoWindow = true
            });

            if (process is null)
                return ModuleResult.Failed("Failed to start powercfg.");

            await process.WaitForExitAsync(token);

            return process.ExitCode == 0
                ? ModuleResult.Successful("Power plan changed successfully.")
                : ModuleResult.Failed($"Failed to change power plan. Exit code: {process.ExitCode}");
        }
    }
}