using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.Registry;
using GameBoost.MVVM.ViewModels.Shared.Selection.Actions.Misc;
using GameBoost.Shared.Results;
using Microsoft.Win32;
using System.Diagnostics;

namespace GameBoost.Features.Modules.Windows.VisualEffects
{
    public sealed class PreferenceOptionsModule : IInputActionModule<object>, IRecommendedActionModule
    {
        public string Name => "Preference Options";

        public object? RecommendedValue => PreferenceOption.Performance;
        public string RecommendationReason => "";
        public bool IsRecommendedValue(object? currentValue) =>
            currentValue is PreferenceOption option && option == PreferenceOption.Performance;


        private readonly RegistryEditInfo UserPreferencesMaskEdit = new()
        {
            Hive = RegistryHive.CurrentUser,
            Path = RegistryConstants.DesktopPath,
            Key = "UserPreferencesMask",
        };

        private static readonly byte[] AppearanceMask = [158, 62, 7, 128, 18, 0, 0, 0];
        private static readonly byte[] PerformanceMask = [144, 18, 3, 128, 16, 0, 0, 0];
        private static IReadOnlyList<ActionOptionViewModel<object>> CreateOptions(
            PreferenceOption currentOption)
        {
            return
            [
                new ActionOptionViewModel<object>
                {
                    DisplayText = "Appearance",
                    Value = PreferenceOption.Appearance,
                    Description = "Enable Windows visual effects for the best appearance",
                    IsDefaultSelected = currentOption == PreferenceOption.Appearance
                },

                new ActionOptionViewModel<object>
                {
                    DisplayText = "Performance",
                    Value = PreferenceOption.Performance,
                    Description = "Disable heavier visual effects while keeping thumbnails and font smoothing",
                    IsDefaultSelected = currentOption == PreferenceOption.Performance
                }
            ];
        }

        public async Task<ModuleResult> ExecuteAsync(object input, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var selectedOption = GetSelectedOption(input);

                var errors = selectedOption switch
                {
                    PreferenceOption.Appearance => ApplyAppearancePreset(),
                    PreferenceOption.Performance => ApplyPerformancePreset(),
                    _ => [$"Unsupported preference option: {selectedOption}"]
                };

                return ModuleResult.Successful($"Visual effects changed to {selectedOption}");
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error in PreferenceOptionsModule execute: {ex.Message}");
#endif
                return ModuleResult.Failed(ex.Message);
            }
        }

        private static List<string> ApplyAppearancePreset()
        {
            var errors = new List<string>();

            ApplyRegistryValue(
                errors,
                RegistryConstants.VisualEffectsPath,
                "VisualFXSetting",
                1);

            ApplyRegistryValue(
                errors,
                RegistryConstants.DesktopPath,
                "UserPreferencesMask",
                AppearanceMask);

            ApplyRegistryValue(
                errors,
                RegistryConstants.WindowMetricsPath,
                "MinAnimate",
                "1");

            ApplyRegistryValue(
                errors,
                RegistryConstants.ExplorerAdvancedPath,
                "TaskbarAnimations",
                1);

            ApplyRegistryValue(
                errors,
                RegistryConstants.DWMPath,
                "EnableAeroPeek",
                1);

            ApplyRegistryValue(
                errors,
                RegistryConstants.DWMPath,
                "AlwaysHibernateThumbnails",
                1);

            ApplyRegistryValue(
                errors,
                RegistryConstants.ExplorerAdvancedPath,
                "IconsOnly",
                0);

            ApplyRegistryValue(
                errors,
                RegistryConstants.ExplorerAdvancedPath,
                "ListviewAlphaSelect",
                1);

            ApplyRegistryValue(
                errors,
                RegistryConstants.DesktopPath,
                "DragFullWindows",
                "1");

            ApplyRegistryValue(
                errors,
                RegistryConstants.DesktopPath,
                "FontSmoothing",
                "2");

            ApplyRegistryValue(
                errors,
                RegistryConstants.ExplorerAdvancedPath,
                "ListviewShadow",
                1);

            ApplyRegistryValue(
                errors,
                RegistryConstants.DesktopPath,
                "UserPreferencesMask",
                AppearanceMask);

            return errors;
        }

        private static List<string> ApplyPerformancePreset()
        {
            var errors = new List<string>();

            ApplyRegistryValue(
                errors,
                RegistryConstants.VisualEffectsPath,
                "VisualFXSetting",
                3);

            ApplyRegistryValue(
                errors,
                RegistryConstants.DesktopPath,
                "UserPreferencesMask",
                PerformanceMask);

            ApplyRegistryValue(
                errors,
                RegistryConstants.WindowMetricsPath,
                "MinAnimate",
                "0");

            ApplyRegistryValue(
                errors,
                RegistryConstants.ExplorerAdvancedPath,
                "TaskbarAnimations",
                0);

            ApplyRegistryValue(
                errors,
                RegistryConstants.DWMPath,
                "EnableAeroPeek",
                0);

            ApplyRegistryValue(
                errors,
                RegistryConstants.DWMPath,
                "AlwaysHibernateThumbnails",
                0);

            ApplyRegistryValue(
                errors,
                RegistryConstants.ExplorerAdvancedPath,
                "IconsOnly",
                0);

            ApplyRegistryValue(
                errors,
                RegistryConstants.ExplorerAdvancedPath,
                "ListviewAlphaSelect",
                0);

            ApplyRegistryValue(
                errors,
                RegistryConstants.DesktopPath,
                "DragFullWindows",
                "0");

            // Keep enabled for readability.
            ApplyRegistryValue(
                errors,
                RegistryConstants.DesktopPath,
                "FontSmoothing",
                "2");

            ApplyRegistryValue(
                errors,
                RegistryConstants.ExplorerAdvancedPath,
                "ListviewShadow",
                0);

            ApplyRegistryValue(
                errors,
                RegistryConstants.DesktopPath,
                "UserPreferencesMask",
                PerformanceMask);

            return errors;
        }

        private static void ApplyRegistryValue(
            List<string> errors,
            string path,
            string key,
            object value)
        {
            var edit = new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = path,
                Key = key
            };

            var result = RegistryHelper.SetValue(edit, value);

            if (!result.Success)
                errors.Add($"{key}: {result.Message}");
        }

        private static PreferenceOption GetSelectedOption(object input)
        {
            if (input is PreferenceOption option)
                return option;

            if (Enum.TryParse(input?.ToString(), out PreferenceOption parsedOption))
                return parsedOption;

            return PreferenceOption.Appearance;
        }

        public Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var currentOption = GetCurrentPreferenceOption();
            var options = CreateOptions(currentOption);

            return Task.FromResult(
                ActionRefreshResult.OptionsAndValue(
                    options,
                    currentOption,
                    currentOption.ToString()));
        }

        private PreferenceOption GetCurrentPreferenceOption()
        {
            var maskResult = RegistryHelper.GetValue(UserPreferencesMaskEdit);

            if (maskResult.Success && maskResult.Value is byte[] mask)
            {
                if (ByteArraysMatch(mask, PerformanceMask))
                    return PreferenceOption.Performance;
                else 
                    return PreferenceOption.Appearance;
            }

            var visualFxResult = RegistryHelper.GetValue(new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = RegistryConstants.VisualEffectsPath,
                Key = "VisualFXSetting"
            });

            if (visualFxResult.Success)
            {
                var value = visualFxResult.Value?.ToString();

                return value switch
                {
                    "2" => PreferenceOption.Performance,
                    "1" => PreferenceOption.Appearance,
                    _ => PreferenceOption.Appearance
                };
            }

            return PreferenceOption.Appearance;
        }

        private static bool ByteArraysMatch(byte[] left, byte[] right)
        {
            if (left.Length != right.Length)
                return false;

            for (var index = 0; index < left.Length; index++)
            {
                if (left[index] != right[index])
                    return false;
            }

            return true;
        }

    }

    public enum PreferenceOption
    {
        Appearance,
        Performance
    }
}
