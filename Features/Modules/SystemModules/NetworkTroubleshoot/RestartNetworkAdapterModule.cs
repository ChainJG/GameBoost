using GameBoost.Features.Modules.Base;
using GameBoost.Shared.Results;

namespace GameBoost.Features.Modules.SystemModules.NetworkTroubleshoot
{
    public sealed class RestartNetworkAdapterModule : ShellCommandModuleBase
    {
        public override string Name => "Restart Network Adapter";

        public override ShellType Shell => ShellType.PowerShell;

        public override string Command =>
            "Get-NetAdapter | " +
            "Where-Object { $_.Status -eq 'Up' -and $_.HardwareInterface -eq $true } | " +
            "Restart-NetAdapter -Confirm:$false";

        public override async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            return await RunCommandAsync(Shell, Command, token);
        }
    }
}
