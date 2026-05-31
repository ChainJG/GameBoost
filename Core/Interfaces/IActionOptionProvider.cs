using GameBoost.MVVM.ViewModels.Shared.Selection.Actions.Misc;

namespace GameBoost.Core.Interfaces
{
    public interface IActionOptionProvider<TValue>
    {
        Task<IReadOnlyList<ActionOptionViewModel<TValue>>> GetOptionsAsync(
            CancellationToken token);
    }
}
