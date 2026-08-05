using GameBoost.Application.Modules;
using GameBoost.Core.Modules;
using System.Collections.ObjectModel;

namespace GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions
{
    public sealed class ComboBoxActionCardViewModel(OptimizationAction action)
        : SelectionActionCardViewModelBase(action)
    {
        public ObservableCollection<ActionOption> Options { get; } = [];

        private ActionOption? _selectedOption;
        public ActionOption? SelectedOption
        {
            get => _selectedOption;
            set
            {
                if (!Set(ref _selectedOption, value))
                    return;

                Action.DesiredValue = value?.Value;

                if (value is not null)
                    IsChecked = true;
            }
        }

        protected override void OnActionOptionsChanged()
        {
            if (Action.Options is null)
                return;

            BuildOptions(Action.Options, Action.CurrentValue);
        }

        private void BuildOptions(IReadOnlyList<ActionOption> options, object? refreshedValue)
        {
            var previousValue = SelectedOption?.Value;

            Options.Clear();

            foreach (var option in options)
                Options.Add(option);

            var selectedOption =
                Options.FirstOrDefault(option => ValuesMatch(option.Value, refreshedValue))
                ?? Options.FirstOrDefault(option => option.IsDefaultSelected)
                ?? Options.FirstOrDefault(option => ValuesMatch(option.Value, previousValue))
                ?? Options.FirstOrDefault();

            SetSelectedOptionFromRefresh(selectedOption);
        }

        private static bool ValuesMatch(object? left, object? right)
        {
            if (Equals(left, right))
                return true;

            if (left is null || right is null)
                return false;

            return string.Equals(
                left.ToString(),
                right.ToString(),
                StringComparison.OrdinalIgnoreCase);
        }

        // Applies a refresh-driven selection without marking the card as user-selected.
        private void SetSelectedOptionFromRefresh(ActionOption? option)
        {
            Set(ref _selectedOption, option, nameof(SelectedOption));
            Action.DesiredValue = option?.Value;
        }
    }
}
