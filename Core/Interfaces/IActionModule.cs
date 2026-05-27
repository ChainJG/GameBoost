using GameBoost.Shared.Results;

namespace GameBoost.Core.Interfaces
{
    public interface IActionModule 
    {
        string Name { get; }
        Task<string> RefreshStatusAsync(CancellationToken token);

        Task<ModuleResult> ExecuteAsync(CancellationToken token);
    }
}
