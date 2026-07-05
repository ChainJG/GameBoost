using GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions;
using GameBoost.Shared.Results;

namespace GameBoost.Core.Interfaces
{
    public interface IActionModule
    {
        string Name { get; }
        SelectionActionCardViewModelBase? ActionCard { get; set; }

        Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token);
        Task<ModuleResult> ExecuteAsync(CancellationToken token);
    }
}
