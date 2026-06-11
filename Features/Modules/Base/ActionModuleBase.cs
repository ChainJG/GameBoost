using GameBoost.Core.Interfaces;
using GameBoost.Shared.Results;

namespace GameBoost.Features.Modules.Base
{
    public abstract class ActionModuleBase : IActionModule, IRecommendedActionModule, IRequiredModule
    {
        // IActionModule
        public abstract string Name { get; }


        // IRequiredModule
        public virtual bool SystemReboot => false;
        public virtual bool Admin => false;


        // IRecommendedActionModule
        public virtual RecommendationPriority RecommendationPriority => RecommendationPriority.None;
        public virtual object? RecommendedValue => ToggleType.None;
        public virtual string RecommendationReason => $"{Name} is recommended to be {RecommendedValue}";
        public virtual bool IsRecommendedValue(object? currentValue)
        {
            if (RecommendedValue is not ToggleType recommendedValue || recommendedValue == ToggleType.None)
                return false;

            return currentValue is ToggleType toggleType &&
                   toggleType == recommendedValue;
        }
        public virtual Task<ModuleResult> ExecuteRecommendedAsync(CancellationToken token) => ExecuteAsync(token);


        // IActionModule
        public abstract Task<ModuleResult> ExecuteAsync(CancellationToken token);
        public abstract Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token);



        protected abstract string FormatStatus(ToggleType status);
    }
}
