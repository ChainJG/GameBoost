using GameBoost.Features.Modules.Base;

namespace GameBoost.Features.Modules.SystemModules.SystemTroubleshoot
{
    public sealed class SfcScanModule : VisibleCommandPromptModuleBase
    {
        public override string Name => "System File Checker Scan";

        protected override ShellType Shell => ShellType.Cmd;
        protected override string Command => "sfc /scannow";
        protected override string WindowTitle => "GameBoost - System File Checker";

    }
}