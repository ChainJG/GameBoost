using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.WindowsModules.Gaming
{
    public class GameModeModule : SystemTweakModuleBase
    {
        public override string Name =>
            "Game Mode";

        public override RecommendationPriority RecommendationPriority => RecommendationPriority.High;
        public override object? RecommendedValue => ToggleType.Enabled;
        public override string RecommendationReason => "Game Mode is recommended to be enabled for gaming focused systems";

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
