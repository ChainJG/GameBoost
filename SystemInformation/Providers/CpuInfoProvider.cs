using GameBoost.SystemInformation.Components;
using System.Management;

namespace GameBoost.SystemInformation.Providers
{
    public static class CpuInfoProvider
    {
        public static CpuInfo FetchCpuInformation()
        {
            var cpu = new CpuInfo();

            try
            {
                // Query the Win32_Processor class for all required data
                using var searcher = new ManagementObjectSearcher(
                    "SELECT Name, NumberOfCores, NumberOfLogicalProcessors, Manufacturer, " +
                    "Architecture, SocketDesignation, L2CacheSize, L3CacheSize, " +
                    "MaxClockSpeed, CurrentClockSpeed, ProcessorId FROM Win32_Processor");

                foreach (ManagementObject obj in searcher.Get())
                {
                    // Populating the values using the setters
                    cpu.Name = obj["Name"]?.ToString()?.Trim() ?? "Unknown";
                    cpu.CoreCount = Convert.ToInt32(obj["NumberOfCores"] ?? 0);
                    cpu.LogicalProcessors = Convert.ToInt32(obj["NumberOfLogicalProcessors"] ?? 0);
                    cpu.Manufacturer = obj["Manufacturer"]?.ToString()?.Trim() ?? "Unknown";
                    cpu.Socket = obj["SocketDesignation"]?.ToString()?.Trim() ?? "Unknown";
                    cpu.ProcessorId = obj["ProcessorId"]?.ToString()?.Trim() ?? "Unknown";

                    // Format Clock Speeds
                    cpu.MaxClockSpeed = $"{obj["MaxClockSpeed"]} MHz";
                    cpu.CurrentClockSpeed = $"{obj["CurrentClockSpeed"]} MHz";

                    // Format Cache Sizes (WMI provides this in KB)
                    cpu.L2Cache = FormatCache(obj["L2CacheSize"]);
                    cpu.L3Cache = FormatCache(obj["L3CacheSize"]);

                    // Convert raw architecture architecture ID to human readable string
                    cpu.Architecture = FormatArchitecture(obj["Architecture"]);

                    // Return after the first CPU package found
                    return cpu;
                }
            }
            catch (Exception ex)
            {
                // Fallback defaults if WMI fails
                SetDefaultValues(cpu, $"Error retrieving CPU data: {ex.Message}");
            }

            return cpu;
        }

        private static string FormatCache(object cacheValue)
        {
            if (cacheValue == null) return "Unknown";
            double sizeInKb = Convert.ToDouble(cacheValue);

            // Convert to MB if it's large enough
            return sizeInKb >= 1024
                ? $"{Math.Round(sizeInKb / 1024, 1)} MB"
                : $"{sizeInKb} KB";
        }

        private static string FormatArchitecture(object archValue)
        {
            if (archValue == null) return "Unknown";

            return Convert.ToInt32(archValue) switch
            {
                0 => "x86",
                5 => "ARM",
                9 => "x64",
                12 => "ARM64",
                _ => "Unknown"
            };
        }

        private static void SetDefaultValues(CpuInfo cpu, string errorMessage)
        {
            cpu.Name = "Unknown Processor";
            cpu.Manufacturer = "Unknown";
            cpu.Architecture = "Unknown";
            cpu.Socket = "Unknown";
            cpu.L2Cache = "Unknown";
            cpu.L3Cache = "Unknown";
            cpu.MaxClockSpeed = "0 MHz";
            cpu.CurrentClockSpeed = "0 MHz";
            cpu.ProcessorId = "Unknown";
        }
    }
}
