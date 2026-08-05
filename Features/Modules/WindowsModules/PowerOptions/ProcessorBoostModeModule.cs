using GameBoost.Core.Interfaces;
using GameBoost.Features.Modules.WindowsModules.PowerOptions.Options;
using GameBoost.Infrastructure.Power;
using GameBoost.Infrastructure.Shell;
using GameBoost.Core.Modules;
using GameBoost.Shared.Results;

namespace GameBoost.Features.Modules.WindowsModules.PowerOptions
{
    public sealed class ProcessorBoostModeModule : IInputActionModule<object>, IRecommendedActionModule, IRequiredModule
    {
        public string Name => "Processor Boost Mode";

        #region IRequiredModule
        public bool Admin => true;
        public bool SystemReboot => false;
        #endregion

        #region IRecommendedActionModule
        public RecommendationPriority RecommendationPriority => RecommendationPriority.Medium;
        public object? RecommendedValue => ProcessorBoostModeOption.Aggressive;
        public string RecommendationReason =>
           "Processor Boost Mode is recommended to be set to Aggressive for gaming focused desktop systems because it allows the CPU to boost more responsively under load, improving peak CPU performance. This may increase heat, fan noise, and power usage.";

        public bool IsRecommendedValue(object? currentValue) => 
            currentValue is ProcessorBoostModeOption option &&
                   option.Value == ProcessorBoostModeOption.Aggressive.Value;
        #endregion

        private const string ProcessorSubGroupGuid = "54533251-82be-4824-96c1-47b60b740d00";
        private const string ProcessorBoostModeGuid = "be337238-0d82-4146-a960-4f3749d470c7";

        private static readonly PowerCfgSettingDefinition Setting = new()
        {
            Name = "Processor Boost Mode",

            SubGroupAlias = "SUB_PROCESSOR",
            SettingAlias = "PERFBOOSTMODE",

            SubGroupGuid = "54533251-82be-4824-96c1-47b60b740d00",
            SettingGuid = "be337238-0d82-4146-a960-4f3749d470c7",

            RecommendedAcValue = 2,
            CheckDcValue = false
        };

        private static readonly IReadOnlyList<ProcessorBoostModeOption> Options =
        [
            ProcessorBoostModeOption.Disabled,
            ProcessorBoostModeOption.Enabled,
            ProcessorBoostModeOption.Aggressive,
            ProcessorBoostModeOption.EfficientEnabled,
            ProcessorBoostModeOption.EfficientAggressive,
            ProcessorBoostModeOption.AggressiveAtGuaranteed,
            ProcessorBoostModeOption.EfficientAggressiveAtGuaranteed
        ];

        public async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            await EnsureProcessorBoostModeVisibleAsync(token);

            var status = await PowerCfgStatusHelper.GetStatusAsync(
                Setting,
                token);

            var selectedOption = GetOptionFromValue(
                status.CurrentAcValue);

            return ActionRefreshResult.OptionsAndValue(
                CreateActionOptions(selectedOption),
                selectedOption,
                selectedOption.Name);
        }

        public Task<ModuleResult> ExecuteRecommendedAsync(CancellationToken token) =>
            ExecuteAsync(ProcessorBoostModeOption.Aggressive, token);
        public async Task<ModuleResult> ExecuteAsync(object input, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var selectedOption = GetSelectedOption(input);

                if (selectedOption == ProcessorBoostModeOption.Unknown)
                    return ModuleResult.Failed("Invalid Processor Boost Mode option selected");

                var currentStatus = await PowerCfgStatusHelper.GetStatusAsync(Setting, token);

                if (currentStatus.CurrentAcValue == selectedOption.Value)
                    return ModuleResult.Successful($"{Name} is already set to {selectedOption.Name}");

                var command =
                    $"powercfg /setacvalueindex SCHEME_CURRENT " +
                    $"{Setting.SubGroupAlias} " +
                    $"{Setting.SettingAlias} " +
                    $"{selectedOption.Value} && " +
                    "powercfg /setactive SCHEME_CURRENT";

                var result = await ShellService.RunAsync(ShellType.Cmd, command, token);

                if (!result.Success)
                    return ModuleResult.Failed(result.Error);

                return ModuleResult.Successful($"{Name} was set to {selectedOption.Name} successfully");
            }
            catch (OperationCanceledException)
            {
                return ModuleResult.Failed($"{Name} change was cancelled");
            }
            catch (Exception ex)
            {
                return ModuleResult.Failed(ex.Message);
            }
        }

        private static Task<ModuleResult> EnsureProcessorBoostModeVisibleAsync(CancellationToken token) =>
            PowerCfgAttributeHelper.ShowSettingAsync(
                ProcessorSubGroupGuid,
                ProcessorBoostModeGuid,
                token);

        private static List<ActionOption> CreateActionOptions(ProcessorBoostModeOption selectedOption) =>
            [.. Options
               .Select(option => new ActionOption
               {
                   DisplayText = option.Name,
                   Value = option,
                   Description = option.Description,
                   IsDefaultSelected = option.Value == selectedOption.Value
               })];

        private static ProcessorBoostModeOption GetSelectedOption(object? input)
        {
            if (input is ProcessorBoostModeOption option)
                return option;

            if (input is int intValue)
                return GetOptionFromValue(intValue);

            if (int.TryParse(input?.ToString(), out var parsedValue))
                return GetOptionFromValue(parsedValue);

            var text = input?.ToString();

            return Options.FirstOrDefault(option =>
                       string.Equals(
                           option.Name,
                           text,
                           StringComparison.OrdinalIgnoreCase))
                   ?? ProcessorBoostModeOption.Unknown;
        }

        private static ProcessorBoostModeOption GetOptionFromValue(int? value) =>
            Options.FirstOrDefault(option => option.Value == value)
                   ?? ProcessorBoostModeOption.Unknown;
    }
}
