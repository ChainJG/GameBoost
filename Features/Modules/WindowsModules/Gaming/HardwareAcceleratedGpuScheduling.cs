using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.WindowsModules.Gaming
{
    public class HardwareAcceleratedGpuScheduling : SystemTweakModuleBase
    {
        public override string Name => "Hardware Accelerated GPU Scheduling";

        public override bool Admin => true;

        public override RecommendationPriority RecommendationPriority => RecommendationPriority.Medium;
        public override object? RecommendedValue => ToggleType.Enabled;
        public override string RecommendationReason => "Recommended for supported gaming PCs because it can reduce GPU scheduling overhead and improve responsiveness";

        public override RegistryEditInfo[] RegistryEdits => 
        [
            new () 
            {
                Hive = RegistryHive.LocalMachine,
                Path = RegistryConstants.GraphicsDriversPath,
                Key = "HwSchMode",
                EnabledValue = 2,
                DisabledValue = 1
            }
        ];
    }
}
