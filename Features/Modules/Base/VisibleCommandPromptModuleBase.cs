using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.Shell;
using GameBoost.Shared.Results;
using System.ComponentModel;
using System.Diagnostics;

namespace GameBoost.Features.Modules.Base
{
    public abstract class VisibleCommandPromptModuleBase : IActionModule, IRequiredModule
    {
        public abstract string Name { get; }

        #region Command
        protected abstract ShellType Shell { get; }
        protected abstract string Command { get; }
        protected virtual string WindowTitle => $"GameBoost - {Name}";
        #endregion

        #region IRequiredModule
        public virtual bool Admin => true;
        public virtual bool SystemReboot => false;
        #endregion

        protected static string FormatStatus(ToggleType status) => status == ToggleType.None ? "" : status.ToString();
        public async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var status = GetStatus();

            return ActionRefreshResult.ValueOnly(status, FormatStatus(status));
        }
        public virtual ToggleType GetStatus()
        {
            return ToggleType.None;
        }

        public Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                VisibleCommandPromptService.StartCommand(Shell, command: Command, title: WindowTitle, runAsAdministrator: Admin);

                return Task.FromResult(
                    ModuleResult.Successful($"{Name} was started in Command Prompt"));
            }
            catch (OperationCanceledException)
            {
                return Task.FromResult(
                    ModuleResult.Failed($"{Name} was cancelled"));
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == 1223)
            {
                return Task.FromResult(
                    ModuleResult.Failed("Administrator permission was cancelled"));
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Failed to start {Name}: {ex.Message}");
#endif
                return Task.FromResult(
                    ModuleResult.Failed(ex.Message));
            }
        }
    }
}