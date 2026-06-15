using GameBoost.MVVM.ViewModels.Shared.Selection;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions;

namespace GameBoost.Application.Selection.Services
{
    public sealed class SelectionActionRefreshService(int maxConcurrentRefreshes = 4)
    {
        private readonly SemaphoreSlim _refreshConcurrency = new(Math.Clamp(maxConcurrentRefreshes, 1, 8), Math.Clamp(maxConcurrentRefreshes, 1, 8));

        public TimeSpan DefaultCacheDuration { get; } = TimeSpan.FromMinutes(5);

        public Task RefreshActionAsync(SelectionActionCardViewModelBase action, CancellationToken token, ActionRefreshMode mode = ActionRefreshMode.UseCache) =>
            action.RefreshStatusSafeAsync(token, mode, DefaultCacheDuration);

        public async Task RefreshActionsAsync(IEnumerable<SelectionActionCardViewModelBase> actions, CancellationToken token, ActionRefreshMode mode = ActionRefreshMode.UseCache)
        {
            var distinctActions = actions
                .Distinct()
                .ToList();

            if (distinctActions.Count == 0)
                return;

            var refreshTasks = distinctActions.Select(action =>
                RefreshActionWithConcurrencyLimitAsync(
                    action,
                    token,
                    mode));

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
            ActionRefreshMode mode)
        {
            await _refreshConcurrency.WaitAsync(token);

            try
            {
                await RefreshActionAsync(
                    action,
                    token,
                    mode);
            }
            finally
            {
                _refreshConcurrency.Release();
            }
        }
    }
}