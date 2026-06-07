using GameBoost.SystemInformation.Components;
using System.Management;
using GameBoost.Shared.Helpers;
using System.Diagnostics;

namespace GameBoost.SystemInformation.Providers
{
    public static class MotherboardInfoProvider
    {
        public static MotherboardInfo FetchMotherboardInformation()
        {
            var mobo = new MotherboardInfo();

            try
            {
                using var boardSearcher = new ManagementObjectSearcher(
                    "SELECT Manufacturer, Product, SerialNumber, Version, SKU, Tag FROM Win32_BaseBoard");

                foreach (ManagementObject obj in boardSearcher.Get())
                {
                    mobo.Manufacturer = CleanWmiValue(obj["Manufacturer"]);
                    mobo.Product = CleanWmiValue(obj["Product"]);
                    mobo.SerialNumber = CleanWmiValue(obj["SerialNumber"]);
                    mobo.Version = CleanWmiValue(obj["Version"]);
                    mobo.SKU = CleanWmiValue(obj["SKU"]);

                    // Win32_BaseBoard usually has Tag, not AssetTag.
                    mobo.AssetTag = CleanWmiValue(obj["Tag"]);

                    break;
                }

                mobo.Brand = DetectBrand(mobo.Manufacturer);

                using var biosSearcher = new ManagementObjectSearcher(
                    "SELECT SMBIOSBIOSVersion, Manufacturer, ReleaseDate FROM Win32_BIOS");

                foreach (ManagementObject obj in biosSearcher.Get())
                {
                    mobo.BIOSVersion = CleanWmiValue(obj["SMBIOSBIOSVersion"]);
                    mobo.BIOSManufacturer = CleanWmiValue(obj["Manufacturer"]);
                    mobo.BIOSReleaseDate = WmiDateHelper.FormatDate(
                        obj["ReleaseDate"]?.ToString(),
                        "dd/MM/yyyy");

                    break;
                }
            }
            catch (ManagementException ex)
            {
#if DEBUG
                Debug.WriteLine($"WMI motherboard query failed: {ex.Message}");
                Debug.WriteLine($"WMI error code: {ex.ErrorCode}");
#endif

                SetDefaultValues(mobo, $"WMI error retrieving motherboard data: {ex.Message}");
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error retrieving motherboard data: {ex.Message}");
#endif

                SetDefaultValues(mobo, $"Error retrieving motherboard data: {ex.Message}");
            }

            return mobo;
        }

        private static string CleanWmiValue(object? value)
        {
            var text = value?.ToString()?.Trim();

            if (string.IsNullOrWhiteSpace(text))
                return "Unknown";

            return text;
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
