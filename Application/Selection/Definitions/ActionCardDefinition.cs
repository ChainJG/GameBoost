using GameBoost.Core.Interfaces;
using MaterialDesignThemes.Wpf;

namespace GameBoost.Application.Selection.Definitions
{
    public sealed class ActionCardDefinition
    {
        public required string Title { get; init; }

        public required PackIconKind Icon { get; init; }

        public required ActionCardKind Kind { get; init; }

        public string InfoToolTip { get; init; } = string.Empty;

        public IActionModule? ActionModule { get; init; }

        public IInputActionModule<object>? ObjectInputModule { get; init; }

        public IInputActionModule<double>? DoubleInputModule { get; init; }

        public double Minimum { get; init; }

        public double Maximum { get; init; } = 100;

        public double TickFrequency { get; init; } = 1;

        public string ValueSuffix { get; init; } = string.Empty;
    }
}