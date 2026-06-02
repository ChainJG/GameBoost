using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.Windows.DirectXUserGlobal
{
    public sealed class GpuPreferencesGameModule(RegistryResult registry) : SystemTweakModuleBase
    {
        public override string Name => registry.Message;

        public override RegistryEditInfo[] RegistryEdits => 
        [
            new () 
            {
                Hive = RegistryHive.CurrentUser,
                Path = @"Software\Microsoft\DirectX\UserGpuPreferences",
                Key = registry.Value as string ?? string.Empty,
                EnabledValue = "GpuPreference=2;SwapEffectUpgradeEnable=1;",
                DisabledValue = "",
            }
        ];

        protected override string FormatStatus(ToggleType status)
        {
            return status switch
            {
                ToggleType.Enabled => "High Performance",
                ToggleType.Disabled => "Automatic",
                _ => "Automatic"
            };
        }
    }
}
