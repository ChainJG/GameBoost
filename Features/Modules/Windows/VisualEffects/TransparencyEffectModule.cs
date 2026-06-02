using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoost.Features.Modules.Windows.VisualEffects
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
                EnabledValue = 1,
                DisabledValue = 0
            }
        ];
    }
}
