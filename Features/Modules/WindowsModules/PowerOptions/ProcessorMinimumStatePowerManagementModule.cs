using GameBoost.Features.Modules.WindowsModules.PowerOptions.Base;
using GameBoost.Infrastructure.Power;

namespace GameBoost.Features.Modules.WindowsModules.PowerOptions
{
    public sealed class ProcessorMinimumStatePowerManagementModule : PowerCfgPercentageSliderModuleBase
    {
        protected override PowerCfgSettingDefinition Setting { get; } = new()
        {
            Name = "Processor Minimum State",

            SubGroupAlias = "SUB_PROCESSOR",
            SettingAlias = "PROCTHROTTLEMIN",

            SubGroupGuid = "54533251-82be-4824-96c1-47b60b740d00",
            SettingGuid = "893dee8e-2bef-41e0-89c6-b55d0929964c",

            RecommendedAcValue = 100,
            CheckDcValue = false
        };

        protected override string RecommendedReason =>
            "Processor Minimum State can be set higher on plugged-in gaming desktops to reduce aggressive CPU downclocking, but it may increase heat, fan noise, and power usage.";
    }
}