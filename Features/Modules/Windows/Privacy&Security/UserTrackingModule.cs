using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.Windows.Privacy_Security
{
    public class UserTrackingModule : SystemTweakModuleBase
    {
        public override string Name => "User Tracking";

        public override object? RecommendedValue => ToggleType.Disabled;
        public override string RecommendationReason => "User Tracking is recommended to be disabled because it reduces activity-based personalization, advertising tracking, and background data collection";

        public override RegistryEditInfo[] RegistryEdits =>
        [
            new()
            {
                Hive = RegistryHive.CurrentUser,
                Path = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced",
                Key = "Start_TrackProgs",
                EnabledValue = 1,
                DisabledValue = 0
            },
            new()
            {
                Hive = RegistryHive.CurrentUser,
                Path = @"Software\Microsoft\Input\TIPC",
                Key = "Enabled",
                EnabledValue = 1,
                DisabledValue = 0
            },
            new()
            {
                Hive = RegistryHive.CurrentUser,
                Path = @"Software\Microsoft\Windows\CurrentVersion\Privacy",
                Key = "TailoredExperiencesWithDiagnosticDataEnabled",
                EnabledValue = 1,
                DisabledValue = 0
            },
            new()
            {
                Hive = RegistryHive.CurrentUser,
                Path = @"Control Panel\International\User Profile",
                Key = "HttpAcceptLanguageOptOut",
                EnabledValue = 0,
                DisabledValue = 1
            }, // Allow websites to access your language list
            new ()
            {
                Hive = RegistryHive.CurrentUser,
                Path = @"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo",
                Key = "Enabled",
                EnabledValue = 1,
                DisabledValue = 0
            }, // Let apps show me personalised ads by using my advertising ID
        ];
    }
}
