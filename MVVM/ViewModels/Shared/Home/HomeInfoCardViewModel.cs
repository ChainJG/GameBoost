using MaterialDesignThemes.Wpf;

namespace GameBoost.MVVM.ViewModels.Shared.Home
{
    public sealed class HomeInfoCardViewModel
    {
        public required PackIconKind Icon { get; init; }

        public required string Title { get; init; }

        public required string Info { get; init; }

        public string Footer { get; init; } = string.Empty;
    }
}
