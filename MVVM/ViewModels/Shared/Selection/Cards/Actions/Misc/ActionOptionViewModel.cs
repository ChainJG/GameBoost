namespace GameBoost.MVVM.ViewModels.Shared.Selection.Actions.Misc
{
    public sealed class ActionOptionViewModel<TValue>
    {
        public required string DisplayText { get; init; }
        public required TValue Value { get; init; }
        public string? Description { get; init; }
        public bool IsDefaultSelected { get; set; }
        public override string ToString() => DisplayText;
    }
}
