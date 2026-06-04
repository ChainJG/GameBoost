using System.Diagnostics;
using System.IO;

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
    }
}
