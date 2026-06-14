using GameBoost.Infrastructure.Shell;
using GameBoost.Shared.Results;
using System.Diagnostics;

namespace GameBoost.Features.Modules.Base
{
    public abstract class ToggleShellCommandModuleBase : ActionModuleBase
    {
        #region Commands
        public abstract ShellType Shell { get; }
        public abstract string EnabledCommand { get; }
        public abstract string DisableCommand { get; }
        #endregion

        public virtual ToggleType GetStatus() => ToggleType.None;
        protected override string FormatStatus(ToggleType status) => status == ToggleType.None ? "" : status.ToString();
        public override async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var status = GetStatus();

            return ActionRefreshResult.ValueOnly(status, FormatStatus(status));
        }

        public override async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var currentStatus = GetStatus();
                var targetStatus = GetTargetStatus(currentStatus);
                var command = GetCommand(targetStatus);

                var result = await ShellService.RunAsync(Shell, command, token);

                if (!result.Success)
                    return ModuleResult.Failed(result.Error);

                return ModuleResult.Successful($"Successfully {targetStatus} {Name}");

            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"{Name} Execution Failed: {ex.Message}");
#endif

                return ModuleResult.Failed(ex.Message);
            }
        }

        private static ToggleType GetTargetStatus(ToggleType currentStatus) => currentStatus == ToggleType.Enabled ? ToggleType.Disabled : ToggleType.Enabled;
        private string GetCommand(ToggleType targetStatus) => targetStatus == ToggleType.Enabled ? EnabledCommand : DisableCommand;
    }
}

