namespace GameBoost.Shared.Helpers.Game
{
    public static class GameNameHelper
    {
        private static readonly Dictionary<string, string> FriendlyGameNames =
            new(StringComparer.OrdinalIgnoreCase)
        {
            { "cod", "Call of Duty" },
            { "modernwarfare", "Call of Duty" },
            { "warzone", "Call of Duty" },

            { "factorygamesteam", "Satisfactory" },

            { "cs2", "Counter Strike 2" },

            { "PioneerGame", "Arc Raiders" },

            { "leagueclient", "League of Legends" },

            { "rainbowsix", "Rainbow Six Siege" },

            { "vrdashboard", "Virtual Desktop Dashboard" },

            { "gtav", "Grand Theft Auto V" },
        };

        public static string Resolve(string gameName)
        {
            if (string.IsNullOrWhiteSpace(gameName))
                return "Unknown Game";

            // Remove common suffixes
            string[] unwantedParts =
            {
                "-Win64-Shipping",
                "_Win64_Shipping",
                "Win64-Shipping",
                "Win64 Shipping",

                "-Shipping",
                "_Shipping",
                "Shipping",

                "Client",
                "-Client",
                "_Client",

                ".exe"
            };

            foreach (var part in unwantedParts)
            {
                gameName = gameName.Replace(
                    part,
                    "",
                    StringComparison.OrdinalIgnoreCase);
            }

            // Replace separators
            gameName = gameName.Replace("_", " ");
            gameName = gameName.Replace("-", " ");

            // Remove duplicate spaces
            gameName = System.Text.RegularExpressions.Regex
                .Replace(gameName, @"\s+", " ")
                .Trim();

            //// Convert to Title Case
            gameName = System.Globalization.CultureInfo.CurrentCulture
                .TextInfo
                .ToTitleCase(gameName.ToLower());

            foreach (var friendlyName in FriendlyGameNames)
                if (gameName.Contains(friendlyName.Key, StringComparison.OrdinalIgnoreCase))
                    return friendlyName.Value;

            return gameName;
        }
    }
}
