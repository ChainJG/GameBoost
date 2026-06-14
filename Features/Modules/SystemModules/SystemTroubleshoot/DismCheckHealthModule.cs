using GameBoost.Features.Modules.Base;

namespace GameBoost.Features.Modules.SystemModules.SystemTroubleshoot
{
    public sealed class DismCheckHealthModule : VisibleCommandPromptModuleBase
    {
        public override string Name => "DISM Check Health";

        protected override ShellType Shell => ShellType.Cmd;
        protected override string Command => "DISM /Online /Cleanup-Image /CheckHealth";
        protected override string WindowTitle => "GameBoost - DISM Check Health";

    }
}
