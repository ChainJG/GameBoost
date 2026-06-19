using GameBoost.Application;
using GameBoost.MVVM.Core;

namespace GameBoost.MVVM.ViewModels
{
    public sealed class ApplicationInstallerViewModel(GameBoostUIServices uiService) : ObservableObject
    {
        private readonly GameBoostUIServices _uiService = uiService;
    }
}
