using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using GameBoost.Infrastructure.Services;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.WindowsModules.PrivacySecurity
{
    public class ErrorReportingModule : SystemTweakModuleBase
    {
        public override string Name => "Error Reporting";

        public override RecommendationPriority RecommendationPriority => RecommendationPriority.High;
        public override object? RecommendedValue => ToggleType.Disabled;
        public override string RecommendationReason => "Error Reporting is recommended to be disabled because it reduces background diagnostic uploads and prevents Windows from sending crash/report data to Microsoft";

        public override RegistryEditInfo[] RegistryEdits =>
        [
            new()
            {
                Hive = RegistryHive.LocalMachine,
                Path = @"Software\Microsoft\Windows\Windows Error Reporting",
                Key = "Disabled",
                Kind = RegistryValueKind.DWord,
                EnabledValue = 1,
                DisabledValue = 0
            }
        ];

        public override ServiceEditInfo[] ServiceEdits =>
        [
            new()
            {
                Name = "WerSvc",
                RequiresAdmin = true
            },
        ];
    }
}
