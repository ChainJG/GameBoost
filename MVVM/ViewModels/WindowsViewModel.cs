using GameBoost.Features.Modules.Windows.DirectXUserGlobal;
using GameBoost.Features.Modules.Windows.Gaming;
using GameBoost.Features.Modules.Windows.Privacy_Security;
using GameBoost.Features.Modules.Windows.Taskbar;
using GameBoost.Features.Modules.Windows.VisualEffects;
using GameBoost.Infrastructure.Registry.DirectXUserGlobal;
using GameBoost.MVVM.ViewModels.Shared.Selection;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions;
using MaterialDesignThemes.Wpf;

namespace GameBoost.MVVM.ViewModels
{
    public class WindowsViewModel : SelectionViewModel
    {

        public WindowsViewModel(string pageTitle)
        {
            PageTitle = pageTitle;

            FeatureCards =
            [
                WindowsDefender(),
                Gaming(),
                VisualEffects(),
                Taskbar(),
                PrivacyAndSecurity(),
            ];

            LoadGpuPreferencesGames();
        }

        private static SelectionFeatureViewModel WindowsDefender()
        {
            var security = new SelectionFeatureViewModel
            {
                Title = "Windows Security",
                Description = "Real-time protection, and firewall settings is crucial for maintaining system security, preventing malware attacks, and ensuring data integrity",
                Icon = PackIconKind.Security,
            };

            security.AddActions(
            [
                new MultipurposeActionCardViewModel()
                {
                    Title = "Real Time Protection",
                    Icon = PackIconKind.SmokeDetector,
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Firewall",
                    Icon = PackIconKind.Firebase,
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Core Memory Integrity",
                    Icon = PackIconKind.Memory,
                }
            ]);

            return security;
        }


        private void LoadGpuPreferencesGames()
       {
            var gamePreferences = new SelectionFeatureViewModel
            {
                Title = "Application Preferences",
                Description = $"Configures per-game Windows graphics preferences, GPU performance modes, and gaming optimizations for reduced latency and improved frame consistency",
                Icon = PackIconKind.AppBadge,
            };

            var discoveredGames = DirectXUserGpuPreferences.GetDirectXUserGpuPreferencesGames();

            foreach (var regGame in discoveredGames)
            {
                gamePreferences.AddAction(
                    new MultipurposeActionCardViewModel()
                    {
                        Title = regGame.Message,
                        Icon = PackIconKind.Controller,
                        Module = new GpuPreferencesGameModule(regGame),
                    }
                );
            }

            if (discoveredGames.Count >= 1)
                FeatureCards.Add(gamePreferences);
       }
        private static SelectionFeatureViewModel Taskbar()
        {
            var taskbar = new SelectionFeatureViewModel
            {
                Title = "Taskbar",
                Description = "Customize which features appear on your Windows taskbar, such as the search bar, widgets, and system buttons",
                Icon = PackIconKind.TableRow,
            };

            taskbar.AddActions(
            [
                new MultipurposeActionCardViewModel()
                {
                    Title = "Search Bar",
                    Icon = PackIconKind.Search,
                    Module = new SearchboxTaskbarModule(),
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Task View",
                    Icon = PackIconKind.ImageFilterNone,
                    Module = new TaskViewTaskbarModule(),
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Widgets",
                    Icon = PackIconKind.Widgets,
                    Module = new WidgetsTaskbarModule(),
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "End Task",
                    Icon = PackIconKind.ContainEnd,
                    Module = new EndTaskTaskbarModule(),
                },
            ]);


            return taskbar;
        }
        private static SelectionFeatureViewModel VisualEffects()
        {
            var visualEffects = new SelectionFeatureViewModel
            {
                Title = "Visual Effects",
                Description = "Manage and customize system and application themes, including dark mode settings and other appearance options to enhance your user experience",
                Icon = PackIconKind.Theme,
            };

            visualEffects.AddActions(
            [
                new ComboBoxActionCardViewModel()
                {
                    Title = "Preference Options",
                    Icon = PackIconKind.VectorPolyline,
                    Module = new PreferenceOptionsModule(),
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "System Theme Mode",
                    Icon = PackIconKind.Computer,
                    Module = new SystemThemeModeModule(),
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Transparency Effects",
                    Icon = PackIconKind.VectorUnion,
                    Module = new TransparencyEffectModule(),
                },
            ]);


            return visualEffects;
        }
        private static SelectionFeatureViewModel Gaming()
        {
            var gaming = new SelectionFeatureViewModel
            {
                Title = "Gaming",
                Description = "Optimise Windows 11 for gaming performance with Game Mode, VRR, DirectStorage, and controller optimizations",
                Icon = PackIconKind.Gamepad,
            };

            gaming.AddActions(
            [
                new MultipurposeActionCardViewModel()
                {
                    Title = "Game Mode",
                    Icon = PackIconKind.Controller,
                    Module = new GameModeModule(),
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Hardware Acceleration GPU Scheduling",
                    Icon = PackIconKind.WindowRestore,
                    Module = new HardwareAcceleratedGpuScheduling(),
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Windowed Game Optimization",
                    Icon = PackIconKind.WindowRestore,
                    Module = new WindowedGameOptimizationModule(),
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Variable Refresh Rate (VRR)",
                    Icon = PackIconKind.MonitorShimmer,
                    Module = new VariableRefreshRateModule(),
                },
            ]);

            return gaming;
        }
        private static SelectionFeatureViewModel PrivacyAndSecurity()
        {
            var privacyAndSecurity = new SelectionFeatureViewModel
            {
                Title = "Privacy & Security",
                Description = "Disabling telemetry, Cortana, and other privacy settings is crucial for enhancing user privacy, reducing data collection, and improving system performance",
                Icon = PackIconKind.ShieldEdit,
            };

            privacyAndSecurity.AddActions(
            [
                new MultipurposeActionCardViewModel()
                {
                    Title = "Telemetry",
                    Icon = PackIconKind.ClipboardPlus,
                    Module = new TelemetryModule(),
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Biometrics",
                    Icon = PackIconKind.HandWash,
                    Module = new BiometricsModule(),
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "App Permissions",
                    Icon = PackIconKind.Widgets,
                    Module = new AppPermissionsModule(),
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "User Tracking",
                    Icon = PackIconKind.MapMarkerRadius,
                    Module = new UserTrackingModule(),
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Feedback Requests",
                    Icon = PackIconKind.CardAccountDetails,
                    Module = new FeedbackRequestsModule(),
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Error Reporting",
                    Icon = PackIconKind.AlertRemove,
                    Module = new ErrorReportingModule(),
                },
            ]);

            return privacyAndSecurity;
        }
    }
}
