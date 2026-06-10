using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.WindowsModules.Taskbar
{
    public sealed class TaskViewTaskbarModule : SystemTweakModuleBase
    {
        public override string Name => "Task View";

        public override RegistryEditInfo[] RegistryEdits =>
        [
            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                Key = "ShowTaskViewButton",
                EnabledValue = 1,
                DisabledValue = 0
            },
        ];
    }
}
