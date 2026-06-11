using GameBoost.Application;
using GameBoost.Application.Selection.Builders;
using GameBoost.Application.Selection.Registries;
using GameBoost.MVVM.ViewModels.Shared.Selection;

namespace GameBoost.MVVM.ViewModels
{
    public class WindowsViewModel : SelectionViewModel
    {
        public WindowsViewModel(string pageTitle, GameBoostUIServices uiService) : base(uiService)
        {
            PageTitle = pageTitle;

            FeatureCards =
            [
                ..SelectionFeatureBuilder.BuildMany(
                    WindowsFeatureRegistry.GetFeatures())
            ];
        }
    }
}