using GameBoost.Application.Modules;

namespace GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions
{
    public sealed class SliderActionCardViewModel(OptimizationAction action)
        : SelectionActionCardViewModelBase(action)
    {
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

                Action.DesiredValue = value;

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

        protected override void OnActionValueChanged()
        {
            var refreshedValue = Action.CurrentValue;

            if (refreshedValue is double doubleValue)
            {
                SetValueFromRefresh(doubleValue);
                return;
            }

            if (refreshedValue is int intValue)
            {
                SetValueFromRefresh(intValue);
                return;
            }

            if (double.TryParse(refreshedValue?.ToString(), out var parsedValue))
            {
                SetValueFromRefresh(parsedValue);
            }
        }

        // Applies a refresh-driven value without marking the card as user-selected.
        private void SetValueFromRefresh(double value)
        {
            if (!Set(ref _value, value, nameof(Value)))
                return;

            Action.DesiredValue = value;

            OnPropertyChanged(nameof(ValueText));
        }
    }
}
