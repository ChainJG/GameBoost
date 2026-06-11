using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.WindowsModules.VisualEffects
{
    public sealed class TransparencyEffectModule : SystemTweakModuleBase
    {
        public override string Name => "Transparency Effect";

        public override RegistryEditInfo[] RegistryEdits =>
        [
            new () 
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.Personalize,
                Key = "EnableTransparency",
                Kind = RegistryValueKind.DWord,
                EnabledValue = 1,
                DisabledValue = 0
            }
        ];
    }
}
