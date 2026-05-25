using GameBoost.MVVM.SplashScreen;
using GameBoost.MVVM.Windows;
using System.Windows;

namespace GameBoost
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow();

            var splashWindow = new SplashScreenWindow();
            var splashViewModel = new SplashScreenViewModel();

            splashWindow.DataContext = splashViewModel;
            splashWindow.Show();

            splashViewModel.InitialisationComplete += (success) =>
            {
                //AppBootstrapper.Initialise(mainWindow);

                mainWindow.Show();
                splashWindow.Close();
            };

            await splashViewModel.InitialiseApplicationAsync();
        }
    }

}
