using GameBoost.Core.Interfaces;
using GameBoost.MVVM.ViewModels.Shared.Selection.Actions.Misc;
using GameBoost.Shared.Results;
using System.Collections.ObjectModel;

namespace GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions
{
    public sealed class ComboBoxActionCardViewModel : SelectionActionCardViewModelBase
    {
        public IInputActionModule<object> Module { get; init; }

        public ObservableCollection<ActionOptionViewModel<object>> Options { get; } = [];

        private ActionOptionViewModel<object>? _selectedOption;
        public ActionOptionViewModel<object>? SelectedOption
        {
            get => _selectedOption;
            set
            {
                if (!Set(ref _selectedOption, value))
                    return;

                if (value is not null)
                    IsChecked = true;
            }
        }

        public string PlaceholderText { get; init; } = "Select option";

        protected override Task<string> RefreshStatusAsync(CancellationToken token)
        {
            if (SelectedOption is null || Module is null)
                return Task.FromResult("Select option");

            return Module.RefreshStatusAsync(
                SelectedOption.Value,
                token);
        }

        protected override Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            if (Module is null)
                return Task.FromResult(ModuleResult.Failed($"{Title} does not have a module"));

            if (SelectedOption is null)
                return Task.FromResult(ModuleResult.Failed($"{Title} requires a selected option."));

            return Module.ExecuteAsync(
                SelectedOption.Value,
                token);
        }
    }
}