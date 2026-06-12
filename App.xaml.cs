using GameBoost.Application;
using GameBoost.Application.Diagnostics;
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
                var selectedFilePath = e.Args[1];

                var processLookupWindow = new ProcessLookupWindow(selectedFilePath);
                processLookupWindow.Show();

                return;
            }

#if DEBUG
            GameBoostContext.Diagnostic = new DiagnosticService(
                new DiagnosticOptions
                {
                    Enabled = true,
                    IncludeSuccessfulOperations = true
                });
#else
GameBoostDiagnostics.Current = DiagnosticService.Disabled;
#endif

            var startupService = new StartupService();

            var splashWindow = new SplashScreenWindow();
            var splashViewModel = new SplashScreenViewModel(startupService);

            splashWindow.DataContext = splashViewModel;
            splashWindow.Show();

            splashViewModel.StartupCompleted += async success =>
            {
                var mainWindow = new MainWindow();

                await mainWindow.ViewModel.InitialiseStartup();

                mainWindow.Show();
                splashWindow.Close();
            };

            await splashViewModel.InitialiseApplicationAsync();
        }
    }
}