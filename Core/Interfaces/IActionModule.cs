using GameBoost.Shared.Results;

namespace GameBoost.Core.Interfaces
{
    public interface IActionModule
    {
        string Name { get; }

        Task<object> RefreshStatusAsync(CancellationToken token);

        Task<ModuleResult> ExecuteAsync(CancellationToken token);
    }
}
