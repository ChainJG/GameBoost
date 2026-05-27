using GameBoost.Application;
using GameBoost.Application.Startup;
using GameBoost.MVVM.SplashScreen;
using GameBoost.MVVM.Windows;
using System.Windows;

namespace GameBoost
{
    public partial class App : System.Windows.Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var startupService = new StartupService();

            var mainWindow = new MainWindow();

            var splashWindow = new SplashScreenWindow();
            var splashViewModel = new SplashScreenViewModel(startupService);

            splashWindow.DataContext = splashViewModel;
            splashWindow.Show();

            splashViewModel.StartupCompleted += (success) =>
            {
                mainWindow.Show();
                splashWindow.Close();
            };

            await splashViewModel.InitialiseApplicationAsync();
        }
    }

}
