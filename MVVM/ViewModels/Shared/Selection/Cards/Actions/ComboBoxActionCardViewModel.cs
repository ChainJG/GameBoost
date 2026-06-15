using GameBoost.Core.Interfaces;
using GameBoost.MVVM.ViewModels.Shared.Selection.Actions.Misc;
using GameBoost.Shared.Results;
using System.Collections.ObjectModel;

namespace GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions
{
    public sealed class ComboBoxActionCardViewModel : SelectionActionCardViewModelBase
    {
        public IInputActionModule<object> Module { get; init; }

        protected override IRequiredModule? RequiredModule => Module as IRequiredModule;
        protected override IRecommendedActionModule? RecommendationModule => Module as IRecommendedActionModule;

        public ObservableCollection<ActionOptionViewModel<object>> Options { get; } = [];

        private ActionOptionViewModel<object>? _selectedOption;
        public ActionOptionViewModel<object>? SelectedOption
        {
            get => _selectedOption;
            set
            {
                if (!Set(ref _selectedOption, value))
                    return;

                SetCurrentValue(value?.Value);

                if (value is not null)
                    IsChecked = true;
            }
        }
        protected override Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            if (Module is null)
                throw new InvalidOperationException("Does not have a module");

            if (SelectedOption is null)
                throw new InvalidOperationException("Requires a selected option");

            return Module.ExecuteAsync(
                SelectedOption.Value,
                token);
        }

        protected override async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            if (Module is null)
                throw new InvalidOperationException("Does not have a module");

            return await Module.RefreshStatusAsync(token);
        }

        protected override void ApplyRefreshResult(ActionRefreshResult refreshResult)
        {
            if (refreshResult.Options is null)
                return;

            base.ApplyRefreshResult(refreshResult);

            BuildOptions(refreshResult.Options, refreshResult.Value);
        }

        private void BuildOptions(IReadOnlyList<ActionOptionViewModel<object>> options, object? refreshedValue)
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

        private void SetSelectedOptionFromRefresh(ActionOptionViewModel<object>? option)
        {
            Set(ref _selectedOption, option, nameof(SelectedOption));
            SetCurrentValue(option?.Value);
        }
    }
}