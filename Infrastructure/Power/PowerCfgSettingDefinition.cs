namespace GameBoost.Infrastructure.Power
{
    public sealed class PowerCfgSettingDefinition
    {
        public required string Name { get; init; }

        public required string SubGroupAlias { get; init; }

        public required string SettingAlias { get; init; }

        public required string SubGroupGuid { get; init; }

        public required string SettingGuid { get; init; }

        public required int RecommendedAcValue { get; init; }

        public int? RecommendedDcValue { get; init; }

        public bool CheckDcValue { get; init; } = false;
    }
}
