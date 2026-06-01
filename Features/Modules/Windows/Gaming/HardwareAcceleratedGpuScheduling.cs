using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.Windows.Gaming
{
    public class HardwareAcceleratedGpuScheduling : SystemTweakModuleBase
    {
        public override string Name => "Hardware Accelerated GPU Scheduling";

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
