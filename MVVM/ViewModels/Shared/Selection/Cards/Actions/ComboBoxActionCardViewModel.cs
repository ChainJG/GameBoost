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

        protected override Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            if (Module is null)
                return Task.FromResult(ModuleResult.Failed($"Does not have a module"));

            if (SelectedOption is null)
                return Task.FromResult(ModuleResult.Failed($"Requires a selected option"));

            return Module.ExecuteAsync(
                SelectedOption.Value,
                token);
        }
        protected override async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            if (Module is null)
                throw new InvalidOperationException($"Does not have a module");

            return await Module.RefreshStatusAsync(token);
        }

        protected override void ApplyRefreshResult(ActionRefreshResult refreshResult)
        {
            if (refreshResult.Options is not null)
                ReplaceOptions(refreshResult.Options);

            SelectBestOption(refreshResult.Value);
        }

        private void ReplaceOptions(
            IReadOnlyList<ActionOptionViewModel<object>> options)
        {
            var previousValue = SelectedOption?.Value;

            Options.Clear();

            foreach (var option in options)
                Options.Add(option);

            SelectedOption =
                Options.FirstOrDefault(option => Equals(option.Value, previousValue))
                ?? Options.FirstOrDefault(option => option.IsDefaultSelected)
                ?? Options.FirstOrDefault();
        }

        private void SelectBestOption(object? value)
        {
            if (value is null || Options.Count == 0)
                return;

            var matchingOption = Options.FirstOrDefault(option =>
                Equals(option.Value, value));

            if (matchingOption is not null)
                SelectedOption = matchingOption;
        }

    }
}