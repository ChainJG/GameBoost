using GameBoost.Application;
using GameBoost.Application.Diagnostics;
using GameBoost.Application.Startup;
using GameBoost.Core.Debugger;
using GameBoost.MVVM.SplashScreen;
using GameBoost.MVVM.ViewModels;
using GameBoost.MVVM.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace GameBoost
{
    public partial class App : System.Windows.Application
    {
        private IServiceProvider? _services;

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

            // Diagnostics are always on so field failures leave a trace; Release only
            // records failures to keep the log small, Debug records everything.
            GameBoostContext.Diagnostic = new DiagnosticService(
                new DiagnosticOptions
                {
                    Enabled = true,
#if DEBUG
                    IncludeSuccessfulOperations = true
#else
                    IncludeSuccessfulOperations = false
#endif
                });

            // Nothing should reach the user as an unexplained crash.
            DispatcherUnhandledException += (_, args) =>
            {
                GameBoostDebug.Error("Unhandled dispatcher exception", args.Exception);
                args.Handled = false;
            };

            _services = GameBoostServiceRegistration.BuildServiceProvider();

            var startupService = _services.GetRequiredService<StartupService>();
            var mainViewModel = _services.GetRequiredService<MainViewModel>();

            var mainWindow = new MainWindow(mainViewModel);

            var splashWindow = new SplashScreenWindow();
            var splashViewModel = new SplashScreenViewModel(startupService);

            splashWindow.DataContext = splashViewModel;
            splashWindow.Show();

            splashViewModel.StartupCompleted += success =>
            {
                if (!success)
                {
                    splashWindow.Close();
                    Shutdown();
                    return;
                }

                mainWindow.Show();
                splashWindow.Close();
            };

            await splashViewModel.InitialiseApplicationAsync(
                initialiseMainViewModel: async (progress, token) =>
                {
                    await mainViewModel.InitialiseStartup(progress, token);
                });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            (_services as IDisposable)?.Dispose();

            base.OnExit(e);
        }
    }
}
