using GameBoost.Infrastructure.Installers.Models;

namespace GameBoost.Infrastructure.Installers.Catalog
{
    public static class AppInstallCatalog
    {
        public static IReadOnlyList<AppInstallDefinition> AppInstallDefinitions { get; } =
        [
            new()
            {
                Id = "discord",
                DisplayName = "Discord",
                Description = "Voice, chat, and community app often used by gamers.",
                Category = AppInstallCategory.Communication,
                Provider = AppInstallProvider.Winget,
                WingetId = "Discord.Discord",
                WingetSource = "winget",
                ProcessNames = ["Discord"],
                InstalledProgramNames = ["Discord"],
                Tags = ["Gaming", "Voice Chat", "Communication"]
            },

            new()
            {
                Id = "winrar",
                DisplayName = "WinRAR",
                Description = "Archive manager for RAR, ZIP, and compressed files.",
                Category = AppInstallCategory.Utility,
                Provider = AppInstallProvider.Winget,
                WingetId = "RARLab.WinRAR",
                WingetSource = "winget",
                ProcessNames = ["WinRAR"],
                InstalledProgramNames = ["WinRAR"],
                Tags = ["Archive", "Utility", "Compression"]
            },
        ];
    }
}
