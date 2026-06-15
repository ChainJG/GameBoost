using System.IO;

namespace GameBoost.Shared.FileSystem.Scanning
{
    public sealed class FileSystemScanOptions
    {
        public bool IgnoreInaccessiblePaths { get; init; } = true;

        public bool SkipReparsePoints { get; init; } = true;

        public bool SkipHiddenDirectories { get; init; }

        public bool SkipSystemDirectories { get; init; }

        public int? MaxDepth { get; init; }

        public int MaxDegreeOfParallelism { get; init; } =
            Math.Clamp(Environment.ProcessorCount / 2, 2, 6);

        public IReadOnlySet<string> ExcludedTopLevelDirectoryNames { get; init; } =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public IReadOnlySet<string> ExcludedDirectoryNames { get; init; } =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public IReadOnlySet<string> ExcludedFileNames { get; init; } =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public IReadOnlyList<string> ExcludedPathPrefixes { get; init; } = [];
    }
}