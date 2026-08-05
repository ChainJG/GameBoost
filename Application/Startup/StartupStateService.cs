namespace GameBoost.Application.Startup
{
    /// <summary>
    /// Tracks whether application startup (splash pipeline + initial module refresh)
    /// has completed, and notifies subscribers when it does.
    /// </summary>
    public sealed class StartupStateService
    {
        public event Action? StartupCompleted;

        public bool IsStartupCompleted { get; private set; }

        public void NotifyStartupCompleted()
        {
            IsStartupCompleted = true;
            StartupCompleted?.Invoke();
        }
    }
}
