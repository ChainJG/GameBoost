using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.WindowsModules.Taskbar
{
    public class WidgetsTaskbarModule : SystemTweakModuleBase
    {
        public override string Name => "Taskbar Widgets";

        public override bool Admin => true;

        public override RegistryEditInfo[] RegistryEdits =>
        [
            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                Key = "TaskbarDa",
                EnabledValue = 1,
                DisabledValue = 0
            },
        ];
    }
}
