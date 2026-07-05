using GameBoost.Core.Interfaces;
using GameBoost.Shared.Results;

namespace GameBoost.Features.Modules.Base
{
    public abstract class InputActionModuleBase<TInput> :
        IInputActionModule<TInput>,
        IRecommendedActionModule,
        IRequiredModule
    {
        // IInputActionModule
        public abstract string Name { get; }

        // IRequiredModule
        public virtual bool SystemReboot => false;
        public virtual bool Admin => false;

        // IRecommendedActionModule
        public virtual RecommendationPriority RecommendationPriority =>
            RecommendationPriority.None;

        public virtual object? RecommendedValue =>
            null;

        public virtual string RecommendationReason =>
            RecommendedValue is null
                ? string.Empty
                : $"{Name} is recommended to be {RecommendedValue}";

        public virtual bool IsRecommendedValue(object? currentValue)
        {
            if (RecommendedValue is null)
                return false;

            return Equals(currentValue, RecommendedValue);
        }

        public virtual Task<ModuleResult> ExecuteRecommendedAsync(
            CancellationToken token)
        {
            if (RecommendedValue is null)
                return Task.FromResult(
                    ModuleResult.Failed($"{Name} does not have a recommended value"));

            if (RecommendedValue is TInput typedValue)
                return ExecuteAsync(typedValue, token);

            if (TryConvertRecommendedValue(RecommendedValue, out var convertedValue))
                return ExecuteAsync(convertedValue, token);

            return Task.FromResult(
                ModuleResult.Failed($"Invalid recommended value for {Name}"));
        }

        // IInputActionModule
        public abstract Task<ActionRefreshResult> RefreshStatusAsync(
            CancellationToken token);

        public abstract Task<ModuleResult> ExecuteAsync(
            TInput input,
            CancellationToken token);

        protected virtual bool TryConvertRecommendedValue(
            object value,
            out TInput convertedValue)
        {
            try
            {
                if (typeof(TInput).IsEnum &&
                    value is string enumText &&
                    Enum.TryParse(typeof(TInput), enumText, ignoreCase: true, out var enumValue))
                {
                    convertedValue = (TInput)enumValue;
                    return true;
                }

                var converted = Convert.ChangeType(
                    value,
                    typeof(TInput));

                if (converted is TInput typedConverted)
                {
                    convertedValue = typedConverted;
                    return true;
                }
            }
            catch
            {
                // Ignore conversion failure.
            }

            convertedValue = default!;
            return false;
        }
    }
}