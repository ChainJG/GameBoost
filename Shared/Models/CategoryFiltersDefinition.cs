using GameBoost.MVVM.Core;
using MaterialDesignThemes.Wpf;

namespace GameBoost.Shared.Models
{
    public class CategoryFiltersDefinition : ObservableObject
    {
        public required string Category { get; init; }
        public required PackIconKind Icon {  get; init; }
    }
}
