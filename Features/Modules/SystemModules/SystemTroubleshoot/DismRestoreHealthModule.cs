using GameBoost.Features.Modules.Base;

namespace GameBoost.Features.Modules.SystemModules.SystemTroubleshoot
{
    public sealed class DismRestoreHealthModule : VisibleCommandPromptModuleBase
    {
        public override string Name => "DISM Restore Health";

        protected override ShellType Shell => ShellType.Cmd;
        protected override string Command => "DISM /Online /Cleanup-Image /RestoreHealth";
        protected override string WindowTitle => "GameBoost - DISM Restore Health";
    }
}
