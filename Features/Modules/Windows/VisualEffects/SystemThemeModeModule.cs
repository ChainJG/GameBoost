using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.Windows.VisualEffects
{
    internal class SystemThemeModeModule : SystemTweakModuleBase
    {
        public override string Name =>
            "System Theme Mode";

        public override RegistryEditInfo[] RegistryEdits =>
        [
            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.Personalize,
                Key = "SystemUsesLightTheme",
                EnabledValue = 1,   // Light
                DisabledValue = 0   // Dark
            },
            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.Personalize,
                Key = "AppsUseLightTheme",
                EnabledValue = 1,   // Light
                DisabledValue = 0   // Dark
            },
        ];

        protected override string FormatStatus(ToggleType status)
        {
            return status switch
            {
                ToggleType.Enabled => "Light",
                ToggleType.Disabled => "Dark",
                _ => "Unknown"
            };
        }
    }
}
