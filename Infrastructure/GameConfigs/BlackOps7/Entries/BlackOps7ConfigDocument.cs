namespace GameBoost.Infrastructure.GameConfigs.BlackOps7.Entries
{
    public sealed class BlackOps7ConfigDocument
    {
        public required string Path { get; init; }

        public required List<string> Lines { get; init; }

        public required IReadOnlyList<BlackOps7ConfigEntry> Entries { get; init; }
    }
}
