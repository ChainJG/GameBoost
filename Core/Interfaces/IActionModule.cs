using GameBoost.Shared.Results;

namespace GameBoost.Core.Interfaces
{
    public interface IActionModule 
    {
        string Name { get; }
        string Status { get; }

        Task RefreshStatusAsync();

        Task<ModuleResult> ExecuteAsync();
    }
}
