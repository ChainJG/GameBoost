using GameBoost.MVVM.ViewModels.Shared.Selection.Actions.Misc;

namespace GameBoost.Shared.Results
{
    public sealed class ActionRefreshResult
    {
        public string? StatusText { get; init; }

        public object? Value { get; init; }

        public IReadOnlyList<ActionOptionViewModel<object>>? Options { get; init; }

        public static ActionRefreshResult Status(string statusText)
        {
            return new ActionRefreshResult
            {
                StatusText = statusText
            };
        }

        public static ActionRefreshResult ValueOnly(object value, string? statusText)
        {
            return new ActionRefreshResult
            {
                Value = value,
                StatusText = statusText
            };
        }

        public static ActionRefreshResult OptionsOnly(
            IReadOnlyList<ActionOptionViewModel<object>> options,
            string? statusText = null)
        {
            return new ActionRefreshResult
            {
                Options = options,
                StatusText = statusText
            };
        }
        public static ActionRefreshResult OptionsAndValue(
            IReadOnlyList<ActionOptionViewModel<object>> options,
            object? value,
            string? statusText = null)
        {
            return new ActionRefreshResult
            {
                Options = options,
                Value = value,
                StatusText = statusText
            };
        }
    }
}