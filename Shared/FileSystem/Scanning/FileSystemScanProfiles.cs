namespace GameBoost.Shared.FileSystem.Scanning
{
    public static class FileSystemScanProfiles
    {
        public static FileSystemScanOptions StorageTopFolders => new()
        {
            IgnoreInaccessiblePaths = true,
            SkipReparsePoints = true,
            SkipHiddenDirectories = false,
            SkipSystemDirectories = false,

            ExcludedTopLevelDirectoryNames =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "$Recycle.Bin",
                    "$WINDOWS.~BT",
                    "$WINDOWS.~WS",
                    "$WinREAgent",
                    "$GetCurrent",
                    "$SysReset",
                    "Recovery",
                    "System Volume Information",
                    "Config.Msi",
                    "MSOCache",
                    "PerfLogs",
                    "Documents and Settings"
                }
        };

        public static FileSystemScanOptions StorageCategoryScan => new()
        {
            IgnoreInaccessiblePaths = true,
            SkipReparsePoints = true,
            SkipHiddenDirectories = false,
            SkipSystemDirectories = false,

            ExcludedTopLevelDirectoryNames =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "$Recycle.Bin",
                    "System Volume Information",
                    "Recovery"
                }
        };

        public static FileSystemScanOptions UserCleanupScan => new()
        {
            IgnoreInaccessiblePaths = true,
            SkipReparsePoints = true,
            SkipHiddenDirectories = false,
            SkipSystemDirectories = true,

            ExcludedDirectoryNames =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "Windows",
                    "System32",
                    "WinSxS",
                    "Program Files",
                    "Program Files (x86)"
                }
        };

        public static FileSystemScanOptions GameLibraryDiscovery => new()
        {
            IgnoreInaccessiblePaths = true,
            SkipReparsePoints = true,
            SkipHiddenDirectories = true,
            SkipSystemDirectories = true,
            MaxDepth = 4
        };
    }
}