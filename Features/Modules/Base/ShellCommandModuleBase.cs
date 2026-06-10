using GameBoost.Core;
using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.Shell;
using GameBoost.Shared.Results;
using System.Diagnostics;

namespace GameBoost.Features.Modules.Base
{
    public abstract class ShellCommandModuleBase : IActionModule, IRecommendedActionModule, IRequireModule
    {
        public abstract string Name { get; }
        public abstract string Command { get; }
        public abstract ShellType Shell { get; }

        #region IRecommendedActionModule
        public virtual RecommendationPriority RecommendationPriority { get; } = RecommendationPriority.None;
        public virtual object? RecommendedValue { get; } = ToggleType.Enabled;
        public virtual string RecommendationReason =>
            $"{Name} is recommended to be {RecommendedValue}";
        public virtual bool IsRecommendedValue(object? currentValue)
        {
            if (RecommendedValue is not ToggleType recommendedValue)
                return false;

            return currentValue is ToggleType toggleType &&
                   toggleType == recommendedValue;
        }
        #endregion

        #region IRequireModule
        public virtual bool SystemReboot { get; } = false;
        public virtual bool Admin { get; } = false;
        #endregion


        public abstract Task<ModuleResult> ExecuteAsync(CancellationToken token);
        public virtual Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            return Task.FromResult(
                ActionRefreshResult.ValueOnly(
                    ToggleType.Unknown,
                    string.Empty));
        }

        protected virtual string FormatStatus(ToggleType status) => status.ToString();
        protected ActionRefreshResult ToggleStatusResult(ToggleType status)
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

                var result = await ShellService.RunAsync(shell, command);

                token.ThrowIfCancellationRequested();

                if (!result.Success)
                    return ModuleResult.Failed(result.Output);

                return ModuleResult.Successful(result.Output);
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

            var result = await ShellService.RunAsync(
                shell,
                command);

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

        public Task<ModuleResult> ExecuteRecommendedAsync(CancellationToken token) => ExecuteAsync(token);
    }
}

