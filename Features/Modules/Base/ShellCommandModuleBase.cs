using GameBoost.Infrastructure.Shell;
using GameBoost.Shared.Results;
using System.Diagnostics;

namespace GameBoost.Features.Modules.Base
{
    public abstract class ShellCommandModuleBase : ActionModuleBase
    {
        public abstract ShellType Shell { get; }
        public abstract string Command { get; }

        public override abstract Task<ModuleResult> ExecuteAsync(CancellationToken token);
        public override Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            return Task.FromResult(
                ActionRefreshResult.ValueOnly(
                    ToggleType.Unknown,
                    string.Empty));
        }

        protected override string FormatStatus(ToggleType status) => status.ToString();
        protected ActionRefreshResult GetStatusResult(ToggleType status)
        {
            return ActionRefreshResult.ValueOnly(
                status,
                FormatStatus(status));
        }

        protected virtual async Task<ModuleResult> RunCommandAsync(ShellType shell, string command, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var result = await ShellService.RunAsync(shell, command, token);

                token.ThrowIfCancellationRequested();

                if (!result.Success)
                    return ModuleResult.Failed(result.Error);

                return ModuleResult.Successful($"Successfully {Name}");
            }

            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Failed to run command: {command}, Reason: {ex.Message}");
#endif
                return ModuleResult.Failed($"Failed: {ex.Message}");
            }
        }



        protected static async Task<ToggleType> ReadToggleStatusAsync(
            ShellType shell,
            string command,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var result = await ShellService.RunAsync(shell, command, token);

            token.ThrowIfCancellationRequested();

            if (!result.Success)
                return ToggleType.Unknown;

            return ParseToggleType(result.Output);
        }

        private static ToggleType ParseToggleType(string? output)
        {
            if (string.IsNullOrWhiteSpace(output))
                return ToggleType.Unknown;

            var value = output.Trim();

            if (value.Contains("Enabled", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("True", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("1", StringComparison.OrdinalIgnoreCase))
            {
                return ToggleType.Enabled;
            }

            if (value.Contains("Disabled", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("False", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("0", StringComparison.OrdinalIgnoreCase))
            {
                return ToggleType.Disabled;
            }

            return ToggleType.Unknown;
        }
    }
}

