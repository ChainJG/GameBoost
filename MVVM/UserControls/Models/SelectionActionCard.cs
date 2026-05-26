using GameBoost.Core.Interfaces;
using GameBoost.MVVM.Core;
using MaterialDesignThemes.Wpf;

namespace GameBoost.MVVM.UserControls.Models
{
    public class SelectionActionCard : ObservableObject, ISelectionButton
    {
        public SelectionFeatureCard? Parent { get; internal set; }

        public required string Title { get; set; }
        public required PackIconKind Icon { get; set; }

        private bool _isChecked = false;
        public bool IsChecked { get => _isChecked; set => Set(ref _isChecked, value); }

        public IActionModule? Module { get; set; }
    }
}
