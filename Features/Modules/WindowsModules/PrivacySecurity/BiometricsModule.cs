using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using GameBoost.Infrastructure.Services;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.WindowsModules.PrivacySecurity
{
    public class BiometricsModule : SystemTweakModuleBase
    {
        public override string Name => "Biometrics";

        public override bool Admin => true;
        public override RecommendationPriority RecommendationPriority => RecommendationPriority.High;
        public override object? RecommendedValue => ToggleType.Disabled;
        public override string RecommendationReason => "Biometrics is recommended to be disabled for privacy-focused systems because it reduces biometric sign-in data usage and removes an unnecessary authentication feature for users who do not use Windows Hello";

        public override ServiceEditInfo[] ServiceEdits =>
        [
            new()
            {
                Name = "WbioSrvc",
                DisplayName = "Windows Biometric Service",
                Description = "The Windows biometric service gives client applications the ability to capture, compare, manipulate, and store biometric data without gaining direct access to any biometric hardware or samples",
                RequiresAdmin = true
            },
        ];
    }
}
