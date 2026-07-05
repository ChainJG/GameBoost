using GameBoost.Infrastructure.Installers.Models;
using MaterialDesignThemes.Wpf;

namespace GameBoost.Infrastructure.Installers.Catalog
{
    public static class AppInstallCatalog
    {
        public static IReadOnlyList<AppInstallDefinition> GetApps()
        {
            return Apps;
        }

        private static readonly AppInstallDefinition[] Apps =
        [
            // ============================================================
            // Browsers
            // ============================================================

            Winget(
                id: "google-chrome",
                displayName: "Google Chrome",
                description: "Modern web browser",
                category: AppInstallCategory.Browser,
                icon: PackIconKind.GoogleChrome,
                wingetId: "Google.Chrome",
                installedProgramNames: ["Google Chrome"],
                processNames: ["chrome"],
                tags: ["Browser", "Google", "Internet"],
                sortOrder: 10),

            Winget(
                id: "mozilla-firefox",
                displayName: "Mozilla Firefox",
                description: "Classic web browser",
                category: AppInstallCategory.Browser,
                icon: PackIconKind.MozillaFirefox,
                wingetId: "Mozilla.Firefox",
                installedProgramNames: ["Mozilla Firefox"],
                processNames: ["firefox"],
                tags: ["Browser", "Mozilla", "Internet"],
                sortOrder: 20),

            Winget(
                id: "brave-browser",
                displayName: "Brave Browser",
                description: "Privacy web browser",
                icon: PackIconKind.Shield,
                category: AppInstallCategory.Browser,
                wingetId: "Brave.Brave",
                installedProgramNames: ["Brave"],
                processNames: ["brave"],
                tags: ["Browser", "Privacy", "Internet"],
                sortOrder: 30),


            Winget(
                id: "opera",
                displayName: "Opera",
                description: "Gaming web browser",
                icon: PackIconKind.Opera,
                category: AppInstallCategory.Browser,
                wingetId: "Opera.Opera",
                installedProgramNames: ["Opera"],
                processNames: ["opera"],
                tags: ["Browser", "Internet"],
                sortOrder: 50),

            Winget(
                id: "opera-gx",
                displayName: "Opera GX",
                description: "Gaming web browser",
                icon: PackIconKind.Opera,
                category: AppInstallCategory.Browser,
                wingetId: "Opera.OperaGX",
                installedProgramNames: ["Opera GX"],
                processNames: ["opera"],
                tags: ["Browser", "Gaming", "Internet"],
                sortOrder: 60),

            // ============================================================
            // Communication
            // ============================================================

            Winget(
                id: "discord",
                displayName: "Discord",
                description: "Gaming voice chat",
                icon: PackIconKind.VoiceChat,
                category: AppInstallCategory.Communication,
                wingetId: "Discord.Discord",
                installedProgramNames: ["Discord"],
                processNames: ["Discord"],
                tags: ["Gaming", "Voice", "Chat", "Communication"],
                sortOrder: 100),

            Winget(
                id: "telegram",
                displayName: "Telegram Desktop",
                description: "Desktop messaging app",
                category: AppInstallCategory.Communication,
                icon: PackIconKind.Telephone,
                wingetId: "Telegram.TelegramDesktop",
                installedProgramNames: ["Telegram Desktop"],
                processNames: ["Telegram"],
                tags: ["Chat", "Messaging", "Communication"],
                sortOrder: 120),

            Winget(
                id: "whatsapp",
                displayName: "WhatsApp",
                description: "Common messaging app",
                icon: PackIconKind.Whatsapp,
                category: AppInstallCategory.Communication,
                wingetId: "WhatsApp.WhatsApp",
                installedProgramNames: ["WhatsApp"],
                processNames: ["WhatsApp"],
                tags: ["Chat", "Messaging", "Communication"],
                sortOrder: 140),

            Winget(
                id: "zoom",
                displayName: "Zoom",
                description: "Video meetings app",
                icon: PackIconKind.ZoomPlus,
                category: AppInstallCategory.Communication,
                wingetId: "Zoom.Zoom",
                installedProgramNames: ["Zoom"],
                processNames: ["Zoom"],
                tags: ["Video", "Meetings", "Communication"],
                sortOrder: 150),

            Winget(
                id: "skype",
                displayName: "Skype",
                description: "Voice, video, and messaging app",
                icon: PackIconKind.Skype,
                category: AppInstallCategory.Communication,
                wingetId: "Microsoft.Skype",
                installedProgramNames: ["Skype"],
                processNames: ["Skype"],
                tags: ["Voice", "Video", "Communication"],
                sortOrder: 170),

            Winget(
                id: "teamspeak",
                displayName: "TeamSpeak",
                description: "Team voice",
                icon: PackIconKind.SpeakerWireless,
                category: AppInstallCategory.Communication,
                wingetId: "TeamSpeakSystems.TeamSpeakClient",
                installedProgramNames: ["TeamSpeak"],
                processNames: ["ts3client_win64", "TeamSpeak"],
                tags: ["Gaming", "Voice", "Communication"],
                sortOrder: 180),


            // ============================================================
            // Gaming and Launchers
            // ============================================================

            Winget(
                id: "steam",
                displayName: "Steam",
                description: "Launch PC games",
                icon: PackIconKind.Steam,
                category: AppInstallCategory.Launcher,
                wingetId: "Valve.Steam",
                installedProgramNames: ["Steam"],
                processNames: ["steam"],
                tags: ["Gaming", "Launcher", "Games"],
                sortOrder: 200),

            Winget(
                id: "epic-games-launcher",
                displayName: "Epic Games",
                description: "Epic games launcher",
                icon: PackIconKind.Games,
                category: AppInstallCategory.Launcher,
                wingetId: "EpicGames.EpicGamesLauncher",
                installedProgramNames: ["Epic Games Launcher"],
                processNames: ["EpicGamesLauncher"],
                tags: ["Gaming", "Launcher", "Games"],
                sortOrder: 210),

            Winget(
                id: "gog-galaxy",
                displayName: "GOG Galaxy",
                description: "DRM free launcher",
                icon: PackIconKind.Games,
                category: AppInstallCategory.Launcher,
                wingetId: "GOG.Galaxy",
                installedProgramNames: ["GOG GALAXY"],
                processNames: ["GalaxyClient"],
                tags: ["Gaming", "Launcher", "Games"],
                sortOrder: 220),

            Winget(
                id: "ea-app",
                displayName: "EA App",
                description: "Electronic Arts games",
                icon: PackIconKind.GamepadSquare,
                category: AppInstallCategory.Launcher,
                wingetId: "ElectronicArts.EADesktop",
                installedProgramNames: ["EA app", "EA Desktop"],
                processNames: ["EADesktop", "EALauncher"],
                tags: ["Gaming", "Launcher", "Games"],
                sortOrder: 230),

            Winget(
                id: "ubisoft-connect",
                displayName: "Ubisoft Connect",
                description: "Ubisoft games",
                icon: PackIconKind.Ubisoft,
                category: AppInstallCategory.Launcher,
                wingetId: "Ubisoft.Connect",
                installedProgramNames: ["Ubisoft Connect"],
                processNames: ["upc", "UbisoftConnect"],
                tags: ["Gaming", "Launcher", "Games"],
                sortOrder: 240),

            Winget(
                id: "battle-net",
                displayName: "Battle.net",
                description: "Blizzard games",
                icon: PackIconKind.GamesOutline,
                category: AppInstallCategory.Launcher,
                wingetId: "Blizzard.BattleNet",
                installedProgramNames: ["Battle.net"],
                processNames: ["Battle.net"],
                tags: ["Gaming", "Launcher", "Games"],
                sortOrder: 250),

            Winget(
                id: "heroic-games-launcher",
                displayName: "Heroic Games Launcher",
                description: "Open source launcher",
                icon: PackIconKind.Games,
                category: AppInstallCategory.Launcher,
                wingetId: "HeroicGamesLauncher.HeroicGamesLauncher",
                installedProgramNames: ["Heroic"],
                processNames: ["Heroic"],
                tags: ["Gaming", "Launcher", "Open Source"],
                sortOrder: 260),

            // ============================================================
            // Utilities
            // ============================================================

            Winget(
                id: "7zip",
                displayName: "7-Zip",
                description: "File archiver for ZIP",
                icon: PackIconKind.FolderZip,
                category: AppInstallCategory.Utility,
                wingetId: "7zip.7zip",
                installedProgramNames: ["7-Zip"],
                processNames: ["7zFM", "7zG"],
                tags: ["Archive", "Compression", "Utility"],
                sortOrder: 400),

            Winget(
                id: "winrar",
                displayName: "WinRAR",
                icon: PackIconKind.FolderZip,
                description: "Manager compressed files",
                category: AppInstallCategory.Utility,
                wingetId: "RARLab.WinRAR",
                installedProgramNames: ["WinRAR"],
                processNames: ["WinRAR"],
                tags: ["Archive", "Compression", "Utility"],
                sortOrder: 410),

            Winget(
                id: "notepad-plus-plus",
                displayName: "Notepad++",
                description: "Text and code editor",
                icon: PackIconKind.Notebook,
                category: AppInstallCategory.Utility,
                wingetId: "Notepad++.Notepad++",
                installedProgramNames: ["Notepad++"],
                processNames: ["notepad++"],
                tags: ["Text Editor", "Utility", "Code"],
                sortOrder: 420),

            Winget(
                id: "powertoys",
                displayName: "Microsoft PowerToys",
                description: "Microsoft utility tools",
                icon: PackIconKind.Wrench,
                category: AppInstallCategory.Utility,
                wingetId: "Microsoft.PowerToys",
                installedProgramNames: ["Microsoft PowerToys"],
                processNames: ["PowerToys"],
                tags: ["Microsoft", "Utility", "Productivity"],
                sortOrder: 440),

            // ============================================================
            // Media
            // ============================================================

            Winget(
                id: "vlc",
                displayName: "VLC Media Player",
                description: "Media player",
                icon: PackIconKind.Vlc,
                category: AppInstallCategory.Media,
                wingetId: "VideoLAN.VLC",
                installedProgramNames: ["VLC media player"],
                processNames: ["vlc"],
                tags: ["Video", "Audio", "Media"],
                sortOrder: 600),

            Winget(
                id: "spotify",
                displayName: "Spotify",
                description: "Music desktop app",
                icon: PackIconKind.Spotify,
                category: AppInstallCategory.Media,
                wingetId: "Spotify.Spotify",
                installedProgramNames: ["Spotify"],
                processNames: ["Spotify"],
                tags: ["Music", "Audio", "Media"],
                sortOrder: 610),

            Winget(
                id: "audacity",
                displayName: "Audacity",
                icon: PackIconKind.AudioInputXlr,
                description: "Audio recording app",
                category: AppInstallCategory.Media,
                wingetId: "Audacity.Audacity",
                installedProgramNames: ["Audacity"],
                processNames: ["audacity"],
                tags: ["Audio", "Editor", "Media"],
                sortOrder: 620),

            Winget(
                id: "blender",
                displayName: "Blender",
                description: "3D modelling app",
                icon: PackIconKind.Blender,
                category: AppInstallCategory.Media,
                wingetId: "BlenderFoundation.Blender",
                installedProgramNames: ["Blender"],
                processNames: ["blender"],
                tags: ["3D", "Rendering", "Media"],
                sortOrder: 700),

            // ============================================================
            // Streaming
            // ============================================================

            Winget(
                id: "obs-studio",
                displayName: "OBS Studio",
                description: "Live streaming app",
                icon: PackIconKind.RecordCircle,
                category: AppInstallCategory.Streaming,
                wingetId: "OBSProject.OBSStudio",
                installedProgramNames: ["OBS Studio"],
                processNames: ["obs64", "obs32"],
                tags: ["Streaming", "Recording", "Video"],
                sortOrder: 800),

            Winget(
                id: "streamlabs",
                displayName: "Streamlabs Desktop",
                description: "Streaming and recording app",
                icon: PackIconKind.RecordCircle,
                category: AppInstallCategory.Streaming,
                wingetId: "Streamlabs.Streamlabs",
                installedProgramNames: ["Streamlabs Desktop"],
                processNames: ["Streamlabs Desktop"],
                tags: ["Streaming", "Recording", "Video"],
                sortOrder: 810),

            Winget(
                id: "elgato-stream-deck",
                displayName: "Elgato Stream Deck",
                description: "Control Elgato Stream Deck",
                icon: PackIconKind.ControlPoint,
                category: AppInstallCategory.Streaming,
                wingetId: "Elgato.StreamDeck",
                installedProgramNames: ["Elgato Stream Deck", "Stream Deck"],
                processNames: ["StreamDeck"],
                tags: ["Streaming", "Elgato", "Hardware"],
                sortOrder: 820),

            Winget(
                id: "voicemod",
                displayName: "Voicemod",
                description: "Voice changer app",
                icon: PackIconKind.SettingsVoice,
                category: AppInstallCategory.Streaming,
                wingetId: "Voicemod.Voicemod",
                installedProgramNames: ["Voicemod"],
                processNames: ["VoicemodDesktop"],
                tags: ["Streaming", "Audio", "Voice"],
                sortOrder: 830),

            // ============================================================
            // Hardware
            // ============================================================

            Winget(
                id: "hwinfo",
                displayName: "HWiNFO",
                description: "Hardware monitoring tool",
                icon: PackIconKind.HardHat,
                category: AppInstallCategory.Hardware,
                wingetId: "REALiX.HWiNFO",
                installedProgramNames: ["HWiNFO"],
                processNames: ["HWiNFO64", "HWiNFO32"],
                tags: ["Hardware", "Monitoring", "Sensors"],
                sortOrder: 1200),

            Winget(
                id: "cpu-z",
                displayName: "CPU-Z",
                description: "System information tool",
                icon: PackIconKind.Cpu64Bit,
                category: AppInstallCategory.Hardware,
                wingetId: "CPUID.CPU-Z",
                installedProgramNames: ["CPUID CPU-Z", "CPU-Z"],
                processNames: ["cpuz"],
                tags: ["Hardware", "CPU", "System Info"],
                sortOrder: 1210),

            Winget(
                id: "gpu-z",
                displayName: "GPU-Z",
                description: "GPU monitoring tool",
                icon: PackIconKind.Gpu,
                category: AppInstallCategory.Hardware,
                wingetId: "TechPowerUp.GPU-Z",
                installedProgramNames: ["GPU-Z"],
                processNames: ["GPU-Z"],
                tags: ["Hardware", "GPU", "System Info"],
                sortOrder: 1220),

            Winget(
                id: "crystaldiskinfo",
                displayName: "CrystalDiskInfo",
                description: "Drive health tool",
                icon: PackIconKind.Drive,
                category: AppInstallCategory.Hardware,
                wingetId: "CrystalDewWorld.CrystalDiskInfo",
                installedProgramNames: ["CrystalDiskInfo"],
                processNames: ["DiskInfo64", "DiskInfo32"],
                tags: ["Hardware", "Storage", "Disk"],
                sortOrder: 1230),

            Winget(
                id: "crystaldiskmark",
                displayName: "CrystalDiskMark",
                description: "Storage benchmark tool",
                icon: PackIconKind.Drive,
                category: AppInstallCategory.Hardware,
                wingetId: "CrystalDewWorld.CrystalDiskMark",
                installedProgramNames: ["CrystalDiskMark"],
                processNames: ["DiskMark64", "DiskMark32"],
                tags: ["Hardware", "Storage", "Benchmark"],
                sortOrder: 1240),

            Winget(
                id: "logitech-ghub",
                displayName: "Logitech G HUB",
                description: "Confiure Logitech devices",
                category: AppInstallCategory.Hardware,
                icon: PackIconKind.Mouse,
                wingetId: "Logitech.GHUB",
                installedProgramNames: ["Logitech G HUB"],
                processNames: ["lghub", "lghub_agent"],
                tags: ["Hardware", "Logitech", "Gaming"],
                sortOrder: 1250),

            Winget(
                id: "razer-synapse",
                displayName: "Razer Synapse",
                description: "Configure Razer devices",
                icon: PackIconKind.RazorDoubleEdge,
                category: AppInstallCategory.Hardware,
                wingetId: "Razer.Synapse.3",
                installedProgramNames: ["Razer Synapse"],
                processNames: ["Razer Synapse"],
                tags: ["Hardware", "Razer", "Gaming"],
                sortOrder: 1270),

            Winget(
                id: "msi-afterburner",
                displayName: "MSI Afterburner",
                description: "Tuning utility",
                category: AppInstallCategory.Hardware,
                icon: PackIconKind.Mister,
                wingetId: "Guru3D.Afterburner",
                installedProgramNames: ["MSI Afterburner"],
                processNames: ["MSIAfterburner"],
                tags: ["Hardware", "GPU", "Monitoring"],
                sortOrder: 1310,
                requiresAdmin: true),
        ];

        private static AppInstallDefinition Winget(
            string id,
            string displayName,
            string description,
            AppInstallCategory category,
            string wingetId,
            string[] installedProgramNames,
            string[] processNames,
            string[] tags,
            int sortOrder,
            PackIconKind? icon = null,
            bool requiresAdmin = false,
            bool requiresRestart = false,
            bool supportsSilentInstall = true)
        {
            return new AppInstallDefinition
            {
                Id = id,
                DisplayName = displayName,
                Description = description,
                Icon = icon ?? GetCategoryIcon(category),
                Category = category,
                Provider = AppInstallProvider.Winget,
                WingetId = wingetId,
                WingetSource = "winget",
                RequiresAdmin = requiresAdmin,
                RequiresRestart = requiresRestart,
                SupportsSilentInstall = supportsSilentInstall,
                ProcessNames = processNames,
                InstalledProgramNames = installedProgramNames,
                Tags = tags,
                SortOrder = sortOrder
            };
        }

        private static PackIconKind GetCategoryIcon(AppInstallCategory category)
        {
            return category switch
            {
                AppInstallCategory.Browser => PackIconKind.OpenInBrowser,
                AppInstallCategory.Communication => PackIconKind.MessageText,
                AppInstallCategory.Launcher => PackIconKind.Rocket,
                AppInstallCategory.Utility => PackIconKind.Tools,
                AppInstallCategory.Media => PackIconKind.PlayCircle,
                AppInstallCategory.Development => PackIconKind.CodeTags,
                AppInstallCategory.Hardware => PackIconKind.Chip,
                AppInstallCategory.Streaming => PackIconKind.Cast,
                AppInstallCategory.Productivity => PackIconKind.Briefcase,
                _ => PackIconKind.Category
            };
        }
    }
}