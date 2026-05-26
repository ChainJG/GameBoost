using GameBoost.MVVM.Core;
using MaterialDesignThemes.Wpf;

namespace GameBoost.MVVM.UserControls.Models
{
    public interface ISelectionButton
    {
        public string Title { get; }
        public PackIconKind Icon { get; }
    }
}
