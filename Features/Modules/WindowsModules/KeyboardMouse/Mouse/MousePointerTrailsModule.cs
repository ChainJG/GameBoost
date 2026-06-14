using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using GameBoost.Infrastructure.UserInput;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.WindowsModules.KeyboardMouse.Mouse
{
    public sealed class MousePointerTrailsModule : SystemTweakModuleBase
    {
        public override string Name => "Mouse Pointer Trails";

        public override RegistryEditInfo[] RegistryEdits =>
        [
            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.MousePath,
                Key = "MouseTrails",
                Kind = RegistryValueKind.String,
                EnabledValue = "1",
                DisabledValue = "0"
            }
        ];

        public override object? RecommendedValue => ToggleType.Disabled;
        public override string RecommendationReason =>
            "Mouse pointer trails are recommended to be disabled for cleaner pointer visibility and reduced visual distraction";

        protected override void ApplyLiveValue(ToggleType status)
        {
            UserInputNativeMethods.SetMouseTrails(
                status == ToggleType.Enabled ? 7 : 0);
        }
    }
}