namespace GameBoost.Infrastructure.MSI
{
    public sealed class MsiModeDeviceInfo
    {
        public required string Name { get; init; }

        public required string PnpDeviceId { get; init; }

        public required MsiModeDeviceCategory Category { get; init; }

        public string Manufacturer { get; init; } = string.Empty;

        public string DisplayName =>
            string.IsNullOrWhiteSpace(Manufacturer)
                ? Name
                : $"{Name} ({Manufacturer})";
    }

    public enum MsiModeDeviceCategory
    {
        UsbController,
        VideoController,
        NetworkAdapter
    }
}
