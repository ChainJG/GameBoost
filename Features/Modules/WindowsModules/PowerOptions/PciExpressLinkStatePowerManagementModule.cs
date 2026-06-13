using GameBoost.Core.Interfaces;
using GameBoost.Features.Modules.WindowsModules.PowerOptions.Options;
using GameBoost.Infrastructure.Power;
using GameBoost.Infrastructure.Shell;
using GameBoost.MVVM.ViewModels.Shared.Selection.Actions.Misc;
using GameBoost.Shared.Results;
using System.Diagnostics;

namespace GameBoost.Features.Modules.WindowsModules.PowerOptions
{
    public sealed class PciExpressLinkStatePowerManagementModule : IInputActionModule<object>, IRecommendedActionModule, IRequiredModule
    {
        public string Name => "PCIe Link State Power Management";

        #region IRequiredModule
        public bool Admin => true;
        public bool SystemReboot => false;
        #endregion

        #region IRecommendedActionModule
        public RecommendationPriority RecommendationPriority => RecommendationPriority.Low;
        public object? RecommendedValue => PciExpressLinkStateOption.Off;
        public string RecommendationReason =>
            "PCIe Link State Power Management is recommended to be set to Off for gaming-focused systems because it reduces PCIe power-saving behaviour and helps keep PCIe devices more responsive.";
        public bool IsRecommendedValue(object? currentValue)
        {
            return currentValue is PciExpressLinkStateOption option && 
                option.Value == PciExpressLinkStateOption.Off.Value;
        }
        #endregion

        private static readonly PowerCfgSettingDefinition Setting = new()
        {
            Name = "PCIe Link State Power Management",

            SubGroupAlias = "SUB_PCIEXPRESS",
            SettingAlias = "ASPM",

            SubGroupGuid = "501a4d13-42af-4429-9fd1-a8218c268e20",
            SettingGuid = "ee12f906-d277-404b-b6da-e5fa1a576df5",

            RecommendedAcValue = 0,
            CheckDcValue = false
        };

        private static readonly IReadOnlyList<PciExpressLinkStateOption> Options =
        [
            PciExpressLinkStateOption.Off,
            PciExpressLinkStateOption.ModeratePowerSavings,
            PciExpressLinkStateOption.MaximumPowerSavings,
        ];

        public async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var status = await PowerCfgStatusHelper.GetStatusAsync(Setting, token);

            var selectedOption = Options.FirstOrDefault(
                option => option.Value == status.CurrentAcValue);

            selectedOption ??= PciExpressLinkStateOption.Unknown;

            var options = Options
                .Select(option => new ActionOptionViewModel<object>
                {
                    DisplayText = $"{option.Name}",
                    Value = option,
                    IsDefaultSelected = option.Value == status.CurrentAcValue,
                })
                .ToList();

            return ActionRefreshResult.OptionsAndValue(options, selectedOption, selectedOption.Name);
        }

        public async  Task<ModuleResult> ExecuteAsync(object input, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (input is not PciExpressLinkStateOption option || option == PciExpressLinkStateOption.Unknown)
                return ModuleResult.Failed("Invalid PCIe Link State Power Management option selected");

            var command =
                $"powercfg /setacvalueindex SCHEME_CURRENT SUB_PCIEXPRESS ASPM {option.Value} && " +
                "powercfg /setactive SCHEME_CURRENT";

            var result = await ShellService.RunAsync(ShellType.Cmd, command, token);

            if (!result.Success)
                return ModuleResult.Failed(result.Error);

            return ModuleResult.Successful($"{Name} set to {option.Name}");
        }

        public Task<ModuleResult> ExecuteRecommendedAsync(CancellationToken token)
        {
            return ExecuteAsync(PciExpressLinkStateOption.Off, token);
        }

    }
}
