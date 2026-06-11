using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.WindowsModules.PrivacySecurity
{
    public class FeedbackRequestsModule : SystemTweakModuleBase
    {
        public override string Name => "Feedback Requests";

        public override RecommendationPriority RecommendationPriority => RecommendationPriority.High;
        public override object? RecommendedValue => ToggleType.Disabled;
        public override string RecommendationReason => "Feedback Request is recommended to be disabled because it prevents Windows from asking the user for feedback and reduces unnecessary background prompts";

        public override RegistryEditInfo[] RegistryEdits =>
        [
            new()
            {
                Hive = RegistryHive.CurrentUser,
                Path = @"Software\Microsoft\Siuf\Rules",
                Key = "NumberOfSIUFInPeriod",
                Kind = RegistryValueKind.DWord,
                EnabledAction = RegistryValueAction.Delete,
                DisabledValue = 0
            },
            new()
            {
                Hive = RegistryHive.CurrentUser,
                Path = @"Software\Microsoft\Siuf\Rules",
                Key = "PeriodInNanoSeconds",
                EnabledAction = RegistryValueAction.Delete,
                DisabledAction = RegistryValueAction.Delete
            },
        ];
    }
}
