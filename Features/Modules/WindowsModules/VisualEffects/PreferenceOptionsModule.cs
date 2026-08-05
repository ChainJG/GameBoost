using GameBoost.Core.Interfaces;
using GameBoost.Features.Modules.WindowsModules.VisualEffects.Options;
using GameBoost.Infrastructure.Registry;
using GameBoost.Core.Modules;
using GameBoost.Shared.Results;
using Microsoft.Win32;
using System.Diagnostics;

namespace GameBoost.Features.Modules.WindowsModules.VisualEffects
{
    public sealed class PreferenceOptionsModule : IInputActionModule<object>, IRecommendedActionModule, IRequiredModule
    {
        public string Name => "Preference Options";

        #region Recommendation
        public RecommendationPriority RecommendationPriority =>
            RecommendationPriority.Medium;

        public object? RecommendedValue =>
            PreferenceOption.Performance;

        public string RecommendationReason =>
            "Windows Advanced Performance Options is recommended to be set to Adjust for best performance on gaming-focused systems because it disables unnecessary visual effects and animations, reducing small UI overhead and keeping the system more performance-focused.";

        public bool IsRecommendedValue(object? currentValue)
        {
            return currentValue is PreferenceOption.Performance;
        }
        #endregion

        #region Requirements
        public bool SystemReboot => true;

        public bool Admin => false;
        #endregion

        private static readonly byte[] AppearanceMask =
        [
            158, 62, 7, 128, 18, 0, 0, 0
        ];

        private static readonly byte[] PerformanceMask =
        [
            144, 18, 3, 128, 16, 0, 0, 0
        ];

        private static readonly RegistryEditInfo UserPreferencesMaskEdit =
            CreateEdit(
                RegistryConstants.DesktopPath,
                "UserPreferencesMask");

        private static readonly RegistryEditInfo VisualFxSettingEdit =
            CreateEdit(
                RegistryConstants.VisualEffectsPath,
                "VisualFXSetting");

        private static readonly IReadOnlyDictionary<PreferenceOption, PreferenceOptions> Presets = new Dictionary<PreferenceOption, PreferenceOptions>
        {
            [PreferenceOption.Appearance] = new()
            {
                Option = PreferenceOption.Appearance,
                DisplayName = "Appearance",
                Description = "Enable Windows visual effects for the best appearance.",
                RegistryValues =
                [
                    Value(RegistryConstants.VisualEffectsPath, "VisualFXSetting", 1),
                    Value(RegistryConstants.DesktopPath, "UserPreferencesMask", AppearanceMask),
                    Value(RegistryConstants.WindowMetricsPath, "MinAnimate", "1"),
                    Value(RegistryConstants.ExplorerAdvancedPath, "TaskbarAnimations", 1),
                    Value(RegistryConstants.DWMPath, "EnableAeroPeek", 1),
                    Value(RegistryConstants.DWMPath, "AlwaysHibernateThumbnails", 1),
                    Value(RegistryConstants.ExplorerAdvancedPath, "IconsOnly", 0),
                    Value(RegistryConstants.ExplorerAdvancedPath, "ListviewAlphaSelect", 1),
                    Value(RegistryConstants.DesktopPath, "DragFullWindows", "1"),
                    Value(RegistryConstants.DesktopPath, "FontSmoothing", "2"),
                    Value(RegistryConstants.ExplorerAdvancedPath, "ListviewShadow", 1)
                ]
            },

            [PreferenceOption.Performance] = new()
            {
                Option = PreferenceOption.Performance,
                DisplayName = "Performance",
                Description = "Disable heavier visual effects while keeping thumbnails and font smoothing.",
                RegistryValues =
                [
                    Value(RegistryConstants.VisualEffectsPath, "VisualFXSetting", 3),
                    Value(RegistryConstants.DesktopPath, "UserPreferencesMask", PerformanceMask),
                    Value(RegistryConstants.WindowMetricsPath, "MinAnimate", "0"),
                    Value(RegistryConstants.ExplorerAdvancedPath, "TaskbarAnimations", 0),
                    Value(RegistryConstants.DWMPath, "EnableAeroPeek", 0),
                    Value(RegistryConstants.DWMPath, "AlwaysHibernateThumbnails", 0),
                    Value(RegistryConstants.ExplorerAdvancedPath, "IconsOnly", 0),
                    Value(RegistryConstants.ExplorerAdvancedPath, "ListviewAlphaSelect", 0),
                    Value(RegistryConstants.DesktopPath, "DragFullWindows", "0"),

                    // Keep enabled for readability.
                    Value(RegistryConstants.DesktopPath, "FontSmoothing", "2"),

                    Value(RegistryConstants.ExplorerAdvancedPath, "ListviewShadow", 0)
                ]
            }
        };

        public Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var currentOption = GetCurrentPreferenceOption();

            return Task.FromResult(
                ActionRefreshResult.OptionsAndValue(
                    CreateOptions(currentOption),
                    currentOption,
                    GetDisplayName(currentOption)));
        }

        public Task<ModuleResult> ExecuteRecommendedAsync(CancellationToken token)
        {
            return ExecuteAsync(
                PreferenceOption.Performance,
                token);
        }

        public Task<ModuleResult> ExecuteAsync(object input, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                if (!TryGetSelectedOption(input, out var selectedOption))
                {
                    return Task.FromResult(
                        ModuleResult.Failed("Invalid preference option selected"));
                }

                var errors = ApplyPreset(selectedOption);

                if (errors.Count > 0)
                {
                    return Task.FromResult(
                        ModuleResult.Failed(string.Join(Environment.NewLine, errors)));
                }

                return Task.FromResult(
                    ModuleResult.Successful($"Visual effects changed to {GetDisplayName(selectedOption)}"));
            }
            catch (OperationCanceledException)
            {
                return Task.FromResult(
                    ModuleResult.Failed("Preference options change was cancelled"));
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error in PreferenceOptionsModule execute: {ex.Message}");
#endif
                return Task.FromResult(
                    ModuleResult.Failed(ex.Message));
            }
        }

        private static IReadOnlyList<ActionOption> CreateOptions(PreferenceOption currentOption)
        {
            return Presets.Values
                .Select(preset => new ActionOption
                {
                    DisplayText = preset.DisplayName,
                    Value = preset.Option,
                    Description = preset.Description,
                    IsDefaultSelected = preset.Option == currentOption
                })
                .ToList();
        }

        private static List<string> ApplyPreset(
            PreferenceOption option)
        {
            if (!Presets.TryGetValue(option, out var preset))
                return [$"Unsupported preference option: {option}"];

            var errors = new List<string>();

            foreach (var registryValue in preset.RegistryValues)
            {
                var result = RegistryHelper.SetValue(
                    registryValue.Edit,
                    registryValue.Value);

                if (!result.Success)
                    errors.Add($"{registryValue.Edit.Key}: {result.Message}");
            }

            return errors;
        }

        private static PreferenceOption GetCurrentPreferenceOption()
        {
            var maskResult = RegistryHelper.GetValue(UserPreferencesMaskEdit);

            if (maskResult.Success && maskResult.Value is byte[] mask)
            {
                if (ByteArraysMatch(mask, PerformanceMask))
                    return PreferenceOption.Performance;

                if (ByteArraysMatch(mask, AppearanceMask))
                    return PreferenceOption.Appearance;
            }

            var visualFxResult = RegistryHelper.GetValue(VisualFxSettingEdit);

            if (visualFxResult.Success)
            {
                var value = visualFxResult.Value?.ToString();

                return value switch
                {
                    "3" => PreferenceOption.Performance,
                    "1" => PreferenceOption.Appearance,
                    _ => PreferenceOption.Appearance
                };
            }

            return PreferenceOption.Appearance;
        }

        private static bool TryGetSelectedOption(
            object? input,
            out PreferenceOption option)
        {
            if (input is PreferenceOption selectedOption)
            {
                option = selectedOption;
                return true;
            }

            if (Enum.TryParse(input?.ToString(), out PreferenceOption parsedOption))
            {
                option = parsedOption;
                return true;
            }

            option = default;
            return false;
        }

        private static string GetDisplayName(
            PreferenceOption option)
        {
            return Presets.TryGetValue(option, out var preset)
                ? preset.DisplayName
                : option.ToString();
        }

        private static RegistryPresetValue Value(
            string path,
            string key,
            object value)
        {
            return new RegistryPresetValue
            {
                Edit = CreateEdit(path, key),
                Value = value
            };
        }

        private static RegistryEditInfo CreateEdit(
            string path,
            string key)
        {
            return new RegistryEditInfo
            {
                Hive = RegistryHive.CurrentUser,
                Path = path,
                Key = key
            };
        }

        private static bool ByteArraysMatch(
            byte[] left,
            byte[] right)
        {
            return left.AsSpan().SequenceEqual(right);
        }
    }
}