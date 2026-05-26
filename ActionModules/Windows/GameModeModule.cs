using GameBoost.Core.Abstact;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.ActionModules.Windows
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
