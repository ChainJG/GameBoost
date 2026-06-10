using GameBoost.Features.Modules.Base;
using GameBoost.Shared.Results;

namespace GameBoost.Features.Modules.SystemModules.NetworkTroubleshoot
{
    public sealed class ClearArpCacheModule : ShellCommandModuleBase
    {
        public override string Name => "Clear ARP Cache";

        public override ShellType Shell => ShellType.Cmd;

        public override string Command => "arp -d *";

        public override async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            return await RunCommandAsync(Shell, Command, token);
        }
    }
}
