using GameBoost.SystemInformation.Components;
using System.Management;

namespace GameBoost.SystemInformation.Providers
{
    public static class MemoryInfoProvider
    {
        public static MemoryInfo FetchMemoryInformation()
        {
            var memory = new MemoryInfo
            {
                Sticks = new List<RamStickInfo>()
            };

            try
            {
                // Get Global Capacity Metrics from Win32_ComputerSystem & Win32_OperatingSystem
                using (var osSearcher = new ManagementObjectSearcher(
                    "SELECT TotalVisibleMemorySize, FreePhysicalMemory, TotalVirtualMemorySize, FreeVirtualMemory FROM Win32_OperatingSystem"))
                {
                    foreach (ManagementObject obj in osSearcher.Get())
                    {
                        // OS reports sizes in Kilobytes; convert to Bytes to match 'ulong' representation
                        memory.TotalPhysicalMemory = Convert.ToUInt64(obj["TotalVisibleMemorySize"]) * 1024;
                        memory.AvailablePhysicalMemory = Convert.ToUInt64(obj["FreePhysicalMemory"]) * 1024;
                        memory.TotalVirtualMemory = Convert.ToUInt64(obj["TotalVirtualMemorySize"]) * 1024;
                        memory.AvailableVirtualMemory = Convert.ToUInt64(obj["FreeVirtualMemory"]) * 1024;
                        break;
                    }
                }

                // Query individual memory modules for hardware fingerprints
                uint rawMemoryType = 0;
                int maxSpeedFound = 0;

                using (var ramSearcher = new ManagementObjectSearcher(
                    "SELECT Manufacturer, Capacity, Speed, SerialNumber, PartNumber, MemoryType, SMBIOSMemoryType FROM Win32_PhysicalMemory"))
                {
                    foreach (ManagementObject obj in ramSearcher.Get())
                    {
                        var stick = new RamStickInfo
                        {
                            Manufacturer = obj["Manufacturer"]?.ToString()?.Trim() ?? "Unknown",
                            SerialNumber = obj["SerialNumber"]?.ToString()?.Trim() ?? "Unknown",
                            PartNumber = obj["PartNumber"]?.ToString()?.Trim() ?? "Unknown"
                        };

                        // Extract Memory Types for generation checking (DDR4 vs DDR5)
                        if (obj["SMBIOSMemoryType"] != null) rawMemoryType = Convert.ToUInt32(obj["SMBIOSMemoryType"]);
                        else if (obj["MemoryType"] != null) rawMemoryType = Convert.ToUInt32(obj["MemoryType"]);

                        // Speed handling
                        int stickSpeed = Convert.ToInt32(obj["Speed"] ?? 0);
                        stick.Speed = $"{stickSpeed} MHz";
                        if (stickSpeed > maxSpeedFound) maxSpeedFound = stickSpeed;

                        // Stick capacity formatting
                        ulong bytes = Convert.ToUInt64(obj["Capacity"] ?? 0);
                        stick.Capacity = $"{bytes / (1024 * 1024 * 1024)} GB";

                        memory.Sticks.Add(stick);
                    }
                }

                // Complete structural parameters
                memory.SlotsUsed = memory.Sticks.Count;
                memory.SpeedMHz = maxSpeedFound;
                memory.MemoryType = TranslateMemoryType(rawMemoryType, maxSpeedFound);

                // Get Total Available Board Slots via structural mapping
                memory.TotalSlots = FetchTotalMotherboardSlots() ?? Math.Max(4, memory.SlotsUsed);
            }
            catch (Exception ex)
            {
                // Assign minimal recovery settings
                memory.MemoryType = "Unknown";
                if (memory.Sticks.Count == 0) memory.Sticks.Add(new RamStickInfo { Manufacturer = $"Error: {ex.Message}" });
            }

            return memory;
        }

        // Mapping helper to resolve the exact DDR generation version
        private static string TranslateMemoryType(uint rawType, int speed)
        {
            // Standard SMBIOS structural values
            switch (rawType)
            {
                case 24: return "DDR3";
                case 26: return "DDR4";
                case 34: return "DDR5";
            }

            // Fallback strategy: Calculate Generation based on common clock speeds if SMBIOS table is generic (0)
            if (speed >= 4800) return "DDR5";
            if (speed >= 2133) return "DDR4";
            if (speed >= 800) return "DDR3";

            return "DDR";
        }

        // Queries the Motherboard physical layout definitions to find out how many slots exist
        private static int? FetchTotalMotherboardSlots()
        {
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT MemoryDevices FROM Win32_PhysicalMemoryArray");
                foreach (ManagementObject obj in searcher.Get())
                {
                    return Convert.ToInt32(obj["MemoryDevices"]);
                }
            }
            catch { }
            return null;
        }
    }
}

