using GameBoost.MVVM.ViewModels.Shared.Selection;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions;
using GameBoost.Shared.Results;

namespace GameBoost.Application.Selection.Services
{
    public sealed class RecommendedActionService
    {
        private readonly GameBoostUIServices _uiServices;
        private readonly List<SelectionViewModel> _selectionPages = [];
        private readonly List<SelectionActionCardViewModelBase> _recommendedActions = [];

        public RecommendedActionService(GameBoostUIServices uiService)
        {
            _uiServices = uiService;

            _uiServices.SelectionScanNotifications.ScanCompleted += args =>
            {
                _ = RefreshActionsAsync(args.ActionsCards, CancellationToken.None);
            };
        }

        public event Action? RecommendationsChanged;

        public IReadOnlyList<SelectionActionCardViewModelBase> RecommendedActions =>
            _recommendedActions;

        public IReadOnlyList<SelectionActionCardViewModelBase> AllActions =>
            [.. _selectionPages
            .SelectMany(page => page.FeatureCards)
            .SelectMany(feature => feature.Actions)
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

            await _uiServices.SelectionRefresh.RefreshActionsAsync(
                actions,
                token,
                ActionRefreshMode.UseCache,
                progress);

            RebuildRecommendations(actions, token);
            RecommendationsChanged?.Invoke();
        }

        public async Task RefreshActionsAsync(
            IReadOnlyList<SelectionActionCardViewModelBase> actions,
            CancellationToken token = default)
        {
            if (actions.Count == 0)
                return;

            await _uiServices.SelectionRefresh.RefreshActionsAsync(
                actions,
                token,
                ActionRefreshMode.UseCache);

            RebuildRecommendations([.. AllActions], token);

            RecommendationsChanged?.Invoke();
        }

        private void RebuildRecommendations(
            IReadOnlyList<SelectionActionCardViewModelBase> actions,
            CancellationToken token)
        {
            _recommendedActions.Clear();

            foreach (var action in actions)
            {
                token.ThrowIfCancellationRequested();

                if (!action.ShouldShowAsHomeRecommendation)
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
            SelectionActionCardViewModelBase first,
            SelectionActionCardViewModelBase second)
        {
            var priorityCompare = second.RecommendationPriority.CompareTo(
                first.RecommendationPriority);

            if (priorityCompare != 0)
                return priorityCompare;

            var parentCompare = string.Compare(
                first.Parent?.Title,
                second.Parent?.Title,
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
