using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace GameBoost.Shared.Helpers
{
    public sealed class InstalledProgramSnapshot(HashSet<string> programNames)
    {
        private static readonly bool IsDebug = false;

        private readonly HashSet<string> _programNames = programNames;

        public static InstalledProgramSnapshot Create()
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            ReadUninstallNames(
                Registry.CurrentUser,
                @"Software\Microsoft\Windows\CurrentVersion\Uninstall",
                names);

            ReadUninstallNames(
                Registry.LocalMachine,
                @"Software\Microsoft\Windows\CurrentVersion\Uninstall",
                names);

            ReadUninstallNames(
                Registry.LocalMachine,
                @"Software\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
                names);

            return new InstalledProgramSnapshot(names);
        }

        public bool TryFindMatch(string value, out string matchedProgramName)
        {
            matchedProgramName = string.Empty;

            if (string.IsNullOrWhiteSpace(value))
                return false;

            var normalizedValue = Normalize(value);

            foreach (var programName in _programNames)
            {
                if (!IsProgramNameMatch(programName, normalizedValue))
                    continue;

                matchedProgramName = programName;
                return true;
            }

            return false;
        }

        public bool Contains(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var normalizedValue = Normalize(value);

            return _programNames.Any(programName =>
                IsProgramNameMatch(programName, normalizedValue));
        }

        private static bool IsProgramNameMatch(
            string installedProgramName,
            string searchValue)
        {
            var normalizedProgramName = Normalize(installedProgramName);

            if (string.Equals(
                    normalizedProgramName,
                    searchValue,
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return ContainsWholeWords(
                normalizedProgramName,
                searchValue);
        }

        private static bool ContainsWholeWords(
            string installedProgramName,
            string searchValue)
        {
            var pattern = $@"(^|[^a-z0-9]){Regex.Escape(searchValue)}($|[^a-z0-9])";

            return Regex.IsMatch(
                installedProgramName,
                pattern,
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        private static string Normalize(string value)
        {
            return value
                .Trim()
                .ToLowerInvariant()
                .Replace("_", " ")
                .Replace("-", " ")
                .Replace(".", " ");
        }

        private static void ReadUninstallNames(
            RegistryKey hive,
            string path,
            HashSet<string> names)
        {
            try
            {
                using var key = hive.OpenSubKey(path);

                if (key is null)
                    return;

                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    using var appKey = key.OpenSubKey(subKeyName);

                    var displayName = appKey?.GetValue("DisplayName") as string;

                    if (!string.IsNullOrWhiteSpace(displayName))
                        names.Add(displayName.Trim());
                }
            }
            catch
            {
                // Ignore inaccessible uninstall entries.
            }
        }
    }
}