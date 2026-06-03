namespace GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions.Misc
{
    public sealed class ExecutionRequirementsEventArgs : EventArgs
    {
        public bool RequiresRestart { get; init; }

        public bool RequiresAdmin { get; init; }

        public IReadOnlyList<string> RestartRequiredActions { get; init; } = [];

        public IReadOnlyList<string> AdminRequiredActions { get; init; } = [];
    }
}
