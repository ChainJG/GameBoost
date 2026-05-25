using GameBoost.SystemInformation.Components;
using System.Management;
using GameBoost.Shared.Helpers;

namespace GameBoost.SystemInformation.Providers
{
    public static class MotherboardInfoProvider
    {
        public static MotherboardInfo FetchMotherboardInformation()
        {
            var mobo = new MotherboardInfo();

            try
            {
                // Query Win32_BaseBoard for physical hardware details
                using (var boardSearcher = new ManagementObjectSearcher(
                    "SELECT Manufacturer, Product, SerialNumber, Version, AssetTag, SKU FROM Win32_BaseBoard"))
                {
                    foreach (ManagementObject obj in boardSearcher.Get())
                    {
                        mobo.Manufacturer = obj["Manufacturer"]?.ToString()?.Trim() ?? "Unknown";
                        mobo.Product = obj["Product"]?.ToString()?.Trim() ?? "Unknown";
                        mobo.SerialNumber = obj["SerialNumber"]?.ToString()?.Trim() ?? "Unknown";
                        mobo.Version = obj["Version"]?.ToString()?.Trim() ?? "Unknown";
                        mobo.AssetTag = obj["AssetTag"]?.ToString()?.Trim() ?? "Unknown";
                        mobo.SKU = obj["SKU"]?.ToString()?.Trim() ?? "Unknown";
                        break; 
                    }
                }

                // Map the text manufacturer to your MotherboardBrand Enum
                mobo.Brand = DetectBrand(mobo.Manufacturer);

                // Query Win32_BIOS for firmware details
                using (var biosSearcher = new ManagementObjectSearcher(
                    "SELECT SMBIOSBIOSVersion, Manufacturer, ReleaseDate FROM Win32_BIOS"))
                {
                    foreach (ManagementObject obj in biosSearcher.Get())
                    {
                        mobo.BIOSVersion = obj["SMBIOSBIOSVersion"]?.ToString()?.Trim() ?? "Unknown";
                        mobo.BIOSManufacturer = obj["Manufacturer"]?.ToString()?.Trim() ?? "Unknown";

                        // Format the raw WMI timestamp into a readable date string
                        mobo.BIOSReleaseDate = WmiDateHelper.FormatDate(obj["ReleaseDate"]?.ToString(), "dd/MM/yyyy");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                SetDefaultValues(mobo, $"Error retrieving motherboard data: {ex.Message}");
            }

            return mobo;
        }

        public static MotherboardBrand DetectBrand(string? manufacturer)
        {
            if (string.IsNullOrWhiteSpace(manufacturer)) return MotherboardBrand.Unknown;

            string upperMfg = manufacturer.ToUpperInvariant();

            if (upperMfg.Contains("MSI") || upperMfg.Contains("MICRO-STAR")) return MotherboardBrand.MSI;
            if (upperMfg.Contains("ASUS") || upperMfg.Contains("ASUSTEK")) return MotherboardBrand.ASUS;
            if (upperMfg.Contains("GIGABYTE")) return MotherboardBrand.Gigabyte;
            if (upperMfg.Contains("ASROCK")) return MotherboardBrand.ASRock;
            if (upperMfg.Contains("DELL")) return MotherboardBrand.Dell;
            if (upperMfg.Contains("HP") || upperMfg.Contains("HEWLETT-PACKARD")) return MotherboardBrand.HP;
            if (upperMfg.Contains("LENOVO")) return MotherboardBrand.Lenovo;

            return MotherboardBrand.Unknown;
        }
        private static void SetDefaultValues(MotherboardInfo mobo, string errorMessage)
        {
            mobo.Manufacturer = "Unknown";
            mobo.Product = "Unknown";
            mobo.SerialNumber = "Unknown";
            mobo.Brand = MotherboardBrand.Unknown;
            mobo.Version = "Unknown";
            mobo.AssetTag = "Unknown";
            mobo.SKU = "Unknown";
            mobo.BIOSVersion = "Unknown";
            mobo.BIOSManufacturer = "Unknown";
            mobo.BIOSReleaseDate = "Unknown";
        }
    }
}
