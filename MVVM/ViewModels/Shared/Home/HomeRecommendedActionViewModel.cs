using GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions;
using MaterialDesignThemes.Wpf;

namespace GameBoost.MVVM.ViewModels.Shared.Home
{
    public sealed class HomeRecommendedActionViewModel
    {
        public required string FeatureTitle { get; init; }

        public required SelectionActionCardViewModelBase Action { get; init; }

        public string Title => FeatureTitle;

        public PackIconKind Icon => Action.Icon;

        public string Info => Action.Title;

        public string CurrentText => Action.Status;

        public string RecommendedText => Action.RecommendedValue?.ToString() ?? string.Empty;

        public string Reason => Action.RecommendationToolTip;

        public RecommendationPriority Priority => Action.RecommendationPriority;

        public string Footer => $"{CurrentText} → {RecommendedText}";
    }
}
