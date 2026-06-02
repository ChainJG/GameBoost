using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoost.Shared.Helpers.Game
{
    public static class GameDetectionHelper
    {
        public static readonly string[] GameExeIndicators =
        {
             "steamapps",
             "epic games",
             "fortnitegame",
             "riot games",
             "valorant",
             "call of duty",
             "battle.net",
             "ubisoft",
             "ea games",
             "minecraft",
             "elden ring",
             "apex",
             "rainbowsix",
             "rockstar games",
             "overwatch",
        };

        public static bool IsGameFromExe(string exePath)
        {
            if (string.IsNullOrWhiteSpace(exePath))
                return false;

            return GameExeIndicators.Any(x =>
                exePath.Contains(
                    x,
                    StringComparison.OrdinalIgnoreCase));
        }
    }
}
