using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.Windows.DirectXUserGlobal
{
    public sealed class GpuPreferencesGameModule(RegistryResult registry) : SystemTweakModuleBase
    {
        public override string Name => registry.Message;

        public override object? RecommendedValue => ToggleType.Enabled;
        public override string RecommendationReason => $"GPU Preference is recommended to be set to High Performance for {registry.Message} because it forces the to use the stronger GPU, which can improve FPS, stability, and rendering performance";

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
