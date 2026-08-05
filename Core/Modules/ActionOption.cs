namespace GameBoost.Core.Modules
{
    /// <summary>
    /// A selectable option produced by an optimisation module (e.g. a power plan or
    /// service startup mode). Plain data with no UI dependencies so modules can be
    /// consumed outside the Selection UI.
    /// </summary>
    public sealed class ActionOption
    {
        public required string DisplayText { get; init; }
        public required object Value { get; init; }
        public string? Description { get; init; }
        public bool IsDefaultSelected { get; set; }
        public override string ToString() => DisplayText;
    }
}
