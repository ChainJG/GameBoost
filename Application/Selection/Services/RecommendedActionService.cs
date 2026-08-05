using GameBoost.Application.Modules;
using GameBoost.MVVM.ViewModels.Shared.Selection;
using GameBoost.Shared.Results;

namespace GameBoost.Application.Selection.Services
{
    public sealed class RecommendedActionService
    {
        private readonly SelectionActionRefreshService _refreshService;
        private readonly List<SelectionViewModel> _selectionPages = [];
        private readonly List<OptimizationAction> _recommendedActions = [];

        public RecommendedActionService(
            SelectionActionRefreshService refreshService,
            SelectionScanNotificationService scanNotifications)
        {
            _refreshService = refreshService;

            scanNotifications.ScanCompleted += args =>
            {
                _ = RefreshActionsAsync(args.Actions, CancellationToken.None);
            };
        }

        public event Action? RecommendationsChanged;

        public IReadOnlyList<OptimizationAction> RecommendedActions =>
            _recommendedActions;

        public IReadOnlyList<OptimizationAction> AllActions =>
            [.. _selectionPages
            .SelectMany(page => page.FeatureCards)
            .SelectMany(feature => feature.Actions)
            .Select(card => card.Action)
            .Distinct()];

        public void RegisterSelectionPages(
            IEnumerable<SelectionViewModel> selectionPages)
        {
            _selectionPages.Clear();
            _selectionPages.AddRange(selectionPages.Distinct());
        }


        public async Task RefreshAllAsync(IProgress<ProgressResult>? progress = null, CancellationToken token = default)
        {
            var actions = AllActions.ToList();

            await _refreshService.RefreshActionsAsync(
                actions,
                token,
                ActionRefreshMode.UseCache,
                progress);

            RebuildRecommendations(actions, token);
            RecommendationsChanged?.Invoke();
        }

        public async Task RefreshActionsAsync(
            IReadOnlyList<OptimizationAction> actions,
            CancellationToken token = default)
        {
            if (actions.Count == 0)
                return;

            await _refreshService.RefreshActionsAsync(
                actions,
                token,
                ActionRefreshMode.UseCache);

            RebuildRecommendations([.. AllActions], token);

            RecommendationsChanged?.Invoke();
        }

        private void RebuildRecommendations(
            IReadOnlyList<OptimizationAction> actions,
            CancellationToken token)
        {
            _recommendedActions.Clear();

            foreach (var action in actions)
            {
                token.ThrowIfCancellationRequested();

                if (!action.ShouldShowAsRecommendation)
                    continue;

                if (action.RequiresAdmin &&
                    GameBoostContext.SystemInfo?.IsAdministrator != true)
                {
                    continue;
                }

                _recommendedActions.Add(action);
            }

            _recommendedActions.Sort(CompareRecommendationActions);
        }

        private static int CompareRecommendationActions(
            OptimizationAction first,
            OptimizationAction second)
        {
            var priorityCompare = second.RecommendationPriority.CompareTo(
                first.RecommendationPriority);

            if (priorityCompare != 0)
                return priorityCompare;

            var parentCompare = string.Compare(
                first.FeatureTitle,
                second.FeatureTitle,
                StringComparison.OrdinalIgnoreCase);

            if (parentCompare != 0)
                return parentCompare;

            return string.Compare(
                first.Title,
                second.Title,
                StringComparison.OrdinalIgnoreCase);
        }
    }
}
