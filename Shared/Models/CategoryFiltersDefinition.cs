using MaterialDesignThemes.Wpf;

namespace GameBoost.Shared.Models
{
    public class CategoryFiltersDefinition
    {
        public required string Category { get; init; }
        public required PackIconKind Icon {  get; init; }
    }
}
