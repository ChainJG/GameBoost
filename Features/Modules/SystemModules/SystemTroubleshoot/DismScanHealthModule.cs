using GameBoost.Features.Modules.Base;

namespace GameBoost.Features.Modules.SystemModules.SystemTroubleshoot
{
    public sealed class DismScanHealthModule : VisibleCommandPromptModuleBase
    {
        public override string Name => "DISM Scan Health";

        protected override ShellType Shell => ShellType.Cmd;
        protected override string Command => "DISM /Online /Cleanup-Image /ScanHealth";
        protected override string WindowTitle => "GameBoost - DISM Scan Health";

    }
}
