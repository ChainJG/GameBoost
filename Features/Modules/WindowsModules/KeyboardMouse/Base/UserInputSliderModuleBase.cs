using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.Registry;
using GameBoost.Shared.Results;
using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;

namespace GameBoost.Features.Modules.WindowsModules.KeyboardMouse.Base
{
    public abstract class UserInputSliderModuleBase : IInputActionModule<double>, IRecommendedActionModule, IRequiredModule
    {
        public abstract string Name { get; }

        #region Registry Edit
        protected abstract string RegistryPath { get; }
        protected abstract string RegistryKey { get; }
        #endregion

        #region Slider Values
        protected abstract int MinimumValue { get; }
        protected abstract int MaximumValue { get; }
        protected abstract int DefaultValue { get; }

        protected abstract int RecommendedSliderValue { get; }
        protected virtual string ValueSuffix => string.Empty;
        #endregion

        #region IRequiredModule
        public virtual bool Admin => false;
        public virtual bool SystemReboot => false;
        #endregion

        #region IRecommendedActionModule
        public virtual RecommendationPriority RecommendationPriority => RecommendationPriority.None;
        public object? RecommendedValue => RecommendedSliderValue;
        public abstract string RecommendationReason { get; }
        public bool IsRecommendedValue(object? currentValue) =>
             TryParseInt(currentValue, out var value) &&
                   value == RecommendedSliderValue;
        #endregion

        protected virtual string FormatStatus(int value) =>  $"{value}{ValueSuffix}";
        public Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var value = ReadCurrentValue();

            return Task.FromResult(
                ActionRefreshResult.ValueOnly(
                    value,
                    FormatStatus(value)));
        }

        public async Task<ModuleResult> ExecuteAsync(double input, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var value = Math.Clamp(
                    Convert.ToInt32(Math.Round(input)),
                    MinimumValue,
                    MaximumValue);

                var result = RegistryHelper.SetValue(
                    CreateEdit(),
                    value.ToString(CultureInfo.InvariantCulture));

                if (!result.Success)
                    return ModuleResult.Failed(result.Message);

                ApplyLiveValue(value);

                return ModuleResult.Successful($"{Name} was set to {FormatStatus(value)}");
            }
            catch (OperationCanceledException)
            {
                return ModuleResult.Failed($"{Name} change was cancelled.");
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Failed to set {Name}: {ex.Message}");
#endif
                return ModuleResult.Failed(ex.Message);
            }
        }

        public Task<ModuleResult> ExecuteRecommendedAsync(CancellationToken token) => ExecuteAsync(RecommendedSliderValue, token);


        protected abstract void ApplyLiveValue(int value);

        private int ReadCurrentValue()
        {
            var result = RegistryHelper.GetValue(CreateEdit());

            if (!result.Success)
                return DefaultValue;

            return TryParseInt(result.Value, out var value)
                ? Math.Clamp(value, MinimumValue, MaximumValue)
                : DefaultValue;
        }

        private RegistryEditInfo CreateEdit()
        {
            return new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryPath,
                Key = RegistryKey
            };
        }

        private static bool TryParseInt(object? value, out int result)
        {
            if (value is int intValue)
            {
                result = intValue;
                return true;
            }

            return int.TryParse(
                value?.ToString(),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out result);
        }
    }
}