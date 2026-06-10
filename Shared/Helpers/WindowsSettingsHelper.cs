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