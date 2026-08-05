using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.Registry;
using GameBoost.Core.Modules;
using GameBoost.Shared.Results;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GameBoost.Features.Modules.WindowsModules.Gaming
{
    public sealed class Win32PrioritySeparationModule : IInputActionModule<object>, IRecommendedActionModule, IRequiredModule
    {
        public string Name => "Foreground Priority Boost";

        #region IRequiredModule
        public bool SystemReboot => true;
        public bool Admin => true;
        #endregion

        #region IRecommendedActionModule
        public RecommendationPriority RecommendationPriority => RecommendationPriority.Medium;
        public object? RecommendedValue => ForegroundBoostMode.HighForegroundBoost;
        public string RecommendationReason =>
            "High foreground boost is recommended because it gives the active foreground app stronger CPU scheduling preference";
        public bool IsRecommendedValue(object? currentValue) => currentValue is ForegroundBoostMode mode && mode == ForegroundBoostMode.HighForegroundBoost;
        #endregion

        private static readonly RegistryEditInfo Win32PrioritySeparationEdit = new()
        {
            Hive = RegistryHive.LocalMachine,
            Path = RegistryConstants.PriorityControlPath,
            Key = "Win32PrioritySeparation",
            Kind = RegistryValueKind.DWord
        };

        private static readonly IReadOnlyDictionary<ForegroundBoostMode, ForegroundBoostPreset> Presets = new Dictionary<ForegroundBoostMode, ForegroundBoostPreset>
        {
            [ForegroundBoostMode.HighForegroundBoost] = new()
            {
                Mode = ForegroundBoostMode.HighForegroundBoost,
                DisplayName = "High Foreground boost",
                Description = "Prioritises the active foreground application more strongly. Common gaming-focused preset",
                RegistryValue = 0x26
            },

            [ForegroundBoostMode.MediumForegroundBoost] = new()
            {
                Mode = ForegroundBoostMode.MediumForegroundBoost,
                DisplayName = "Medium Foreground boost",
                Description = "Gives the active foreground application a moderate scheduling boost",
                RegistryValue = 0x25
            },

            [ForegroundBoostMode.NoForegroundBoost] = new()
            {
                Mode = ForegroundBoostMode.NoForegroundBoost,
                DisplayName = "No Foreground boost",
                Description = "Removes foreground boost preference and keeps scheduling more even",
                RegistryValue = 0x24
            }
        };

        public async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var currentMode = GetCurrentMode();

            return ActionRefreshResult.OptionsAndValue(CreateOptions(currentMode), currentMode, GetDisplayName(currentMode));
        }
        public Task<ModuleResult> ExecuteRecommendedAsync(CancellationToken token) => ExecuteAsync(ForegroundBoostMode.HighForegroundBoost, token);
        public async Task<ModuleResult> ExecuteAsync(object input, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                if (!TryGetSelectedMode(input, out var selectedMode))
                    return ModuleResult.Failed($"Invalid foreground boost mode: {input}");

                if (!Presets.TryGetValue(selectedMode, out var preset)) 
                    return ModuleResult.Failed($"No preset found for foreground boost mode: {selectedMode}");

                var result = RegistryHelper.SetValue(Win32PrioritySeparationEdit, preset.RegistryValue);

                if (!result.Success)
                    return ModuleResult.Failed($"Failed to set foreground priority boost: {result.Message}");

                return ModuleResult.Successful($"Foreground priority boost set to {preset.DisplayName}");
            }
            catch (OperationCanceledException)
            {
                return ModuleResult.Failed("Foreground priority boost change was cancelled");
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Failed to change Win32PrioritySeparation: {ex.Message}");
#endif

                return ModuleResult.Failed("Failed to change foreground priority boost");
            }
        }

        private static IReadOnlyList<ActionOption> CreateOptions(ForegroundBoostMode currentMode) =>
            [.. Presets.Values
                .Select(preset => new ActionOption
                {
                    DisplayText = preset.DisplayName,
                    Description = preset.Description,
                    Value = preset.Mode,
                    IsDefaultSelected = preset.Mode == currentMode
                })];

        private static bool TryGetSelectedMode(object? input, out ForegroundBoostMode mode)
        {
            if (input is ForegroundBoostMode selectedMode)
            {
                mode = selectedMode;
                return true;
            }

            if (Enum.TryParse(input?.ToString(), out ForegroundBoostMode parsedMode))
            {
                mode = parsedMode;
                return true;
            }

            mode = default;
            return false;
        }
        private static ForegroundBoostMode GetCurrentMode()
        {
            var result = RegistryHelper.GetValue(Win32PrioritySeparationEdit);

            if (!result.Success || result.Value is null)
                return ForegroundBoostMode.Unknown;

            var value = Convert.ToInt32(result.Value);

            return value switch
            {
                0x26 => ForegroundBoostMode.HighForegroundBoost,
                0x25 => ForegroundBoostMode.MediumForegroundBoost,
                0x24 => ForegroundBoostMode.NoForegroundBoost,
                _ => ForegroundBoostMode.Unknown
            };
        }

        private static string GetDisplayName(ForegroundBoostMode mode) =>
            Presets.TryGetValue(mode, out var preset)
                ? preset.DisplayName
                : mode.ToString();

        #region ForegroundBoost Presets
        private sealed class ForegroundBoostPreset
        {
            public required ForegroundBoostMode Mode { get; init; }

            public required string DisplayName { get; init; }

            public required string Description { get; init; }

            public required int RegistryValue { get; init; }
        }

        private enum ForegroundBoostMode
        {
            Unknown,
            NoForegroundBoost,
            MediumForegroundBoost,
            HighForegroundBoost
        }
        #endregion
    }

}
