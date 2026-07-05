using GameBoost.Infrastructure.GameConfigs.BlackOps7.Settings;
using GameBoost.Shared.Results;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GameBoost.Infrastructure.GameConfigs.BlackOps7
{
    public sealed class BlackOps7ConfigService
    {
        private static readonly Encoding ConfigEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        public static async Task<ModuleResult> ApplyPresetAsync(BlackOps7PresetDefinition preset, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var configPaths = BlackOps7ConfigFileLocator.GetBlackOps7ConfigFileLocator();

            if (configPaths.Count <= 0)
                return ModuleResult.Failed("No Black Ops 7 config files were found");

            var changedFiles = 0;
            var changedSettings = 0;
            var skippedSettings = new List<string>();

            foreach (var configPath in configPaths)
            {
                token.ThrowIfCancellationRequested();

                if (!File.Exists(configPath))
                    continue;

                var originalAttributes = File.GetAttributes(configPath);
                var wasReadOnly = originalAttributes.HasFlag(FileAttributes.ReadOnly);

                try
                {
                    if (wasReadOnly)
                        File.SetAttributes(configPath, originalAttributes & ~FileAttributes.ReadOnly);

                    var text = await File.ReadAllTextAsync(
                        configPath,
                        ConfigEncoding,
                        token);

                    var document = BlackOps7ConfigParser.Parse(configPath, text);

                    var fileChanged = false;

                    foreach (var change in preset.Changes)
                    {
                        token.ThrowIfCancellationRequested();

                        var changed = BlackOps7ConfigParser.TrySetValue(
                            document,
                            change.SettingName,
                            change.Value);

                        if (!changed)
                        {
                            skippedSettings.Add(change.SettingName);
                            continue;
                        }

                        changedSettings++;
                        fileChanged = true;
                    }

                    if (!fileChanged)
                        continue;

                    var output = string.Join("\n", document.Lines);

                    //Debug.WriteLine(output);

                    await File.WriteAllTextAsync(
                        configPath,
                        output,
                        ConfigEncoding,
                        token);

                    changedFiles++;
                }
                finally
                {
                    if (File.Exists(configPath) && wasReadOnly)
                        File.SetAttributes(configPath, originalAttributes);
                }
            }

            if (changedFiles <= 0)
            {
                return ModuleResult.Failed("No config files were changed");
            }

            return ModuleResult.Successful($"Successfully updated {changedSettings} settings");
        }
        public static async Task<ModuleResult> ApplyChangesAsync(string actionName, IReadOnlyList<BlackOps7SettingChange> changes, CancellationToken token)
        {
            var definition = new BlackOps7PresetDefinition
            {
                Id = actionName
                    .Replace(" ", "_", StringComparison.OrdinalIgnoreCase)
                    .ToLowerInvariant(),

                DisplayName = actionName,
                Description = $"Applies {actionName} settings",
                Changes = changes
            };

            return await ApplyPresetAsync(
                definition,
                token);
        }

        public static async Task<string?> GetSettingValueAsync(string settingName, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var configPaths = BlackOps7ConfigFileLocator.GetBlackOps7ConfigFileLocator();

            foreach (var configPath in configPaths)
            {
                if (!File.Exists(configPath))
                    continue;

                var text = await File.ReadAllTextAsync(
                    configPath,
                    token);

                var document = BlackOps7ConfigParser.Parse(
                    configPath,
                    text);

                var entry = document.Entries.FirstOrDefault(entry =>
                    string.Equals(
                        entry.SettingName,
                        settingName,
                        StringComparison.OrdinalIgnoreCase));

                if (entry is not null)
                    return entry.Value;
            }

            return null;
        }

    }
}