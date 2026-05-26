using GameBoost.Core.Interfaces;
using GameBoost.MVVM.Core;
using MaterialDesignThemes.Wpf;

namespace GameBoost.MVVM.ViewModels.Shared.Selection
{
    public class SelectionActionViewModel : ObservableObject, ISelectionButton
    {
        public SelectionFeatureViewModel? Parent { get; internal set; }

        public required string Title { get; set; }
        public required PackIconKind Icon { get; set; }

        private string _status = string.Empty;
        public string Status { get => _status; set => Set(ref _status, value); }

        private bool _isChecked = false;
        public bool IsChecked { get => _isChecked; set => Set(ref _isChecked, value); }

        public IActionModule? Module { get; set; }
    }
}
