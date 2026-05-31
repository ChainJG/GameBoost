using GameBoost.Core.Interfaces;
using GameBoost.MVVM.ViewModels.Shared.Selection.Actions.Misc;
using GameBoost.Shared.Results;
using System.Collections.ObjectModel;

namespace GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions
{
    public sealed class ComboBoxActionCardViewModel<TValue> : SelectionActionCardViewModelBase
    {
        public required IInputActionModule<TValue> Module { get; set; }

        public ObservableCollection<ActionOptionViewModel<TValue>> Options { get; } = [];

        private ActionOptionViewModel<TValue>? _selectedOption;
        public ActionOptionViewModel<TValue>? SelectedOption
        {
            get => _selectedOption;
            set
            {
                if (!Set(ref _selectedOption, value)) return;

                if (value is not null)
                    IsChecked = true;
            }
        }

        public string PlaceholderText { get; init; } = "Select option";

        protected override async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            if (SelectedOption is null)
                return ModuleResult.Failed($"{Title} requires a selected option.");

            return await Module.ExecuteAsync(
                SelectedOption.Value,
                token);
        }

        protected override async Task<string> RefreshStatusAsync(CancellationToken token)
        {
            if (SelectedOption is null)
                return "Select option";

            return await Module.RefreshStatusAsync(
                SelectedOption.Value,
                token);
        }
    }
}
