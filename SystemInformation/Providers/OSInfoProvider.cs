using GameBoost.Shared.Helpers;
using GameBoost.SystemInformation.Components;
using System.Management;

namespace GameBoost.SystemInformation.Providers
{
    public static class OSInfoProvider
    {
        public static OSInfo FetchOSInformation()
        {
            var os = new OSInfo();

            try
            {
                // 1. Query Win32_OperatingSystem for core OS details
                using (var searcher = new ManagementObjectSearcher(
                    "SELECT Caption, Version, BuildNumber, OSArchitecture, " +
                    "InstallDate, SystemDirectory, BootDevice FROM Win32_OperatingSystem"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        os.Name = obj["Caption"]?.ToString()?.Trim() ?? "Windows";
                        os.Version = obj["Version"]?.ToString()?.Trim() ?? "Unknown";
                        os.BuildNumber = obj["BuildNumber"]?.ToString()?.Trim() ?? "Unknown";
                        os.SystemDirectory = obj["SystemDirectory"]?.ToString()?.Trim() ?? @"C:\Windows\System32";
                        os.BootDevice = obj["BootDevice"]?.ToString()?.Trim() ?? "Unknown";

                        // Parse Architecture
                        string arch = obj["OSArchitecture"]?.ToString() ?? "Unknown";
                        os.Architecture = arch;
                        os.Is64Bit = arch.Contains("64");

                        // Format the ugly raw WMI InstallDate string
                        os.InstallDate = WmiDateHelper.FormatDate(obj["InstallDate"]?.ToString(), "dd-MM-yyyy");
                        break;
                    }
                }

                // 2. Calculate real-time Uptime using the system tick counter
                // Environment.TickCount64 gives milliseconds since the system started
                os.Uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
            }
            catch (Exception ex)
            {
                SetDefaultValues(os, $"Error retrieving OS data: {ex.Message}");
            }

            return os;
        }

        private static void SetDefaultValues(OSInfo os, string errorMessage)
        {
            os.Name = "Windows (Unknown)";
            os.Version = "Unknown";
            os.BuildNumber = "Unknown";
            os.Architecture = "Unknown";
            os.InstallDate = "Unknown";
            os.SystemDirectory = @"C:\Windows\System32";
            os.BootDevice = "Unknown";
            os.Is64Bit = Environment.Is64BitOperatingSystem;
            os.Uptime = TimeSpan.Zero;
        }
    }
}
