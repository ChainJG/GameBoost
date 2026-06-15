using System.IO;

namespace GameBoost.Shared.FileSystem.Scanning
{
    public static class FileSystemScanFilter
    {
        public static bool ShouldSkipDirectory(DirectoryInfo directory, FileSystemScanOptions options, int depth, bool isTopLevel)
        {
            try
            {
                if (options.MaxDepth is int maxDepth && depth > maxDepth)
                    return true;

                if (isTopLevel &&
                    options.ExcludedTopLevelDirectoryNames.Contains(directory.Name))
                    return true;

                if (options.ExcludedDirectoryNames.Contains(directory.Name))
                    return true;

                if (IsExcludedPathPrefix(directory.FullName, options))
                    return true;

                var attributes = directory.Attributes;

                if (options.SkipReparsePoints &&
                    attributes.HasFlag(FileAttributes.ReparsePoint))
                    return true;

                if (options.SkipHiddenDirectories &&
                    attributes.HasFlag(FileAttributes.Hidden))
                    return true;

                if (options.SkipSystemDirectories &&
                    attributes.HasFlag(FileAttributes.System))
                    return true;

                return false;
            }
            catch
            {
                return options.IgnoreInaccessiblePaths;
            }
        }

        public static bool ShouldSkipFile(FileInfo file, FileSystemScanOptions options)
        {
            try
            {
                if (options.ExcludedFileNames.Contains(file.Name))
                    return true;

                if (IsExcludedPathPrefix(file.FullName, options))
                    return true;

                return false;
            }
            catch
            {
                return options.IgnoreInaccessiblePaths;
            }
        }

        private static bool IsExcludedPathPrefix(string path, FileSystemScanOptions options) =>
            options.ExcludedPathPrefixes.Any(prefix =>
                path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }
}