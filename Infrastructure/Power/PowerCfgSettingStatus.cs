namespace GameBoost.Infrastructure.Power
{
    public sealed class PowerCfgSettingStatus
    {
        public ToggleType Status { get; init; } = ToggleType.Unknown;

        public int? CurrentAcValue { get; init; }

        public int? CurrentDcValue { get; init; }

        public int RecommendedAcValue { get; init; }

        public int? RecommendedDcValue { get; init; }

        public string Message { get; init; } = string.Empty;

        public bool HasValue =>
            CurrentAcValue is not null;

        public bool IsRecommended =>
            Status == ToggleType.Enabled;
    }
}
