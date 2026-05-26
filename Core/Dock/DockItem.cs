using MaterialDesignThemes.Wpf;

namespace GameBoost.Core.Dock
{
    public class DockItem(string title, PackIconKind icon, object viewModel)
    {
        public string Title { get; set; } = title;
        public PackIconKind Icon { get; set; } = icon;
        public object ViewModel { get; set; } = viewModel;
    }
}
