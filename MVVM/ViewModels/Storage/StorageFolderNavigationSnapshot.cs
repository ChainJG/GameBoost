namespace GameBoost.MVVM.ViewModels.Storage
{
    public sealed class StorageFolderNavigationSnapshot
    {
        public required string FullPath { get; init; }

        public StorageFolderNodeViewModel? OpenedFolder { get; init; }

        public required IReadOnlyList<StorageFolderNodeViewModel> Folders { get; init; }

        public required long TotalSizeBytes { get; init; }

        public DateTimeOffset CreatedAtUtc { get; init; } =
            DateTimeOffset.UtcNow;
    }
}
