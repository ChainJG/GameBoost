using GameBoost.Features.Modules.Base;
using GameBoost.Shared.Results;

namespace GameBoost.Features.Modules.SystemModules.NetworkTroubleshoot
{
    public sealed class ResetWinsockModule : ShellCommandModuleBase
    {
        public override string Name => "Restart Winsock";

        public override ShellType Shell => ShellType.Cmd;

        public override string Command => "netsh winsock reset";

        public override async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            return await RunCommandAsync(Shell, Command, token);
        }

    }
}
