using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.Power;
using GameBoost.Infrastructure.Shell;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoost.Features.Modules.WindowsModules.PowerOptions.Base
{
    public abstract class PowerCfgPercentageSliderModuleBase : IInputActionModule<double>, IRecommendedActionModule, IRequiredModule
    {
        protected abstract PowerCfgSettingDefinition Setting { get; }
        protected abstract string RecommendedReason  { get; }

        protected virtual bool ApplyDcValue => false;
        protected virtual int RecommendedValuePercentage => Setting.RecommendedAcValue;
        public string Name => Setting.Name;

        #region IRequiredModule
        public bool SystemReboot => false;
        public bool Admin => true;
        #endregion

        #region IRecommendedActionModule
        public RecommendationPriority RecommendationPriority => RecommendationPriority.Low;
        public bool IsRecommendedValue(object? currentValue)
        {
            if (currentValue is null)
                return false;

            if (!double.TryParse(currentValue.ToString(), out double currentValueAsDouble))
                return false;

            return Math.Abs(currentValueAsDouble - RecommendedValuePercentage) < 0.5;
        }
        public object? RecommendedValue => RecommendedValuePercentage;
        public string RecommendationReason => RecommendedReason;
        #endregion

        public async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var status = await PowerCfgStatusHelper.GetStatusAsync(Setting, token);

            var currentValue = status.CurrentAcValue ?? Setting.RecommendedAcValue;

            return ActionRefreshResult.ValueOnly(currentValue, currentValue.ToString());
        }

        public async Task<ModuleResult> ExecuteAsync(double input, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var value = MathHelper.ClampToPercentage(input);

            var currnetStatus = await PowerCfgStatusHelper.GetStatusAsync(Setting, token);

            if (currnetStatus.CurrentAcValue == value && !ApplyDcValue || currnetStatus.CurrentDcValue == value) 
                return ModuleResult.Successful($"{Name} is already set to {value}%");


            var command = BuildSetCommand(value);

            var result = await ShellService.RunAsync(ShellType.Cmd, command, token);

            if (!result.Success)
                return ModuleResult.Failed(result.Error);

            return ModuleResult.Successful($"{Name} set to {value}%");
        }

        public Task<ModuleResult> ExecuteRecommendedAsync(CancellationToken token)
        {
            return ExecuteAsync(RecommendedValuePercentage, token);
        }

        private string BuildSetCommand(int value)
        {
            var command =
                $"powercfg /setacvalueindex SCHEME_CURRENT " +
                $"{Setting.SubGroupAlias} " +
                $"{Setting.SettingAlias} " +
                $"{value}";

            if (ApplyDcValue)
            {
                command +=
                    $" && powercfg /setdcvalueindex SCHEME_CURRENT " +
                    $"{Setting.SubGroupAlias} " +
                    $"{Setting.SettingAlias} " +
                    $"{value}";
            }

            command += " && powercfg /setactive SCHEME_CURRENT";

            return command;
        }
    }
}
