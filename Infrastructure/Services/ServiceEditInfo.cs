using MaterialDesignThemes.Wpf;

namespace GameBoost.Infrastructure.Services
{
    public class ServiceEditInfo
    {
        public required string Name { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PackIconKind Icon { get; init; } = PackIconKind.CogOutline;

        public WindowsServiceStartupMode RecommendedStartupMode { get; init; }
        public string? RecommendationReason { get; init; }

        public bool HighRisk { get; init; }
        public bool SystemReboot { get; init; }
        public bool Admin { get; init; }
    }

    public enum WindowsServiceStartupMode
    {
        Automatic,
        Manual,
        Disabled
    }
}
