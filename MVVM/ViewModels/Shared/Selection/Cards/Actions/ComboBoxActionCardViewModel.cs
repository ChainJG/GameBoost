using GameBoost.Core.Interfaces;
using GameBoost.MVVM.ViewModels.Shared.Selection.Actions.Misc;
using GameBoost.Shared.Results;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions
{
    public sealed class ComboBoxActionCardViewModel : SelectionActionCardViewModelBase
    {
        public IInputActionModule<object> Module { get; init; }

        public IActionOptionProvider<object>? OptionProvider { get; init; }

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

        protected override async Task<object> RefreshStatusAsync(CancellationToken token)
        {
            if (Module is null)
                return "Select option";

            Options.Clear();

            IReadOnlyList <ActionOptionViewModel<object>> result =
                (IReadOnlyList<ActionOptionViewModel<object>>)await Module.RefreshStatusAsync(SelectedOption?.Value, token);

            Debug.WriteLine($"Options count: {result.Count}");

            foreach (var option in result)
                Options.Add(option);

            SelectedOption ??=
                Options.FirstOrDefault(option => option.IsDefaultSelected)
                ?? Options.FirstOrDefault();

            return null;
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