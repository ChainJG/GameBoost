using GameBoost.Features.Modules.WindowsModules.KeyboardMouse.Base;
using GameBoost.Infrastructure.Registry;
using GameBoost.Infrastructure.UserInput;

namespace GameBoost.Features.Modules.WindowsModules.KeyboardMouse.Mouse
{
    public sealed class MouseWheelScrollLinesModule : UserInputSliderModuleBase
    {
        public override string Name => "Mouse Wheel Scroll Lines";

        protected override string RegistryPath => RegistryConstants.DesktopPath;
        protected override string RegistryKey => "WheelScrollLines";

        protected override int MinimumValue => 1;
        protected override int MaximumValue => 20;
        protected override int DefaultValue => 3;

        protected override int RecommendedSliderValue => 3;
        public override string RecommendationReason =>
            "Mouse wheel scroll lines is recommended to stay around 3 lines for a balanced scrolling experience across browsers, documents, and Windows apps.";

        protected override void ApplyLiveValue(int value)
        {
            UserInputNativeMethods.SetWheelScrollLines(value);
        }

        protected override string FormatStatus(int value)
        {
            return $"{value} lines";
        }
    }
}