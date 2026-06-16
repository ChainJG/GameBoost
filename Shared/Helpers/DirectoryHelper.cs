using System.Diagnostics;
using System.IO;

namespace GameBoost.Shared.Helpers
{
    public static class DirectoryHelper
    {
        private static bool DebugOutput { get; set; } = false;

        #region Open File In Explorer
        public static void OpenFileInExplorer(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            if (!File.Exists(filePath))
            {
                Debug.WriteLine($"File does not exist: {filePath}");
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"/select,\"{filePath}\"",
                UseShellExecute = true
            });
        }
        public static void OpenFolderInExplorer(string? folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                return;
            if (!Directory.Exists(folderPath))
            {
                Debug.WriteLine($"Directory does not exist: {folderPath}");
                return;
            }
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"\"{folderPath}\"",
                UseShellExecute = true
            });
        }
        #endregion

        #region Get Directory Size
        public static long GetDirectorySize(DirectoryInfo directory)
        {
            if (!directory.Exists) return 0;

            try
            {
                long size = 0;

                // Sum files in current directory
                foreach (var file in directory.EnumerateFiles())
                {
                    try 
                    {
                        size += file.Length; 
                    }
                    catch 
                    {
                        // skip inaccessible files
                    }
                }

                // Recursively sum subdirectories
                foreach (var subDir in directory.EnumerateDirectories())
                {
                    try 
                    {
                        size += GetDirectorySize(subDir);
                    }
                    catch 
                    {
                        // skip inaccessible directories
                    }
                }

                if (DebugOutput)
                    Debug.WriteLine($"Directory: {directory.FullName} | Size: {MathHelper.FormatBytes(size)}");

                return size;
            }
            catch
            {
#if DEBUG
                if (DebugOutput)
                    Debug.WriteLine($"Failed to get directory size for {directory.FullName}");
#endif
                return 0;
            }
        }
        #endregion

        #region Delete Directory
        public static void DeleteDirectory(DirectoryInfo directory, CancellationToken token)
        {
            // Skip scanning this directory
            if (!Directory.Exists(directory.FullName))
                return;

            // Delete all files in the directory
            DeleteAllFiles(directory, token);

            // Recursively delete all subdirectories
            foreach (DirectoryInfo subDir in directory.GetDirectories())
            {
                try
                {
                    token.ThrowIfCancellationRequested();

                    subDir.Delete(true);
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (IOException)
                {
                }
            }
        }
        private static void DeleteAllFiles(DirectoryInfo directory, CancellationToken token)
        {
            foreach (FileInfo fileInfo in directory.GetFiles("*.*"))
            {
                try
                {
                    token.ThrowIfCancellationRequested();

                    File.Delete(fileInfo.FullName);
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (IOException)
                {
                }
            }
        }
        #endregion

        public static bool IsRootDirectory(string folderPath)
        {
            var fullPath = Path.GetFullPath(folderPath);
            var root = Path.GetPathRoot(fullPath);

            if (string.IsNullOrWhiteSpace(root))
                return false;

            return string.Equals(
                TrimDirectoryEnd(fullPath),
                TrimDirectoryEnd(root),
                StringComparison.OrdinalIgnoreCase);
        }
        public static string GetFolderDisplayName(string folderPath)
        {
            var fullPath = NormalizeFolderPath(folderPath);
            var root = Path.GetPathRoot(fullPath);

            if (string.Equals(fullPath, root, StringComparison.OrdinalIgnoreCase))
                return fullPath;

            return Path.GetFileName(fullPath);
        }
        public static string NormalizeFolderPath(string folderPath)
        {
            var fullPath = Path.GetFullPath(folderPath);

            var root = Path.GetPathRoot(fullPath);

            if (string.Equals(fullPath, root, StringComparison.OrdinalIgnoreCase))
                return fullPath;

            return fullPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
        private static string TrimDirectoryEnd(string path)
        {
            return path.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);
        }
    }
}
