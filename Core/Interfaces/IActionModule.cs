using GameBoost.Shared.Results;

namespace GameBoost.Core.Interfaces
{
    public interface IActionModule 
    {
        string Name { get; }
        Task<string> RefreshStatusAsync();

        Task<ModuleResult> ExecuteAsync();
    }
}
