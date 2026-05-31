using GameBoost.Shared.Results;

namespace GameBoost.Core.Interfaces
{
    public interface IInputActionModule<TInput>
    {
        string Name { get; }
        Task<string> RefreshStatusAsync(TInput input, CancellationToken token);
        Task<ModuleResult> ExecuteAsync(TInput input, CancellationToken token);
    }
}
