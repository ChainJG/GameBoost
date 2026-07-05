using System.IO;

namespace GameBoost.Infrastructure.GameConfigs.BlackOps7
{
    public static class BlackOps7ConfigFileLocator
    {
        public static IReadOnlyList<string> GetBlackOps7ConfigFileLocator()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var playersFolder = Path.Combine(documents, "Activision", "Call of Duty", "players");

            if (!Directory.Exists(playersFolder))
                return [];

            return [.. Directory
                .EnumerateFiles(playersFolder, "s.1.0.cod25.txt*", SearchOption.TopDirectoryOnly)
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)];
        }
    }
}