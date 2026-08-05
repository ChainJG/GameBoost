using GameBoost.Application.Modules;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;

namespace GameBoost.Application.Selection.Services
{
    public sealed class SelectionActionRefreshService(int maxConcurrentRefreshes = 4)
    {
        private readonly SemaphoreSlim _refreshConcurrency = new(Math.Clamp(maxConcurrentRefreshes, 1, 8), Math.Clamp(maxConcurrentRefreshes, 1, 8));

        public TimeSpan DefaultCacheDuration { get; } = TimeSpan.FromMinutes(5);

        public Task RefreshActionAsync(OptimizationAction action, CancellationToken token, ActionRefreshMode mode = ActionRefreshMode.UseCache) =>
            action.RefreshStatusSafeAsync(token, mode, DefaultCacheDuration);

        public async Task RefreshActionsAsync(
            IEnumerable<OptimizationAction> actions,
            CancellationToken token,
            ActionRefreshMode mode = ActionRefreshMode.UseCache,
            IProgress<ProgressResult>? progress = null)
        {
            var distinctActions = actions
                .Distinct()
                .ToList();

            if (distinctActions.Count == 0)
            {
                progress?.Report(new ProgressResult("No Modules to initialise", 100));
                return;
            }

            var completed = 0;
            var total = distinctActions.Count;

            var refreshTasks = distinctActions.Select(action =>
                RefreshActionWithConcurrencyLimitAsync(
                    action,
                    token,
                    mode,
                    progress,
                    total,
                    () => Interlocked.Increment(ref completed)));

            await Task.WhenAll(refreshTasks);
        }

        private async Task RefreshActionWithConcurrencyLimitAsync(
            OptimizationAction action,
            CancellationToken token,
            ActionRefreshMode mode,
            IProgress<ProgressResult>? progress,
            int total,
            Func<int> incrementCompleted)
        {
            await _refreshConcurrency.WaitAsync(token);

            try
            {
                token.ThrowIfCancellationRequested();

                await RefreshActionAsync(
                    action,
                    token,
                    mode);

                var completed = incrementCompleted();

                progress?.Report(
                    new ProgressResult(
                    $"Initialising Modules", MathHelper.ToPercentageInt(completed - 10, total)));
            }
            finally
            {
                _refreshConcurrency.Release();
            }
        }
    }
}
