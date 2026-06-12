namespace GameBoost.Application.Diagnostics
{
    public sealed class DiagnosticEntry
    {
        public required string SessionId { get; init; }

        public DateTimeOffset StartedAtUtc { get; init; }

        public DateTimeOffset CompletedAtUtc { get; init; }

        public required string Category { get; init; }

        public required DiagnosticOperationType OperationType { get; init; }

        public required string Name { get; init; }

        public string Source { get; init; } = string.Empty;

        public ResultType State { get; init; }

        public double DurationMilliseconds { get; init; }

        public bool? Success { get; init; }

        public string? ResultStatus { get; init; }

        public string? Message { get; init; }

        public string? ExceptionType { get; init; }

        public string? ExceptionMessage { get; init; }
    }

    public enum DiagnosticOperationType
    {
        StartupStep,
        ModuleRefresh,
        ModuleExecute,
        ModuleRecommendedExecute,
        DirectoryScan,
        Cleanup,
        ShellCommand,
        RegistryOperation,
        Custom
    }
}
