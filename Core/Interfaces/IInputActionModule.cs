using GameBoost.Shared.Results;

namespace GameBoost.Core.Interfaces
{
    public interface IInputActionModule<TInput>
    {
        string Name { get; }
        Task<object> RefreshStatusAsync(TInput input, CancellationToken token);
        Task<ModuleResult> ExecuteAsync(TInput input, CancellationToken token);
    }
}
