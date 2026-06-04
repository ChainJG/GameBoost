using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.Windows.Privacy_Security
{
    public class FeedbackRequestsModule : SystemTweakModuleBase
    {
        public override string Name => "Feedback Requests";

        public override object? RecommendedValue => ToggleType.Disabled;
        public override string RecommendationReason => "Feedback Request is recommended to be disabled because it prevents Windows from asking the user for feedback and reduces unnecessary background prompts";

        public override RegistryEditInfo[] RegistryEdits =>
        [
            new()
            {
                Hive = RegistryHive.CurrentUser,
                Path = @"Software\Microsoft\Siuf\Rules",
                Key = "NumberOfSIUFInPeriod",
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
