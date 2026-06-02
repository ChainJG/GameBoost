using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.Registry;
using GameBoost.MVVM.ViewModels.Shared.Selection.Actions.Misc;
using GameBoost.Shared.Results;
using Microsoft.Win32;
using System.Diagnostics;

namespace GameBoost.Features.Modules.Windows.VisualEffects
{
    public sealed class PreferenceOptionsModule : IInputActionModule<object>
    {
        public string Name => "Preference Options";

        private static RegistryEditInfo[] RegistryData =>
        [
            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.VisualEffectsPath,
                Key = "VisualFXSetting",
                EnabledValue = 0,
                DisabledValue = 3
            }, // Setting the overall type to Custom 

            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.WindowMetricsPath,
                Key = "MinAnimate",
                EnabledValue = "1",
                DisabledValue = "0"
            }, // Animate Windows When Minimizing (Disable)

            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.ExplorerAdvancedPath,
                Key = "TaskbarAnimations",
                EnabledValue = 1,
                DisabledValue = 0
            }, // Animations in Taskbar (Disable)

            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.DWMPath,
                Key = "EnableAeroPeek",
                EnabledValue = 1,
                DisabledValue = 0
            }, // Enable Peak (Disable)

            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.DWMPath,
                Key = "AlwaysHibernateThumbnails",
                EnabledValue = 1,
                DisabledValue = 0
            }, // Save taskbar thumbnails previews (Disable)

            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.ExplorerAdvancedPath,
                Key = "IconsOnly",
                EnabledValue = 1,
                DisabledValue = 0
            }, // Show thumbnails instead of icons (Enable)

            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.ExplorerAdvancedPath,
                Key = "ListviewAlphaSelect",
                EnabledValue = 1,
                DisabledValue = 0
            }, // Show Translucent Selection Rectangle (Disable)

            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.DesktopPath,
                Key = "DragFullWindows",
                EnabledValue = 1,
                DisabledValue = 0
            }, // Show Window Contents While Dragging (Disable)

            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.DesktopPath,
                Key = "FontSmoothing",
                EnabledValue = "0",
                DisabledValue = "2"
            }, // Smooth Edges of Screen fonts (Enable)

            new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.ExplorerAdvancedPath,
                Key = "ListviewShadow",
                EnabledValue = 1,
                DisabledValue = 0
            }, // Use Drop Shadow For Icons Lables On the desktop (Disable)
        ];

        private readonly RegistryEditInfo REGISTRY_MASK = new()
        {
            Hive = RegistryHive.CurrentUser,
            Path = RegistryConstants.DesktopPath,
            Key = "UserPreferencesMask",
        };
        private static readonly byte[] DEFAULT_MASK = [158, 62, 7, 128, 18, 0, 0, 0];
        private static readonly byte[] PERFORMANCE_MASK = [144, 18, 3, 128, 16, 0, 0, 0];

        public Task<ModuleResult> ExecuteAsync(object input, CancellationToken token)
        {
            return Task.FromResult(ModuleResult.Failed());
        }

        public async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            return await Task.FromResult(ActionRefreshResult.OptionsOnly(await GetOptionsAsync(token)));
        }
        public async Task<IReadOnlyList<ActionOptionViewModel<object>>> GetOptionsAsync(
            CancellationToken token)
        {
            return
            [
                new ActionOptionViewModel<object>
                {
                    DisplayText = "Apperance",
                    Value = DEFAULT_MASK
                },
                new ActionOptionViewModel<object>
                {
                    DisplayText = "Performance",
                    Value = PERFORMANCE_MASK
                },
            ];
        }
    }
}
