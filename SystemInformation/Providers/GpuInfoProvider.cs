using GameBoost.SystemInformation.Components;
using System.Management;

namespace GameBoost.SystemInformation.Providers
{
    public static class GpuInfoProvider
    {
        public static GpuInfo FetchGpuInformation()
        {
            var gpu = new GpuInfo();

            try
            {
                // Query Win32_VideoController for graphics card data
                using var searcher = new ManagementObjectSearcher(
                    "SELECT Name, DriverVersion, VideoProcessor, AdapterRAM, " +
                    "VideoModeDescription, PNPDeviceID, CurrentRefreshRate, Status FROM Win32_VideoController");

                foreach (ManagementObject obj in searcher.Get())
                {
                    // Populate the values using the setters
                    gpu.Name = obj["Name"]?.ToString()?.Trim() ?? "Unknown GPU";
                    gpu.DriverVersion = obj["DriverVersion"]?.ToString()?.Trim() ?? "Unknown";
                    gpu.VideoProcessor = obj["VideoProcessor"]?.ToString()?.Trim() ?? "Unknown";
                    gpu.VideoModeDescription = obj["VideoModeDescription"]?.ToString()?.Trim() ?? "Unknown";
                    gpu.PNPDeviceID = obj["PNPDeviceID"]?.ToString()?.Trim() ?? "Unknown";
                    gpu.Status = obj["Status"]?.ToString()?.Trim() ?? "Unknown";

                    gpu.RefreshRate = Convert.ToInt32(obj["CurrentRefreshRate"] ?? 0);

                    // Format AdapterRAM (WMI returns this in raw bytes)
                    gpu.AdapterRAM = FormatVram(obj["AdapterRAM"]);

                    // Return after finding the primary display adapter
                    return gpu;
                }
            }
            catch (Exception ex)
            {
                // Fallback defaults if WMI fails
                SetDefaultValues(gpu, $"Error retrieving GPU data: {ex.Message}");
            }

            return gpu;
        }

        private static string FormatVram(object ramValue)
        {
            if (ramValue == null) return "Unknown";

            // WMI can return VRAM as an unsigned long or uint depending on the GPU size
            double bytes = Convert.ToDouble(ramValue);

            // Convert bytes to Gigabytes (Divide by 1024 x 1024 x 1024)
            double gigabytes = bytes / (1024 * 1024 * 1024);

            // Return a clean rounded string like "12 GB" or "8 GB"
            return $"{Math.Round(gigabytes, 1)} GB";
        }

        private static void SetDefaultValues(GpuInfo gpu, string errorMessage)
        {
            gpu.Name = "Unknown Video Controller";
            gpu.DriverVersion = "Unknown";
            gpu.VideoProcessor = "Unknown";
            gpu.AdapterRAM = "Unknown";
            gpu.VideoModeDescription = "Unknown";
            gpu.PNPDeviceID = "Unknown";
            gpu.RefreshRate = 0;
            gpu.Status = "Error";
        }
    }
}
