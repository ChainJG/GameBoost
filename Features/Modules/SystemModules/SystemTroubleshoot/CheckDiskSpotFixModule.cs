
using GameBoost.Core;
using GameBoost.Features.Modules.Base;

namespace GameBoost.Features.Modules.SystemModules.SystemTroubleshoot
{
    public sealed class CheckDiskSpotFixModule : VisibleCommandPromptModuleBase
    {
        public override string Name => "Check Disk Spot Fix";

        public override bool SystemReboot => true;

        protected override ShellType Shell => ShellType.Cmd;
        protected override string Command => $"chkdsk {GameBoostServices.GetSystemDrive()} /spotfix";
        protected override string WindowTitle => "GameBoost - Check Disk Spot Fix";

    }
}
