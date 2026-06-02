using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.Windows.Taskbar
{
    public sealed class SearchboxTaskbarModule : SystemTweakModuleBase
    {
        public override string Name => "End Task";

        public override RegistryEditInfo[] RegistryEdits =>
        [
            new RegistryEditInfo
                {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.SearchPath,
                Key = "SearchboxTaskbarMode",
                EnabledValue = 2,
                DisabledValue = 0
                },
            ];
    }
}
