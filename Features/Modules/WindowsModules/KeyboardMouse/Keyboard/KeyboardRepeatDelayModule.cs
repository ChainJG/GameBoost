using GameBoost.Features.Modules.WindowsModules.KeyboardMouse.Base;
using GameBoost.Infrastructure.Registry;
using GameBoost.Infrastructure.UserInput;

namespace GameBoost.Features.Modules.WindowsModules.KeyboardMouse.Keyboard
{
    public sealed class KeyboardRepeatDelayModule : UserInputSliderModuleBase
    {
        public override string Name => "Keyboard Character Repeat Delay";

        protected override string RegistryPath => RegistryConstants.KeyboardPath;

        protected override string RegistryKey => "KeyboardDelay";

        protected override int MinimumValue => 0;

        protected override int MaximumValue => 3;

        protected override int DefaultValue => 1;

        protected override int RecommendedSliderValue => 0;

        public override string RecommendationReason =>
            "Keyboard character repeat delay is recommended to be set to the shortest delay for a more responsive typing and gaming-focused keyboard feel.";

        protected override void ApplyLiveValue(int value)
        {
            UserInputNativeMethods.SetKeyboardDelay(value);
        }

        protected override string FormatStatus(int value)
        {
            return value switch
            {
                0 => "Shortest delay",
                1 => "Short delay",
                2 => "Long delay",
                3 => "Longest delay",
                _ => $"{value}"
            };
        }
    }
}