namespace GameBoost.Infrastructure.GameConfigs.BlackOps7.Entries
{
    public sealed class BlackOps7ConfigEntry
    {
        public required int LineIndex { get; init; }

        public required string FullKey { get; init; }

        public required string SettingName { get; init; }

        public required string Value { get; init; }

        public string? Comment { get; init; }
    }
}
