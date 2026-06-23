using MaterialDesignThemes.Wpf;

namespace GameBoost.Infrastructure.Installers.Models
{
    public sealed class AppInstallDefinition
    {
        public required string Id { get; init; }

        public required string DisplayName { get; init; }

        public required string Description { get; init; }

        public PackIconKind Icon { get; init; }

        public required AppInstallCategory Category { get; init; }

        public required AppInstallProvider Provider { get; init; }

        public string? WingetId { get; init; }

        public string? WingetSource { get; init; } = "winget";

        public string? OfficialWebsite { get; init; }

        public bool RequiresAdmin { get; init; }

        public bool RequiresRestart { get; init; }

        public bool SupportsSilentInstall { get; init; } = true;

        public string[] ProcessNames { get; init; } = [];

        public string[] InstalledProgramNames { get; init; } = [];

        public string[] Tags { get; init; } = [];

        public int SortOrder { get; init; } = 100;

        public bool IsSelected { get; set; }

        public bool IsInstalled { get; set; }

        public bool IsInstalling { get; set; }

        public bool InstallFailed { get; set; }
    }

    public enum AppInstallState
    {
        Available,
        Installed,
        Installing,
        Cancelled,
        Failed,
        RequiresAdmin
    }

    public enum AppInstallCategory
    {
        Browser,
        Communication,
        Gaming,
        Launcher,
        Utility,
        Media,
        Development,
        Hardware,
        Streaming,
        Productivity
    }
    public enum AppInstallProvider 
    {
        Winget,
    }
}
