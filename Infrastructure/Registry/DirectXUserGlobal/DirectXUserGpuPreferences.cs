using GameBoost.Shared.Helpers.Game;
using System.Diagnostics;
using System.IO;

namespace GameBoost.Infrastructure.Registry.DirectXUserGlobal
{
    public sealed class DirectXUserGpuPreferences
    {
        private readonly static RegistryEditInfo DiscoveryRegistry = new()
        {
            Hive = Microsoft.Win32.RegistryHive.CurrentUser,
            Path = RegistryConstants.DirectXUserGpuPreferences
        };

        public static List<RegistryResult> GetDirectXUserGpuPreferencesGames()
        {
            var games = new List<RegistryResult>();

            try
            {
                var result = RegistryHelper.OpenKey(DiscoveryRegistry);

                var valueNames = result.Key?.GetValueNames();

                if (valueNames == null)
                    return games;

                foreach (var exePath in valueNames)
                {
                    try
                    {
                        if (!exePath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                            continue;

                        var valueData = result.Key?.GetValue(exePath)?.ToString();

                        if (!File.Exists(exePath))
                            continue;

                        var gameInfo = GetGameInfo(exePath);

                        if (gameInfo != null)
                            games.Add(gameInfo);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debug.WriteLine($"Error in GetDirectXUserGpuPreferencesGames: {ex.Message}");
#endif
                    }
                }

                return [.. games.OrderBy(g => g.Message)];
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error in GetDirectXUserGpuPreferencesGames: {ex.Message}");
#endif
                return games;
            }
        }

        private static RegistryResult? GetGameInfo(string exePath)
        {
            try
            {
                string lowerPath = exePath.ToLowerInvariant();

                string exeName = Path.GetFileNameWithoutExtension(exePath);

                if (!GameDetectionHelper.IsGameFromExe(exePath))
                    return null;

                string friendlyName = GameNameHelper.Resolve(exeName);

                return new RegistryResult
                {
                    Value = exePath,
                    Message = friendlyName
                };
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error in GetGameInfo: {ex.Message}");
#endif
                return null;
            }
        }
    }
}
