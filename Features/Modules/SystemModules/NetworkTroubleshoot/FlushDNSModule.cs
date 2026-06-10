using GameBoost.Features.Modules.Base;
using GameBoost.Shared.Results;

namespace GameBoost.Features.Modules.SystemModules.NetworkTroubleshoot
{
    public sealed class FlushDNSModule : ShellCommandModuleBase
    {
        public override string Name => "Flush DNS";

        public override ShellType Shell => ShellType.Cmd;

        public override string Command => "ipconfig /flushdns";

        public override async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            return await RunCommandAsync(Shell, Command, token);
        }
    }
}
