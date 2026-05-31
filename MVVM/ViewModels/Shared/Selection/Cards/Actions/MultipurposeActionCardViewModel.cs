using GameBoost.Core.Interfaces;
using GameBoost.Shared.Results;

namespace GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions
{
    public sealed class MultipurposeActionCardViewModel : SelectionActionCardViewModelBase
    {
        public required IActionModule Module { get; init; }

        protected override Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            return Module.ExecuteAsync(token);
        }

        protected override Task<string> RefreshStatusAsync(CancellationToken token)
        {
            return Module.RefreshStatusAsync(token);
        }
    }
}
