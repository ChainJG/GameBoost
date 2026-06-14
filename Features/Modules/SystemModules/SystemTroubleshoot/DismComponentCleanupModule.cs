using GameBoost.Features.Modules.Base;

namespace GameBoost.Features.Modules.SystemModules.SystemTroubleshoot
{
    public sealed class DismComponentCleanupModule : VisibleCommandPromptModuleBase
    {
        public override string Name => "Windows Component Cleanup";

        protected override ShellType Shell => ShellType.Cmd;
        protected override string Command => "DISM /Online /Cleanup-Image /StartComponentCleanup";
        protected override string WindowTitle => "GameBoost - Windows Component Cleanup";
    }
}
