using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.Windows.Taskbar
{
    public sealed class EndTaskTaskbarModule : SystemTweakModuleBase
    {
        public override string Name => "End Task";

        public override RegistryEditInfo[] RegistryEdits =>
        [
            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDeveloperSettings",
                Key = "TaskbarEndTask",
                EnabledValue = 1,
                DisabledValue = 0
            },
        ];
    }
}
