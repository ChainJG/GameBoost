using GameBoost.Features.Modules.Windows.Devices.Mouse;
using GameBoost.Features.Modules.Windows.Gaming;
using GameBoost.Features.Modules.Windows.PowerPlan;
using GameBoost.Features.Modules.Windows.Privacy_Security;
using GameBoost.Features.Modules.Windows.VisualEffects;
using GameBoost.MVVM.ViewModels.Shared.Selection;
using GameBoost.MVVM.ViewModels.Shared.Selection.Actions.Misc;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions;
using MaterialDesignThemes.Wpf;
using System.Drawing;

namespace GameBoost.MVVM.ViewModels
{
    public class WindowsViewModel : SelectionViewModel
    {

        public WindowsViewModel(string pageTitle)
        {
            PageTitle = pageTitle;

            FeatureCards =
            [
                Gaming(),
                VisualEffects(),
                PrivacyAndSecurity(),
            ];
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
                    Icon = PackIconKind.Gamepad,
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
