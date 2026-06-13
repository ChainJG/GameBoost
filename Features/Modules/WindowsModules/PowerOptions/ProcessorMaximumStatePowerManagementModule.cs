using GameBoost.Features.Modules.WindowsModules.PowerOptions.Base;
using GameBoost.Infrastructure.Power;

namespace GameBoost.Features.Modules.WindowsModules.PowerOptions
{
    public sealed class ProcessorMaximumStatePowerManagementModule : PowerCfgPercentageSliderModuleBase
    {
        protected override PowerCfgSettingDefinition Setting { get; } = new()
        {
            Name = "Processor Maximum State",

            SubGroupAlias = "SUB_PROCESSOR",
            SettingAlias = "PROCTHROTTLEMAX",

            SubGroupGuid = "54533251-82be-4824-96c1-47b60b740d00",
            SettingGuid = "bc5038f7-23e0-4960-96da-33abaf5935ec",

            RecommendedAcValue = 100,
            CheckDcValue = false
        };

    protected override string RecommendedReason =>
        "Processor Maximum State is recommended to be set to 100% for gaming-focused systems because it prevents the active power plan from artificially limiting CPU performance.";
}
}