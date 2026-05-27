using GameBoost.MVVM.Core;
using MaterialDesignThemes.Wpf;

namespace GameBoost.MVVM.ViewModels.Shared.Selection
{
    public class SelectionFeatureViewModel : ObservableObject, ISelectionButton
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required PackIconKind Icon { get; set; }

        public bool IsSelected { get; set; }
        public SelectionType SelectionType = SelectionType.Multiple;

        private bool _isChecked = false;
        public bool IsChecked { get => _isChecked; set => Set(ref _isChecked, value); }

        public List<SelectionActionViewModel> Actions { get; set; } = [];

        // Checks if at least one action is selected and the feature is checked
        public bool IsRunnable =>
            IsChecked &&
            Actions.Any(item => item.IsChecked);

        public async Task RefreshStatusesAsync(CancellationToken token)
        {
            foreach (var action in Actions)
                await action.RefreshStatusAsync(token);
        }
    }
}
