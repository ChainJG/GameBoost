using GameBoost.Core.Dock;
using GameBoost.MVVM.Windows;

namespace GameBoost.Application
{
    public class AppBootstrapper
    {
        public static void Initialise(MainWindow mainWindow)
        {
            GameBoostContext.Dock =
                new DockController(mainWindow.DockRoot);
        }
    }
}
