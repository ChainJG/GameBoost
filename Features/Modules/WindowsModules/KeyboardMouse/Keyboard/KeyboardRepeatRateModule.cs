using GameBoost.Features.Modules.WindowsModules.KeyboardMouse.Base;
using GameBoost.Infrastructure.Registry;
using GameBoost.Infrastructure.UserInput;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoost.Features.Modules.WindowsModules.KeyboardMouse.Keyboard
{
    public sealed class KeyboardRepeatRateModule : UserInputSliderModuleBase
    {
        public override string Name => "Keyboard Character Repeat Rate";

        protected override string RegistryPath => RegistryConstants.KeyboardPath;

        protected override string RegistryKey => "KeyboardSpeed";

        protected override int MinimumValue => 0;

        protected override int MaximumValue => 31;

        protected override int DefaultValue => 31;

        protected override int RecommendedSliderValue => 31;

        public override string RecommendationReason =>
            "Keyboard character repeat rate is recommended to be set high for faster key-repeat behaviour when holding a key down.";

        protected override void ApplyLiveValue(int value)
        {
            UserInputNativeMethods.SetKeyboardSpeed(value);
        }

        protected override string FormatStatus(int value)
        {
            return $"{value}/31";
        }
    }
}