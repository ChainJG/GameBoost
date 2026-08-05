using GameBoost.Core.Modules;

namespace GameBoost.Shared.Results
{
    public sealed class ActionRefreshResult
    {
        public string? StatusText { get; init; }

        public object? Value { get; init; }

        public IReadOnlyList<ActionOption>? Options { get; init; }

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
            IReadOnlyList<ActionOption> options,
            string? statusText = null)
        {
            return new ActionRefreshResult
            {
                Options = options,
                StatusText = statusText
            };
        }
        public static ActionRefreshResult OptionsAndValue(
            IReadOnlyList<ActionOption> options,
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
