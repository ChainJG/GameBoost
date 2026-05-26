using GameBoost.MVVM.Core;
using MaterialDesignThemes.Wpf;

namespace GameBoost.MVVM.UserControls.Models
{
    public class SelectionResultCard : ObservableObject, ISelectionButton
    {
        public required string Title { get; set; }
        public required PackIconKind Icon { get; set; }
    }
}
