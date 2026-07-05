using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.GameConfigs.BlackOps7;
using GameBoost.Infrastructure.GameConfigs.BlackOps7.Settings;
using GameBoost.Shared.Results;
using System.Globalization;

namespace GameBoost.Features.Modules.GameModules.BlackOps7
{
    public sealed class BlackOps7GroupedFloatSliderSettingModule
        : InputActionModuleBase<double>
    {
        private readonly BlackOps7ConfigService _configService = new();
        private readonly string _displayName;
        private readonly IReadOnlyList<string> _settingNames;

        public BlackOps7GroupedFloatSliderSettingModule(
            string displayName,
            IReadOnlyList<string> settingNames)
        {
            _displayName = displayName;
            _settingNames = settingNames;
        }

        public override string Name => _displayName;

        public override async Task<ActionRefreshResult> RefreshStatusAsync(
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var values = new List<double>();

            foreach (var settingName in _settingNames)
            {
                var rawValue = await BlackOps7ConfigService.GetSettingValueAsync(
                    settingName,
                    token);

                if (double.TryParse(
                        rawValue,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out var parsedValue))
                {
                    values.Add(Math.Clamp(parsedValue, 0d, 1d));
                }
            }

            var averageValue = values.Count > 0
                ? values.Average()
                : 0d;

            var percentage = Math.Round(averageValue * 100d);

            return ActionRefreshResult.ValueOnly(
                percentage,
                $"{percentage:N0}%");
        }

        public override async Task<ModuleResult> ExecuteAsync(
            double input,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var percentage = Math.Clamp(input, 0d, 100d);
            var configValue = percentage / 100d;

            var formattedValue = configValue.ToString(
                "0.000000",
                CultureInfo.InvariantCulture);

            var changes = _settingNames
                .Select(settingName => new BlackOps7SettingChange
                {
                    SettingName = settingName,
                    Value = formattedValue
                })
                .ToList();

            var result = await BlackOps7ConfigService.ApplyChangesAsync(
                _displayName,
                changes,
                token);

            return result.Success
                ? ModuleResult.Successful($"{_displayName} set to {percentage:N0}%")
                : ModuleResult.Failed(result.Message);
        }
    }
}