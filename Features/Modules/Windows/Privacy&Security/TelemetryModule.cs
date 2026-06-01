using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using GameBoost.Infrastructure.Services;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.Windows.Privacy_Security
{
    public class TelemetryModule : SystemTweakModuleBase
    {
        public override string Name => "Temlemetry";

        public override RegistryEditInfo[] RegistryEdits =>
        [
            new()
            {
                Hive = RegistryHive.LocalMachine,
                Path = RegistryConstants.DataCollectionPath,
                Key = "AllowTelemetry",
                EnabledValue = 1,
                DisabledValue = 0
            }
        ];

        public override ServiceEditInfo[] ServiceEdits =>
        [
            new() 
            {
                Name = "dmwappushservice",
                DisplayName = "Device Management WAP Push Service",
                Description = "Handles device management push messages and telemetry-related communication",
                RequiresAdmin = true
            },
            new() 
            {
                Name = "DiagTrack",
                DisplayName = "Connected User Experiences and Telemetry",
                Description = "Collects and sends diagnostic and usage data to Microsoft",
                RequiresAdmin = true
            }
        ];
    }
}
