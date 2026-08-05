using GameBoost.Shared.Results;

namespace GameBoost.Core.Interfaces
{
    public interface IInputActionModule<TInput>
    {
        string Name { get; }

        Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token);
        Task<ModuleResult> ExecuteAsync(TInput input, CancellationToken token);
    }
}
