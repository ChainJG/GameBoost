using GameBoost.Infrastructure.Shell;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace GameBoost.Infrastructure.Power
{
    public static class PowerCfgStatusHelper
    {
        private const string UserPowerSchemesPath =
            @"SYSTEM\CurrentControlSet\Control\Power\User\PowerSchemes";

        public static async Task<PowerCfgSettingStatus> GetStatusAsync(
            PowerCfgSettingDefinition definition,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var registryStatus = TryGetStatusFromRegistry(definition);

            if (registryStatus.Status != ToggleType.Unknown)
                return registryStatus;

            token.ThrowIfCancellationRequested();

            return await GetStatusFromCommandPromptAsync(
                definition,
                token);
        }

        private static PowerCfgSettingStatus TryGetStatusFromRegistry(
            PowerCfgSettingDefinition definition)
        {
            try
            {
                var activeSchemeGuid = TryReadActiveSchemeGuidFromRegistry();

                if (string.IsNullOrWhiteSpace(activeSchemeGuid))
                    return Unknown("Active power scheme was not found in registry");

                var settingPath =
                    $@"{UserPowerSchemesPath}\{activeSchemeGuid}\{NormalizeGuid(definition.SubGroupGuid)}\{NormalizeGuid(definition.SettingGuid)}";

                using var baseKey = RegistryKey.OpenBaseKey(
                    RegistryHive.LocalMachine,
                    RegistryView.Registry64);

                using var settingKey = baseKey.OpenSubKey(settingPath);

                if (settingKey is null)
                    return Unknown($"Power setting registry path was not found: {settingPath}");

                var acValue = TryReadInt(
                    settingKey.GetValue("ACSettingIndex"));

                var dcValue = TryReadInt(
                    settingKey.GetValue("DCSettingIndex"));

                if (acValue is null)
                    return Unknown("ACSettingIndex was not found in registry");

                var status = GetRecommendedStatus(
                    definition,
                    acValue,
                    dcValue);

                return new PowerCfgSettingStatus
                {
                    Status = status,
                    CurrentAcValue = acValue,
                    CurrentDcValue = dcValue,
                    RecommendedAcValue = definition.RecommendedAcValue,
                    RecommendedDcValue = definition.RecommendedDcValue,
                    Message = $"Registry AC={acValue}, DC={dcValue}"
                };
            }
            catch
            {
                return Unknown("Failed to read power setting from registry");
            }
        }

        private static async Task<PowerCfgSettingStatus> GetStatusFromCommandPromptAsync(
            PowerCfgSettingDefinition definition,
            CancellationToken token)
        {
            var command =
                $"powercfg /query SCHEME_CURRENT {definition.SubGroupAlias} {definition.SettingAlias}";

            var result = await ShellService.RunAsync(
                ShellType.Cmd,
                command,
                token);

            if (!result.Success)
            {
                var message = !string.IsNullOrWhiteSpace(result.Error)
                    ? result.Error
                    : result.Output;

                return Unknown(message);
            }

            var acValue = TryParsePowerCfgValue(
                result.Output,
                "Current AC Power Setting Index");

            var dcValue = TryParsePowerCfgValue(
                result.Output,
                "Current DC Power Setting Index");

            if (acValue is null)
                return Unknown("Could not parse AC value from powercfg output");

            var status = GetRecommendedStatus(
                definition,
                acValue,
                dcValue);

            return new PowerCfgSettingStatus
            {
                Status = status,
                CurrentAcValue = acValue,
                CurrentDcValue = dcValue,
                RecommendedAcValue = definition.RecommendedAcValue,
                RecommendedDcValue = definition.RecommendedDcValue,
                Message = $"CommandPrompt AC={acValue}, DC={dcValue}"
            };
        }

        private static string? TryReadActiveSchemeGuidFromRegistry()
        {
            using var baseKey = RegistryKey.OpenBaseKey(
                RegistryHive.LocalMachine,
                RegistryView.Registry64);

            using var schemesKey = baseKey.OpenSubKey(
                UserPowerSchemesPath);

            var value = schemesKey?
                .GetValue("ActivePowerScheme")?
                .ToString();

            if (string.IsNullOrWhiteSpace(value))
                return null;

            return NormalizeGuid(value);
        }

        private static ToggleType GetRecommendedStatus(
            PowerCfgSettingDefinition definition,
            int? acValue,
            int? dcValue)
        {
            if (acValue is null)
                return ToggleType.Unknown;

            if (acValue.Value != definition.RecommendedAcValue)
                return ToggleType.Disabled;

            if (!definition.CheckDcValue)
                return ToggleType.Enabled;

            if (definition.RecommendedDcValue is null)
                return ToggleType.Unknown;

            if (dcValue is null)
                return ToggleType.Unknown;

            return dcValue.Value == definition.RecommendedDcValue.Value
                ? ToggleType.Enabled
                : ToggleType.Disabled;
        }

        private static int? TryParsePowerCfgValue(
            string output,
            string label)
        {
            var pattern =
                $@"{Regex.Escape(label)}:\s*(0x[0-9a-fA-F]+|\d+)";

            var match = Regex.Match(
                output,
                pattern,
                RegexOptions.IgnoreCase);

            if (!match.Success)
                return null;

            var valueText = match.Groups[1].Value.Trim();

            if (valueText.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                return Convert.ToInt32(valueText, 16);

            return int.TryParse(valueText, out var value)
                ? value
                : null;
        }

        private static int? TryReadInt(object? value)
        {
            return value switch
            {
                int intValue => intValue,

                long longValue => unchecked((int)longValue),

                string stringValue when stringValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                    => Convert.ToInt32(stringValue, 16),

                string stringValue when int.TryParse(stringValue, out var parsed)
                    => parsed,

                _ => null
            };
        }

        private static string NormalizeGuid(string guid)
        {
            var cleanGuid = guid
                .Trim()
                .Trim('{', '}');

            return $"{{{cleanGuid}}}";
        }

        private static PowerCfgSettingStatus Unknown(string message)
        {
            return new PowerCfgSettingStatus
            {
                Status = ToggleType.Unknown,
                Message = message
            };
        }
    }
}