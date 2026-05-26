using GameBoost.Shared.Results;

namespace GameBoost.Core.Interfaces
{
    public interface IStartupStep
    {
        string Name { get; }
        Task<ModuleResult> ExecuteAsync(IProgress<ProgressResult> progress);
    }
}
