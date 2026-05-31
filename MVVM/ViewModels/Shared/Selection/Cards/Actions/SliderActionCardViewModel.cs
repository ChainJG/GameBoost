using GameBoost.Core.Interfaces;
using GameBoost.Shared.Results;

namespace GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions
{
    public sealed class SliderActionCardViewModel : SelectionActionCardViewModelBase
    {

        public required IInputActionModule<double> Module { get; init; }

        public double Minimum { get; init; }

        public double Maximum { get; init; } = 100;

        public double TickFrequency { get; init; } = 1;

        public string ValueSuffix { get; init; } = string.Empty;

        private double _value = 0;
        public double Value
        {
            get => _value;
            set
            {
                if (!Set(ref _value, value)) return;

                IsChecked = true;
                OnPropertyChanged(nameof(ValueText));
            }
        }

        public string ValueText => $"{Value:0}{ValueSuffix}";

        protected override Task<string> RefreshStatusAsync(CancellationToken token) =>
            Module.RefreshStatusAsync(Value, token);

        protected override Task<ModuleResult> ExecuteAsync(CancellationToken token) =>
            Module.ExecuteAsync(Value, token);
    }
}
