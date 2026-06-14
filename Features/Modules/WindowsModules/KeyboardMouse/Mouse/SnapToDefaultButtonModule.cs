using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.WindowsModules.KeyboardMouse.Mouse
{
    public sealed class SnapToDefaultButtonModule : SystemTweakModuleBase
    {
        public override string Name => "Snap To Default Button";

        public override RegistryEditInfo[] RegistryEdits =>
        [
            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.MousePath,
                Key = "SnapToDefaultButton",
                Kind = RegistryValueKind.String,
                EnabledValue = "1",
                DisabledValue = "0"
            }
        ];

        public override object? RecommendedValue => ToggleType.Disabled;
        public override string RecommendationReason =>
            "Snap To Default Button is recommended to be disabled so the pointer does not automatically jump to dialog buttons, keeping mouse movement predictable.";
    }
}