using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using GameBoost.Infrastructure.Services;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.Windows.Privacy_Security
{
    public class BiometricsModule : SystemTweakModuleBase
    {
        public override string Name => "Biometrics";

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
