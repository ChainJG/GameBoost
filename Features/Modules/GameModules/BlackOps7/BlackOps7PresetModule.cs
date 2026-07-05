using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.GameConfigs.BlackOps7;
using GameBoost.Infrastructure.GameConfigs.BlackOps7.Catalog;
using GameBoost.MVVM.ViewModels.Shared.Selection.Actions.Misc;
using GameBoost.Shared.Results;

namespace GameBoost.Features.Modules.GameModules.BlackOps7
{
    public sealed class BlackOps7PresetModule : IInputActionModule<object>
    {
        private readonly BlackOps7ConfigService _configService = new();

        public string Name => "Presets";

        public Task<ActionRefreshResult> RefreshStatusAsync(
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var presets = BlackOps7PresetCatalog.GetPresets();

            var options = presets
                .Select(preset => new ActionOptionViewModel<object>
                {
                    DisplayText = preset.DisplayName,
                    Description = preset.Description,
                    Value = preset.Id,
                    IsDefaultSelected = preset.Id == "competitive_fps"
                })
                .ToList();

            return Task.FromResult(
                ActionRefreshResult.OptionsOnly(
                    options,
                    "Select a Black Ops 7 preset"));
        }

        public async Task<ModuleResult> ExecuteAsync(
            object input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var presetId = input?.ToString();

            if (string.IsNullOrWhiteSpace(presetId))
                return ModuleResult.Failed("No preset selected");

            var preset = BlackOps7PresetCatalog
                .GetPresets()
                .FirstOrDefault(p => string.Equals(
                    p.Id,
                    presetId,
                    StringComparison.OrdinalIgnoreCase));

            if (preset is null)
                return ModuleResult.Failed("Selected preset was not found");

            var result = await BlackOps7ConfigService.ApplyPresetAsync(
                preset,
                token);

            return result.Success
                ? ModuleResult.Successful(result.Message)
                : ModuleResult.Failed(result.Message);
        }
    }
}