using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;
using System.Diagnostics;

namespace GameBoost.Shared.Helpers
{
    public static class WindowsSettingsHelper
    {
        public static bool TryOpenRealTimeProtectionSettings()
        {
            return TryOpenUri(
                "windowsdefender://threatsettings",
                "windowsdefender:",
                "ms-settings:windowsdefender");
        }

        public static bool TryOpenMemoryIntegritySettings()
        {
            return TryOpenUri(
                "windowsdefender://coreisolation",
                "windowsdefender:",
                "ms-settings:windowsdefender");
        }

        public static bool TryOpenWindowsSecurity()
        {
            return TryOpenUri(
                "windowsdefender:",
                "ms-settings:windowsdefender");
        }

        public static bool TryOpenUri(params string[] uriCandidates)
        {
            foreach (var uri in uriCandidates)
            {
                if (string.IsNullOrWhiteSpace(uri))
                    continue;

                if (TryStartUri(uri))
                    return true;
            }

            return false;
        }

        public static ToggleType GetTamperProtectionStatus()
        {
            RegistryEditInfo TamperProtectionEdits = new()
            {
                Hive = RegistryHive.LocalMachine,
                Path = @"SOFTWARE\Microsoft\Windows Defender\Features",
                Key = "TamperProtection",
                Kind = RegistryValueKind.DWord,
                EnabledValue = 5,
                DisabledValue = 4
            };

            var result = RegistryHelper.GetValue(TamperProtectionEdits);

            if (!result.Success)
                return ToggleType.Unknown;

            return result.Value switch
            {
                int value when value == 5 => ToggleType.Enabled,
                int value when value == 4 => ToggleType.Disabled,

                string value when value == "5" => ToggleType.Enabled,
                string value when value == "4" => ToggleType.Disabled,

                _ => ToggleType.Unknown
            };
        }

        private static bool TryStartUri(string uri)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = uri,
                    UseShellExecute = true
                });

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}