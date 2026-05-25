namespace GameBoost.SystemInformation.Components
{
    public class MotherboardInfo
    {
        public string Manufacturer { get; set; }
        public string Product { get; set; }        // Z790, B550 etc
        public string SerialNumber { get; set; }   // VERY important
        public MotherboardBrand Brand { get; set; }
        public string Version { get; set; }
        public string AssetTag { get; set; }
        public string SKU { get; set; }
        public string BIOSVersion { get; set; }
        public string BIOSManufacturer { get; set; }
        public string BIOSReleaseDate { get; set; }
    }

    public enum MotherboardBrand
    {
        MSI,
        ASUS,
        Gigabyte,
        ASRock,
        Dell,
        HP,
        Lenovo,
        Unknown
    }
}
