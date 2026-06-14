using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using GameBoost.Infrastructure.UserInput;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.WindowsModules.KeyboardMouse.Mouse
{
    public sealed class EnhancePointerPrecisionModule : SystemTweakModuleBase
    {
        public override string Name => "Enhance Pointer Precision";

        #region IRequiredModule
        public override bool Admin => false;
        public override bool SystemReboot => false;
        #endregion

        #region IRecommendedModule
        public override RecommendationPriority RecommendationPriority => RecommendationPriority.High;
        public override object? RecommendedValue => ToggleType.Disabled;
        public override string RecommendationReason =>
            "Enhance Pointer Precision is recommended to be disabled for consistent mouse movement, especially for gaming-focused users who want predictable pointer behaviour";
        #endregion

        public override RegistryEditInfo[] RegistryEdits =>
        [
            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.MousePath,
                Key = "MouseSpeed",
                Kind = RegistryValueKind.String,
                EnabledValue = "1",
                DisabledValue = "0"
            },
            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.MousePath,
                Key = "MouseThreshold1",
                Kind = RegistryValueKind.String,
                EnabledValue = "6",
                DisabledValue = "0"
            },
            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.MousePath,
                Key = "MouseThreshold2",
                Kind = RegistryValueKind.String,
                EnabledValue = "10",
                DisabledValue = "0"
            }
        ];


        protected override void ApplyLiveValue(ToggleType status)
        {
            UserInputNativeMethods.SetEnhancePointerPrecision(status == ToggleType.Enabled);
        }
    }
}