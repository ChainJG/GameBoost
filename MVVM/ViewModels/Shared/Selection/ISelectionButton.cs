using GameBoost.MVVM.Core;
using MaterialDesignThemes.Wpf;

namespace GameBoost.MVVM.ViewModels.Shared.Selection
{
    public interface ISelectionButton
    {
        public string Title { get; }
        public PackIconKind Icon { get; }
    }
}
