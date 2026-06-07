using GameBoost.Shared.Helpers;
using GameBoost.SystemInformation.Components;
using Microsoft.Win32;
using System.Management;

namespace GameBoost.SystemInformation.Providers
{
    public static class GpuInfoProvider
    {
        const string displayClassPath = @"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}";

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

                    var registryVramBytes = TryGetDedicatedVramFromRegistry(gpu.PNPDeviceID);
                    gpu.AdapterRAM = MathHelper.FormatBytes(registryVramBytes);

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

        private static ulong? TryGetDedicatedVramFromRegistry(string pnpDeviceId)
        {
            if (string.IsNullOrWhiteSpace(pnpDeviceId) ||
                pnpDeviceId.Equals("Unknown", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            const string displayClassPath =
                @"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}";

            using var baseKey = Registry.LocalMachine.OpenSubKey(displayClassPath);

            if (baseKey is null)
                return null;

            var normalizedPnpId = NormalizeDeviceId(pnpDeviceId);

            foreach (var subKeyName in baseKey.GetSubKeyNames())
            {
                using var adapterKey = baseKey.OpenSubKey(subKeyName);

                if (adapterKey is null)
                    continue;

                var matchingDeviceId = adapterKey.GetValue("MatchingDeviceId")?.ToString();

                if (string.IsNullOrWhiteSpace(matchingDeviceId))
                    continue;

                if (!normalizedPnpId.Contains(NormalizeDeviceId(matchingDeviceId)))
                    continue;

                var memoryValue = adapterKey.GetValue("HardwareInformation.qwMemorySize");

                if (memoryValue is long longValue && longValue > 0)
                    return (ulong)longValue;

                if (memoryValue is ulong ulongValue && ulongValue > 0)
                    return ulongValue;
            }

            return null;
        }

        private static string NormalizeDeviceId(string value)
        {
            return value
                .Replace("\\", string.Empty)
                .Replace("&", string.Empty)
                .ToLowerInvariant();
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
