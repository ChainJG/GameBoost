using GameBoost.MVVM.ViewModels.Shared.Selection;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace GameBoost.Application.Selection.Services
{
    public sealed class SelectionActionRefreshService(int maxConcurrentRefreshes = 4)
    {
        private readonly SemaphoreSlim _refreshConcurrency = new(Math.Clamp(maxConcurrentRefreshes, 1, 8), Math.Clamp(maxConcurrentRefreshes, 1, 8));

        public TimeSpan DefaultCacheDuration { get; } = TimeSpan.FromMinutes(5);

        public Task RefreshActionAsync(SelectionActionCardViewModelBase action, CancellationToken token, ActionRefreshMode mode = ActionRefreshMode.UseCache) =>
            action.RefreshStatusSafeAsync(token, mode, DefaultCacheDuration);

        public async Task RefreshActionsAsync(
            IEnumerable<SelectionActionCardViewModelBase> actions,
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

        public Task RefreshFeatureAsync(
            SelectionFeatureViewModel feature,
            CancellationToken token,
            ActionRefreshMode mode = ActionRefreshMode.UseCache)
        {
            return RefreshActionsAsync(
                feature.Actions,
                token,
                mode);
        }

        public Task RefreshFeaturesAsync(
            IEnumerable<SelectionFeatureViewModel> features,
            CancellationToken token,
            ActionRefreshMode mode = ActionRefreshMode.UseCache)
        {
            return RefreshActionsAsync(
                features.SelectMany(feature => feature.Actions),
                token,
                mode);
        }

        public Task RefreshPageAsync(
            SelectionViewModel page,
            CancellationToken token,
            ActionRefreshMode mode = ActionRefreshMode.UseCache)
        {
            return RefreshFeaturesAsync(
                page.FeatureCards,
                token,
                mode);
        }

        public Task RefreshPagesAsync(
            IEnumerable<SelectionViewModel> pages,
            CancellationToken token,
            ActionRefreshMode mode = ActionRefreshMode.UseCache)
        {
            return RefreshActionsAsync(
                pages
                    .SelectMany(page => page.FeatureCards)
                    .SelectMany(feature => feature.Actions),
                token,
                mode);
        }

        private async Task RefreshActionWithConcurrencyLimitAsync(
            SelectionActionCardViewModelBase action,
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