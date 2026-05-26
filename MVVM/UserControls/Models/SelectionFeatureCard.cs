using GameBoost.MVVM.Core;
using MaterialDesignThemes.Wpf;

namespace GameBoost.MVVM.UserControls.Models
{
    public class SelectionFeatureCard : ObservableObject, ISelectionButton
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required PackIconKind Icon { get; set; }

        public bool IsSelected { get; set; }
        public bool AllowMultiSelection { get; set; } = true;

        private bool _isChecked = false;
        public bool IsChecked { get => _isChecked; set => Set(ref _isChecked, value); }

        public List<SelectionActionCard> Actions { get; set; } = [];

        // Checks if at least one action is selected and the feature is checked
        public bool IsRunnable =>
            IsChecked &&
            Actions.Any(item => item.IsChecked);
    }
}
