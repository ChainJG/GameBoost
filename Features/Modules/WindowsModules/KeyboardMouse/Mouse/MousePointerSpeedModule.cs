using GameBoost.Features.Modules.WindowsModules.KeyboardMouse.Base;
using GameBoost.Infrastructure.Registry;
using GameBoost.Infrastructure.UserInput;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoost.Features.Modules.WindowsModules.KeyboardMouse.Mouse
{
    public sealed class MousePointerSpeedModule : UserInputSliderModuleBase
    {
        public override string Name => "Mouse Pointer Speed";

        protected override string RegistryPath => RegistryConstants.MousePath;

        protected override string RegistryKey => "MouseSensitivity";

        protected override int MinimumValue => 1;

        protected override int MaximumValue => 20;

        protected override int DefaultValue => 10;

        protected override int RecommendedSliderValue => 10;

        public override string RecommendationReason =>
            "Mouse pointer speed is recommended to stay at 10 for a neutral Windows pointer speed, especially for users who want consistent mouse movement with Enhance Pointer Precision disabled.";

        protected override void ApplyLiveValue(int value)
        {
            UserInputNativeMethods.SetMousePointerSpeed(value);
        }

        protected override string FormatStatus(int value)
        {
            return $"{value}/20";
        }
    }
}