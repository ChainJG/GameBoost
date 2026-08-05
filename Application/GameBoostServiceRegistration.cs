using GameBoost.Application.Operations;
using GameBoost.Application.Selection.Services;
using GameBoost.Application.Startup;
using GameBoost.Application.Titlebar;
using GameBoost.Features.Storage.Services;
using GameBoost.MVVM.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace GameBoost.Application
{
    /// <summary>
    /// The application's composition root. Everything the app needs is registered
    /// here, in one place, so the dependency graph is readable end to end.
    /// </summary>
    public static class GameBoostServiceRegistration
    {
        private const int MaxConcurrentModuleRefreshes = 4;

        public static IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            AddApplicationServices(services);
            AddFeatureServices(services);
            AddViewModels(services);

            return services.BuildServiceProvider();
        }

        // Cross-cutting application state and coordination.
        private static void AddApplicationServices(IServiceCollection services)
        {
            services.AddSingleton<GlobalOperationService>();
            services.AddSingleton<TitleBarActionService>();
            services.AddSingleton<StartupStateService>();
            services.AddSingleton<StartupService>();
            services.AddSingleton<StartupNotificationService>();

            services.AddSingleton<SelectionScanNotificationService>();
            services.AddSingleton<SelectionExecutionRequirementService>();
            services.AddSingleton<RecommendedActionService>();

            services.AddSingleton(
                _ => new SelectionActionRefreshService(MaxConcurrentModuleRefreshes));
        }

        // Feature-level services that carry no application state.
        private static void AddFeatureServices(IServiceCollection services)
        {
            services.AddSingleton<StorageScanService>();
        }

        // Page ViewModels are singletons: the dock keeps every page alive for the
        // lifetime of the window, and module state must survive navigation.
        private static void AddViewModels(IServiceCollection services)
        {
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<WindowsViewModel>();
            services.AddSingleton<SystemViewModel>();
            services.AddSingleton<StorageViewModel>();
            services.AddSingleton<ApplicationInstallerViewModel>();

            services.AddSingleton<MainViewModel>();
        }
    }
}
