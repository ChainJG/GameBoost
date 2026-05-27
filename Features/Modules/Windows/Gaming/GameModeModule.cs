using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.Windows.Gaming
{
    public class GameModeModule : SystemTweakModuleBase
    {
        public override string Name =>
            "Game Mode";

        public override RegistryEditInfo[] RegistryEdits =>
        [
            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = @"Software\Microsoft\GameBar",
                Key = "AutoGameModeEnabled",
                EnabledValue = 1,
                DisabledValue = 0
            }
        ];
    }
}
