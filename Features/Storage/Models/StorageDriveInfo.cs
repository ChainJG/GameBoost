namespace GameBoost.Features.Storage.Models
{
    public sealed class StorageDriveInfo
    {
        public string Name { get; init; } = string.Empty;

        public string RootPath { get; init; } = string.Empty;

        public long TotalBytes { get; init; }

        public long FreeBytes { get; init; }

        public long UsedBytes => TotalBytes - FreeBytes;
    }
}
