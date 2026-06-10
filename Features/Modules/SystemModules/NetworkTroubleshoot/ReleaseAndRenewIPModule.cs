using GameBoost.Features.Modules.Base;
using GameBoost.Shared.Results;

namespace GameBoost.Features.Modules.SystemModules.NetworkTroubleshoot
{
    public sealed class ReleaseAndRenewIPModule : ShellCommandModuleBase
    {
        public override string Name => "Release and Renew IP";

        public override ShellType Shell => ShellType.Cmd;

        public override string Command => "ipconfig /release && ipconfig /renew";

        public override async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            return await RunCommandAsync(Shell, Command, token);
        }

    }
}
