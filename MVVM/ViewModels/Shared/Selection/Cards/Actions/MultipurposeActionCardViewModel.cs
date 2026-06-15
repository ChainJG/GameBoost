using GameBoost.Core.Interfaces;
using GameBoost.Shared.Results;

namespace GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions
{
    public sealed class MultipurposeActionCardViewModel : SelectionActionCardViewModelBase
    {
        public IActionModule? Module { get; init; }

        protected override IRequiredModule? RequiredModule => Module as IRequiredModule;
        protected override IRecommendedActionModule? RecommendationModule => Module as IRecommendedActionModule;

        protected override Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            if (Module is null)
                throw new InvalidOperationException("Does not have a module");

            return Module.ExecuteAsync(token);
        }

        protected override async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            if (Module is null)
                throw new InvalidOperationException("Does not have a module");

            return await Module.RefreshStatusAsync(token);
        }
    }
}