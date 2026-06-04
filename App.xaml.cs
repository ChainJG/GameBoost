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


            if (e.Args.Length >= 2 &&
                e.Args[0].Equals("--process-lookup", StringComparison.OrdinalIgnoreCase))
            {
                string selectedFilePath = e.Args[1];

                var processLookupWindow = new ProcessLookupWindow(selectedFilePath);
                processLookupWindow.Show();
                return;
            }

            //var delete = new ProcessLookupWindow(@"C:\Users\jcros\source\repos\GameBoostOld\GameBoost\bin\Debug\app.publish\GameBoost.exe");
            //delete.Show();
            //return;

            var startupService = new StartupService();

            var mainWindow = new MainWindow();

            var splashWindow = new SplashScreenWindow();
            var splashViewModel = new SplashScreenViewModel(startupService);

            splashWindow.DataContext = splashViewModel;
            splashWindow.Show();

            splashViewModel.StartupCompleted += (success) =>
            {
                mainWindow.ViewModel.InitialiseStartupTitleBarActions();

                mainWindow.Show();
                splashWindow.Close();
            };

            await splashViewModel.InitialiseApplicationAsync();
        }
    }

}
