using GameBoost.Shared.Results;

namespace GameBoost.Core.Interfaces
{
    public interface IActionModule
    {
        string Name { get; }
        Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token);
        Task<ModuleResult> ExecuteAsync(CancellationToken token);
    }
}
