using GameBoost.Core;
using GameBoost.Features.Modules.Base;

namespace GameBoost.Features.Modules.SystemModules.SystemTroubleshoot
{
    public sealed class CheckDiskOnlineScanModule : VisibleCommandPromptModuleBase
    {
        public override string Name => "Check Disk Online Scan";

        protected override ShellType Shell => ShellType.Cmd;
        protected override string Command => $"chkdsk {GameBoostServices.GetSystemDrive()} /scan";
        protected override string WindowTitle => "GameBoost - Check Disk Online Scan";

    }
}
