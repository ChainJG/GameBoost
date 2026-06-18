namespace GameBoost.Features.Modules.WindowsModules.Gaming.GameFocus
{
    public sealed class GamingFocusProcessDefinition
    {
        public required string ProcessName { get; init; }

        public required string DisplayName { get; init; }

        public required string Reason { get; init; }

        public bool TryGracefulClose { get; init; } = true;

        public bool AllowForceKill { get; init; }

        public bool EnabledByDefault { get; init; } = true;
    }
}