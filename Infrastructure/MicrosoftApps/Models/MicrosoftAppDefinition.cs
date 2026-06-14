using MaterialDesignThemes.Wpf;

namespace GameBoost.Infrastructure.MicrosoftApps.Models
{
    public sealed class MicrosoftAppDefinition
    {
        public required string DisplayName { get; init; }

        public required string PackageName { get; init; }

        public PackIconKind Icon { get; init; } = PackIconKind.PackageVariant;

        public bool IsCritical { get; init; }

        public bool AllowUninstall { get; init; } = true;

        public string Description { get; init; } = string.Empty;
    }
}
