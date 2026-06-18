using System.Diagnostics;
using System.Management;

namespace GameBoost.Infrastructure.MSI
{
    public static class PNPDeviceIDs
    {
        public static IReadOnlyList<MsiModeDeviceInfo> Win32_USBController =>
            GetDevices(
                wmiClassName: "Win32_USBController",
                category: MsiModeDeviceCategory.UsbController,
                includeDevice: device =>
                    StartsWithAny(device.PnpDeviceId, "PCI\\"));

        public static IReadOnlyList<MsiModeDeviceInfo> Win32_VideoController =>
            GetVideoControllers();

        public static IReadOnlyList<MsiModeDeviceInfo> Win32_NetworkAdapter =>
            GetDevices(
                wmiClassName: "Win32_NetworkAdapter",
                category: MsiModeDeviceCategory.NetworkAdapter,
                includeDevice: IsPhysicalNetworkAdapter);

        public static IReadOnlyList<MsiModeDeviceInfo> GetAllMsiModeTargets()
        {
            return
            [
                .. Win32_USBController,
                .. Win32_VideoController,
                .. Win32_NetworkAdapter
            ];
        }

        private static IReadOnlyList<MsiModeDeviceInfo> GetVideoControllers()
        {
            var devices = new List<MsiModeDeviceInfo>();

            devices.AddRange(
                GetPnpDevicesByClass(
                    pnpClass: "Display",
                    category: MsiModeDeviceCategory.VideoController,
                    includeDevice: IsPhysicalVideoController));

            return [.. devices
                .Where(device => !string.IsNullOrWhiteSpace(device.PnpDeviceId))
                .GroupBy(device => device.PnpDeviceId, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(device => device.Name)];
        }

        private static IReadOnlyList<MsiModeDeviceInfo> GetDevices(string wmiClassName, MsiModeDeviceCategory category, Func<MsiModeDeviceInfo, bool> includeDevice)
        {
            var devices = new List<MsiModeDeviceInfo>();

            try
            {
                using var searcher = new ManagementObjectSearcher(
                    $"SELECT Name, PNPDeviceID, Manufacturer FROM {wmiClassName}");

                foreach (ManagementObject result in searcher.Get())
                {
                    var name = result["Name"]?.ToString();
                    var pnpDeviceId = result["PNPDeviceID"]?.ToString();

                    if (string.IsNullOrWhiteSpace(name) ||
                        string.IsNullOrWhiteSpace(pnpDeviceId))
                    {
                        continue;
                    }

                    var device = new MsiModeDeviceInfo
                    {
                        Name = name,
                        PnpDeviceId = pnpDeviceId,
                        Manufacturer = result["Manufacturer"]?.ToString() ?? string.Empty,
                        Category = category
                    };

                    if (!includeDevice(device))
                        continue;

                    devices.Add(device);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Failed to query {wmiClassName}: {ex.Message}");
#endif
            }

            return devices
                .GroupBy(device => device.PnpDeviceId, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderBy(device => device.Category)
                .ThenBy(device => device.Name)
                .ToList();
        }

        private static IReadOnlyList<MsiModeDeviceInfo> GetPnpDevicesByClass(
            string pnpClass,
            MsiModeDeviceCategory category,
            Func<MsiModeDeviceInfo, bool> includeDevice)
        {
            var devices = new List<MsiModeDeviceInfo>();

            try
            {
                using var searcher = new ManagementObjectSearcher(
                    "SELECT Name, PNPDeviceID, Manufacturer, PNPClass, Status " +
                    "FROM Win32_PnPEntity " +
                    $"WHERE PNPClass = '{pnpClass}'");

                foreach (ManagementObject result in searcher.Get())
                {
                    var name = result["Name"]?.ToString();
                    var pnpDeviceId = result["PNPDeviceID"]?.ToString();

                    if (string.IsNullOrWhiteSpace(name) ||
                        string.IsNullOrWhiteSpace(pnpDeviceId))
                    {
                        continue;
                    }

                    var device = new MsiModeDeviceInfo
                    {
                        Name = name,
                        PnpDeviceId = pnpDeviceId,
                        Manufacturer = result["Manufacturer"]?.ToString() ?? string.Empty,
                        Category = category
                    };

                    if (!includeDevice(device))
                        continue;

                    devices.Add(device);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Failed to query Win32_PnPEntity {pnpClass}: {ex.Message}");
#endif
            }

            return devices;
        }

        private static bool IsPhysicalVideoController(
            MsiModeDeviceInfo device)
        {
            if (string.IsNullOrWhiteSpace(device.PnpDeviceId))
                return false;

            if (!device.PnpDeviceId.StartsWith("PCI\\", StringComparison.OrdinalIgnoreCase))
                return false;

            if (ContainsAny(
                    device.Name,
                    "Microsoft Basic",
                    "Remote Display",
                    "Indirect Display",
                    "Virtual",
                    "Parsec",
                    "Steam Streaming"))
            {
                return false;
            }

            return true;
        }

        private static bool IsPhysicalNetworkAdapter(
            MsiModeDeviceInfo device)
        {
            if (!StartsWithAny(device.PnpDeviceId, "PCI\\", "USB\\"))
                return false;

            if (ContainsAny(
                    device.Name,
                    "WAN Miniport",
                    "Virtual",
                    "Bluetooth",
                    "Loopback",
                    "TAP",
                    "VPN",
                    "Hyper-V",
                    "Npcap"))
            {
                return false;
            }

            return true;
        }

        private static bool StartsWithAny(
            string value,
            params string[] prefixes)
        {
            return prefixes.Any(prefix =>
                value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        private static bool ContainsAny(
            string value,
            params string[] values)
        {
            return values.Any(item =>
                value.Contains(item, StringComparison.OrdinalIgnoreCase));
        }
    }
}