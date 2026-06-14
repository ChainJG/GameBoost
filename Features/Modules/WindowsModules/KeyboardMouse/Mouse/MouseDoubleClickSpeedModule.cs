using GameBoost.Features.Modules.WindowsModules.KeyboardMouse.Base;
using GameBoost.Infrastructure.Registry;
using GameBoost.Infrastructure.UserInput;

namespace GameBoost.Features.Modules.WindowsModules.KeyboardMouse.Mouse
{
    public sealed class MouseDoubleClickSpeedModule : UserInputSliderModuleBase
    {
        public override string Name => "Mouse Double-Click Speed";

        protected override string RegistryPath => RegistryConstants.MousePath;

        protected override string RegistryKey => "DoubleClickSpeed";

        protected override int MinimumValue => 200;

        protected override int MaximumValue => 900;

        protected override int DefaultValue => 500;

        protected override int RecommendedSliderValue => 500;

        protected override string ValueSuffix => "ms";

        public override string RecommendationReason =>
            "Mouse double-click speed is recommended to stay at the Windows default of 500ms for a balanced double-click window that works well for most users.";

        protected override void ApplyLiveValue(int value)
        {
            UserInputNativeMethods.SetDoubleClickSpeed(value);
        }

        protected override string FormatStatus(int value)
        {
            return $"{value}ms";
        }
    }
}