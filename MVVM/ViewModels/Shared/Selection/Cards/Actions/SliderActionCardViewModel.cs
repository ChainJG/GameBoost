using GameBoost.Core.Interfaces;
using GameBoost.Shared.Results;

namespace GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions
{
    public sealed class SliderActionCardViewModel : SelectionActionCardViewModelBase
    {
        public IInputActionModule<double>? Module { get; init; }

        protected override IRequireModule? RquiredModule => Module as IRequireModule;
        protected override IRecommendedActionModule? RecommendationModule => Module as IRecommendedActionModule;

        public double Minimum { get; init; }

        public double Maximum { get; init; } = 100;

        public double TickFrequency { get; init; } = 1;

        public string ValueSuffix { get; init; } = string.Empty;

        private double _value;
        public double Value
        {
            get => _value;
            set
            {
                if (!Set(ref _value, value))
                    return;

                SetCurrentValue(value);

                IsChecked = true;

                OnPropertyChanged(nameof(ValueText));
            }
        }

        public string ValueText
        {
            get
            {
                if (Value >= Maximum)
                    return "Max";

                return $"{Value:0}{ValueSuffix}";
            }
        }

        protected override Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            if (Module is null)
                throw new InvalidOperationException("Does not have a module");

            return Module.RefreshStatusAsync(token);
        }

        protected override void ApplyRefreshResult(ActionRefreshResult refreshResult)
        {
            base.ApplyRefreshResult(refreshResult);

            if (refreshResult.Value is double doubleValue)
            {
                SetValueFromRefresh(doubleValue);
                return;
            }

            if (refreshResult.Value is int intValue)
            {
                SetValueFromRefresh(intValue);
                return;
            }

            if (double.TryParse(refreshResult.Value?.ToString(), out var parsedValue))
            {
                SetValueFromRefresh(parsedValue);
            }
        }

        private void SetValueFromRefresh(double value)
        {
            if (!Set(ref _value, value, nameof(Value)))
                return;

            SetCurrentValue(value);

            OnPropertyChanged(nameof(ValueText));
        }

        protected override Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            if (Module is null)
                throw new InvalidOperationException("Does not have a module");

            return Module.ExecuteAsync(Value, token);
        }
    }
}