using GameBoost.Core;
using GameBoost.Features.Modules.Base;
using GameBoost.Shared.Results;
using System.ComponentModel;
using System.Diagnostics;

namespace GameBoost.Features.Modules.SystemModules.Cleanup
{
    public sealed class WindowsDiskCleanupModule : ShellCommandModuleBase
    {
        public override string Name => "Windows Disk Cleanup";

        public override ShellType Shell => ShellType.PowerShell;
        public override string Command => "";

        public async override Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var systemDrive = GameBoostServices.GetSystemDrive();

                Process.Start(new ProcessStartInfo
                {
                    FileName = "cleanmgr.exe",
                    Arguments = $"/d {systemDrive}",
                    UseShellExecute = true,
                    CreateNoWindow = false,
                });

                return ModuleResult.Successful($"Windows Disk Clean-up launched for {systemDrive}");

            }
            catch (OperationCanceledException)
            {
                return ModuleResult.Failed("Windows Disk Clean-up launch was cancelled");
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == 1223)
            {
                return ModuleResult.Failed("Windows Disk Clean-up launch was cancelled");
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Failed to open Windows Disk Clean-up: {ex.Message}");
#endif

                return ModuleResult.Failed("Failed to open Windows Disk Clean-up");
            }
        }
    }
}
