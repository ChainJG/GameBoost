namespace GameBoost.Infrastructure.GameConfigs.BlackOps7.Settings
{
    public sealed class BlackOps7PresetDefinition
    {
        public required string Id { get; init; }

        public required string DisplayName { get; init; }

        public required string Description { get; init; }

        public required IReadOnlyList<BlackOps7SettingChange> Changes { get; init; }
    }
}
