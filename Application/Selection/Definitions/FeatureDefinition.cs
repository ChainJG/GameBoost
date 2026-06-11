using MaterialDesignThemes.Wpf;

namespace GameBoost.Application.Selection.Definitions
{
    public sealed class FeatureDefinition
    {
        public required string Title { get; init; }

        public required string Description { get; init; }

        public required PackIconKind Icon { get; init; }

        public SelectionType SelectionType { get; init; } = SelectionType.Multiple;

        public IReadOnlyList<ActionCardDefinition> Actions { get; init; } = [];
    }
}
