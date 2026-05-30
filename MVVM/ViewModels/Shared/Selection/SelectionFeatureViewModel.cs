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

        public List<SelectionActionViewModel> Actions { get; } = [];

        // Checks if at least one action is selected and the feature is checked
        public bool IsRunnable =>
            IsChecked &&
            Actions.Any(item => item.IsChecked);

        public void AddAction(SelectionActionViewModel action)
        {
            action.Parent = this;
            Actions.Add(action);
        }
        public void AddActions(IEnumerable<SelectionActionViewModel> actions)
        {
            foreach (var action in actions)
                AddAction(action);
        }

        public async Task RefreshStatusesAsync(CancellationToken token)
        {
            foreach (var action in Actions)
                await action.RefreshStatusAsync(token);
        }

        internal void OnActionSelectionChanged(SelectionActionViewModel changedAction)
        {
            if (SelectionType == SelectionType.Single && changedAction.IsChecked)
            {
                UnCheckOtherActions(changedAction);
            }
        }

        private void UnCheckOtherActions(SelectionActionViewModel changedAction)
        {
            foreach (var action in Actions)
                action.IsChecked = action == changedAction;
        }
    }
}
