using GameBoost.Shared.Results;

namespace GameBoost.Core.Interfaces
{
    public interface IActionModule 
    {
        string Name { get; }

        string Status { get; }

        string GetStatus();

        Task<ModuleResult> ExecuteAsync();
    }
}
