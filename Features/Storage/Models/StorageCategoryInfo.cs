namespace GameBoost.Features.Storage.Models
{
    public sealed class StorageCategoryInfo
    {
        public string Name { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public long SizeBytes { get; init; }

        public double PercentageOfDrive { get; init; }
    }
}
