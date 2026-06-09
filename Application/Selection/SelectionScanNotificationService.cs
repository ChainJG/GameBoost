using GameBoost.Core.EventArguments;

namespace GameBoost.Application.Selection
{
    public sealed class SelectionScanNotificationService
    {
        public event Action<SelectionScanCompletedEventArgs>? ScanCompleted;

        public SelectionScanCompletedEventArgs? LastScan { get; private set; }

        public void NotifyCompleted(SelectionScanCompletedEventArgs args)
        {
            LastScan = args;

            ScanCompleted?.Invoke(args);
        }
    }
}
