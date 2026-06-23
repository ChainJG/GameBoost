namespace GameBoost.Infrastructure.Installers.Models
{
    public sealed class AppInstallResult
    {
        public required string AppId { get; init; }

        public required string DisplayName { get; init; }

        public required bool Success { get; init; }

        public required bool Cancelled { get; init; }

        public int? ExitCode { get; init; }

        public string Message { get; init; } = string.Empty;
    }
}
