using GameBoost.Infrastructure.GameConfigs.BlackOps7.Entries;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace GameBoost.Infrastructure.GameConfigs.BlackOps7
{
    public static class BlackOps7ConfigParser
    {
        private static readonly Regex SettingLineRegex = new(
            @"^(?<prefix>\s*)(?<key>[^=\s]+)\s*=\s*(?<value>.*?)(?<spacing>\s*)(?<comment>//.*)?$",
            RegexOptions.Compiled);

        public static BlackOps7ConfigDocument Parse(string path, string text)
        {
            var lines = text
                .Replace("\r\n", "\n")
                .Replace('\r', '\n')
                .Split('\n')
                .ToList();

            var entries = new List<BlackOps7ConfigEntry>();

            for (var index = 0; index < lines.Count; index++)
            {
                var line = lines[index];

                var match = SettingLineRegex.Match(line);

                if (!match.Success)
                    continue;

                //[==============Key==============]   [Value]  [=======Comment=======]
                //LicensedMusicVolume@0;14317;21371 = 1.000000 // 0.000000 to 1.000000
                var fullKey = match.Groups["key"].Value.Trim();
                var value = match.Groups["value"].Value.Trim();
                var comment = match.Groups["comment"].Success ? match.Groups["comment"].Value : null;

                //Debug.WriteLine($"Parsed line {index}: Key='{fullKey}', Value='{value}', Comment='{comment}'");

                if (string.IsNullOrWhiteSpace(fullKey))
                    continue;

                entries.Add(new BlackOps7ConfigEntry
                {
                    LineIndex = index,
                    FullKey = fullKey,
                    SettingName = GetSettingName(fullKey),
                    Value = value,
                    Comment = comment
                });
            }

            return new BlackOps7ConfigDocument
            {
                Path = path,
                Lines = lines,
                Entries = entries
            };
        }

        public static bool TrySetValue(BlackOps7ConfigDocument document, string settingName, string newValue)
        {
            var matches = document.Entries
                .Where(entry => string.Equals(
                    entry.SettingName,
                    settingName,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (matches.Count != 1)
                return false;

            var entry = matches[0];
            var line = document.Lines[entry.LineIndex];

            var match = SettingLineRegex.Match(line);

            if (!match.Success)
                return false;

            var prefix = match.Groups["prefix"].Value;
            var key = match.Groups["key"].Value;
            var spacing = match.Groups["spacing"].Value;
            var comment = match.Groups["comment"].Success ? $" {match.Groups["comment"].Value.Trim()}" : string.Empty;

            //Debug.WriteLine($"Updating line {entry.LineIndex}: Key='{key}', OldValue='{entry.Value}', NewValue='{newValue}', Comment='{comment}'");

            document.Lines[entry.LineIndex] =
                $"{prefix}{key} = {newValue}{spacing}{comment}".TrimEnd();

            return true;
        }

        private static string GetSettingName(string fullKey)
        {
            var atIndex = fullKey.IndexOf('@');

            return atIndex <= 0
                ? fullKey
                : fullKey[..atIndex];
        }
    }
}