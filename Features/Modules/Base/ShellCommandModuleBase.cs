using GameBoost.Infrastructure.Shell;
using GameBoost.Shared.Results;
using System.Diagnostics;

namespace GameBoost.Features.Modules.Base
{
    public abstract class ShellCommandModuleBase : ActionModuleBase
    {
        public abstract ShellType Shell { get; }
        public abstract string Command { get; }

        protected override string FormatStatus(ToggleType status) => status == ToggleType.None ? "" : status.ToString();
        public override async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var status = GetStatus();

            return ActionRefreshResult.ValueOnly(status, FormatStatus(status));
        }
        public virtual ToggleType GetStatus()
        {
            return ToggleType.None;
        }
        public override abstract Task<ModuleResult> ExecuteAsync(CancellationToken token);

        protected virtual async Task<ModuleResult> RunCommandAsync(ShellType shell, string command, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var currentStatus = GetStatus();

                var result = await ShellService.RunAsync(shell, command, token);

                token.ThrowIfCancellationRequested();

                if (!result.Success)
                    return ModuleResult.Failed(result.Error);

                return currentStatus switch
                {
                    ToggleType.None => ModuleResult.Successful($"Successfully Operated {Name}"),
                    _ => ModuleResult.Successful($"Successfully {FormatStatus(currentStatus)} {Name}")
                };
            }

            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Failed to run command: {command}, Reason: {ex.Message}");
#endif
                return ModuleResult.Failed($"Failed: {ex.Message}");
            }
        }
    }
}

