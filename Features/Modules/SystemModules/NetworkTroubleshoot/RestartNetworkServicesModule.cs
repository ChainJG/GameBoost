using GameBoost.Features.Modules.Base;
using GameBoost.Shared.Results;

namespace GameBoost.Features.Modules.SystemModules.NetworkTroubleshoot
{
    public sealed class RestartNetworkServicesModule : ShellCommandModuleBase
    {
        public override string Name => "Restart Network Services";

        public override ShellType Shell => ShellType.PowerShell;

        public override string Command =>
            "$services = @('Dnscache','Dhcp','NlaSvc'); " +
            "foreach ($service in $services) { " +
            "if (Get-Service -Name $service -ErrorAction SilentlyContinue) { " +
            "Restart-Service -Name $service -Force -ErrorAction Stop " +
            "} " +
            "}";

        public override async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            return await RunCommandAsync(Shell, Command, token);
        }
    }
}
