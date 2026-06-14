using MaterialDesignThemes.Wpf;

namespace GameBoost.Infrastructure.MicrosoftApps.Models
{
    public sealed class InstalledAppxPackageInfo
    {
        public required string Name { get; init; }

        public required string PackageFullName { get; init; }

        public string DisplayName { get; init; } = string.Empty;

        public string InstallLocation { get; init; } = string.Empty;

        public string Publisher { get; init; } = string.Empty;

        public bool IsFramework { get; init; }

        public bool IsResourcePackage { get; init; }

        public PackIconKind Icon { get; init; } = PackIconKind.PackageVariant;

        public string FriendlyName =>
            string.IsNullOrWhiteSpace(DisplayName)
                ? Name
                : DisplayName;
    }
}
