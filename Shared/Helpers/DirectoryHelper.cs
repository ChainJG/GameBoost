using System.Diagnostics;
using System.IO;
using System.Runtime;

namespace GameBoost.Shared.Helpers
{
    public static class DirectoryHelper
    {
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

                return size;
            }
            catch
            {
                return 0;
            }
        }

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

        public static bool CanDeleteFile(FileInfo file)
        {
            try
            {
                // Skip readonly files
                if (file.IsReadOnly)
                    return false;

                // Try opening with exclusive access
                using var stream = file.Open(
                    FileMode.Open,
                    FileAccess.ReadWrite,
                    FileShare.None);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
