using GameBoost.Application.Selection.Services;
using GameBoost.MVVM.Core;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions;
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;

namespace GameBoost.MVVM.ViewModels.Shared.Selection.Cards
{
    public class SelectionFeatureViewModel : ObservableObject, ISelectionButton
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public required PackIconKind Icon { get; set; }

        public SelectionType SelectionType { get; set; } = SelectionType.Multiple;

        public event Action? RunnableStateChanged;

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (!Set(ref _isChecked, value))
                    return;

                NotifyRunnableStateChanged();
            }
        }
        public ObservableCollection<SelectionActionCardViewModelBase> Actions { get; } = [];

        // Checks if at least one action is selected and the feature is checked
        public bool IsRunnable =>
            IsChecked &&
            Actions.Any(item => item.IsChecked);

        public void AddActions(IEnumerable<SelectionActionCardViewModelBase> actions)
        {
            foreach (var action in actions)
                AddAction(action);
        }
        public void AddAction(SelectionActionCardViewModelBase action)
        {
            action.Parent = this;
            Actions.Add(action);
        }

        internal void OnActionSelectionChanged(SelectionActionCardViewModelBase changedAction)
        {
            if (changedAction.IsChecked)
                IsChecked = true;

            if (SelectionType == SelectionType.Single && changedAction.IsChecked)
                UnCheckOtherActions(changedAction);

            if (!IsRunnable)
                IsChecked = false;

            NotifyRunnableStateChanged();
        }

        private void UnCheckOtherActions(SelectionActionCardViewModelBase changedAction)
        {
            foreach (var action in Actions)
            {
                if (ReferenceEquals(action, changedAction))
                    continue;

                action.IsChecked = false;
            }
        }

        private void NotifyRunnableStateChanged()
        {
            OnPropertyChanged(nameof(IsRunnable));
            RunnableStateChanged?.Invoke();
        }
    }
}
