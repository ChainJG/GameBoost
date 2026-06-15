namespace GameBoost.Features.Storage.Models
{
    public sealed class StorageFolderNode
    {
        public string Name { get; init; } = string.Empty;

        public string FullPath { get; init; } = string.Empty;

        public long SizeBytes { get; set; }

        public int FileCount { get; set; }

        public int FolderCount { get; set; }

        public bool IsAccessible { get; set; } = true;

        public List<StorageFolderNode> Children { get; } = [];
    }
}
