using GameBoost.Application.Modules;

namespace GameBoost.Core.EventArguments
{
    public sealed class SelectionScanCompletedEventArgs : EventArgs
    {
        public IReadOnlyList<OptimizationAction> Actions { get; set; } = [];
        public int TotalCount => SuccessCount + FailCount;
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public TimeSpan ExecutionTime { get; set; }
    }
}
