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
                description: "Fast and widely supported web browser from Google.",
                category: AppInstallCategory.Browser,
                wingetId: "Google.Chrome",
                installedProgramNames: ["Google Chrome"],
                processNames: ["chrome"],
                tags: ["Browser", "Google", "Internet"],
                sortOrder: 10),

            Winget(
                id: "mozilla-firefox",
                displayName: "Mozilla Firefox",
                description: "Privacy-focused open-source web browser.",
                category: AppInstallCategory.Browser,
                wingetId: "Mozilla.Firefox",
                installedProgramNames: ["Mozilla Firefox"],
                processNames: ["firefox"],
                tags: ["Browser", "Mozilla", "Internet"],
                sortOrder: 20),

            Winget(
                id: "brave-browser",
                displayName: "Brave Browser",
                description: "Privacy-focused Chromium-based browser with built-in tracker blocking.",
                category: AppInstallCategory.Browser,
                wingetId: "Brave.Brave",
                installedProgramNames: ["Brave"],
                processNames: ["brave"],
                tags: ["Browser", "Privacy", "Internet"],
                sortOrder: 30),

            Winget(
                id: "vivaldi",
                displayName: "Vivaldi",
                description: "Customisable Chromium-based browser for power users.",
                category: AppInstallCategory.Browser,
                wingetId: "VivaldiTechnologies.Vivaldi",
                installedProgramNames: ["Vivaldi"],
                processNames: ["vivaldi"],
                tags: ["Browser", "Internet", "Customisable"],
                sortOrder: 40),

            Winget(
                id: "opera",
                displayName: "Opera",
                description: "Feature-rich Chromium-based browser.",
                category: AppInstallCategory.Browser,
                wingetId: "Opera.Opera",
                installedProgramNames: ["Opera"],
                processNames: ["opera"],
                tags: ["Browser", "Internet"],
                sortOrder: 50),

            Winget(
                id: "opera-gx",
                displayName: "Opera GX",
                description: "Gaming-focused browser with resource controls and gaming integrations.",
                category: AppInstallCategory.Browser,
                wingetId: "Opera.OperaGX",
                installedProgramNames: ["Opera GX"],
                processNames: ["opera"],
                tags: ["Browser", "Gaming", "Internet"],
                sortOrder: 60),

            Winget(
                id: "microsoft-edge",
                displayName: "Microsoft Edge",
                description: "Microsoft's Chromium-based browser.",
                category: AppInstallCategory.Browser,
                wingetId: "Microsoft.Edge",
                installedProgramNames: ["Microsoft Edge"],
                processNames: ["msedge"],
                tags: ["Browser", "Microsoft", "Internet"],
                sortOrder: 70),

            Winget(
                id: "waterfox",
                displayName: "Waterfox",
                description: "Firefox-based browser focused on customisation and privacy.",
                category: AppInstallCategory.Browser,
                wingetId: "Waterfox.Waterfox",
                installedProgramNames: ["Waterfox"],
                processNames: ["waterfox"],
                tags: ["Browser", "Privacy", "Internet"],
                sortOrder: 80),

            // ============================================================
            // Communication
            // ============================================================

            Winget(
                id: "discord",
                displayName: "Discord",
                description: "Voice, chat, and community app commonly used by gamers.",
                category: AppInstallCategory.Communication,
                wingetId: "Discord.Discord",
                installedProgramNames: ["Discord"],
                processNames: ["Discord"],
                tags: ["Gaming", "Voice", "Chat", "Communication"],
                sortOrder: 100),

            Winget(
                id: "slack",
                displayName: "Slack",
                description: "Team communication and workspace messaging app.",
                category: AppInstallCategory.Communication,
                wingetId: "SlackTechnologies.Slack",
                installedProgramNames: ["Slack"],
                processNames: ["slack"],
                tags: ["Chat", "Work", "Communication"],
                sortOrder: 110),

            Winget(
                id: "telegram",
                displayName: "Telegram Desktop",
                description: "Fast desktop messaging app.",
                category: AppInstallCategory.Communication,
                wingetId: "Telegram.TelegramDesktop",
                installedProgramNames: ["Telegram Desktop"],
                processNames: ["Telegram"],
                tags: ["Chat", "Messaging", "Communication"],
                sortOrder: 120),

            Winget(
                id: "signal",
                displayName: "Signal",
                description: "Private messaging app with encrypted communication.",
                category: AppInstallCategory.Communication,
                wingetId: "OpenWhisperSystems.Signal",
                installedProgramNames: ["Signal"],
                processNames: ["Signal"],
                tags: ["Chat", "Messaging", "Privacy"],
                sortOrder: 130),

            Winget(
                id: "whatsapp",
                displayName: "WhatsApp",
                description: "Desktop client for WhatsApp messaging.",
                category: AppInstallCategory.Communication,
                wingetId: "WhatsApp.WhatsApp",
                installedProgramNames: ["WhatsApp"],
                processNames: ["WhatsApp"],
                tags: ["Chat", "Messaging", "Communication"],
                sortOrder: 140),

            Winget(
                id: "zoom",
                displayName: "Zoom",
                description: "Video meetings and conferencing app.",
                category: AppInstallCategory.Communication,
                wingetId: "Zoom.Zoom",
                installedProgramNames: ["Zoom"],
                processNames: ["Zoom"],
                tags: ["Video", "Meetings", "Communication"],
                sortOrder: 150),

            Winget(
                id: "microsoft-teams",
                displayName: "Microsoft Teams",
                description: "Microsoft collaboration, meeting, and chat app.",
                category: AppInstallCategory.Communication,
                wingetId: "Microsoft.Teams",
                installedProgramNames: ["Microsoft Teams"],
                processNames: ["Teams", "ms-teams"],
                tags: ["Microsoft", "Meetings", "Communication"],
                sortOrder: 160),

            Winget(
                id: "skype",
                displayName: "Skype",
                description: "Voice, video, and messaging app.",
                category: AppInstallCategory.Communication,
                wingetId: "Microsoft.Skype",
                installedProgramNames: ["Skype"],
                processNames: ["Skype"],
                tags: ["Voice", "Video", "Communication"],
                sortOrder: 170),

            Winget(
                id: "teamspeak",
                displayName: "TeamSpeak",
                description: "Voice chat client often used for gaming communities.",
                category: AppInstallCategory.Communication,
                wingetId: "TeamSpeakSystems.TeamSpeakClient",
                installedProgramNames: ["TeamSpeak"],
                processNames: ["ts3client_win64", "TeamSpeak"],
                tags: ["Gaming", "Voice", "Communication"],
                sortOrder: 180),

            Winget(
                id: "element",
                displayName: "Element",
                description: "Matrix-based secure messaging client.",
                category: AppInstallCategory.Communication,
                wingetId: "Element.Element",
                installedProgramNames: ["Element"],
                processNames: ["Element"],
                tags: ["Chat", "Matrix", "Communication"],
                sortOrder: 190),

            // ============================================================
            // Gaming and Launchers
            // ============================================================

            Winget(
                id: "steam",
                displayName: "Steam",
                description: "PC game store, launcher, and game library.",
                category: AppInstallCategory.Launcher,
                wingetId: "Valve.Steam",
                installedProgramNames: ["Steam"],
                processNames: ["steam"],
                tags: ["Gaming", "Launcher", "Games"],
                sortOrder: 200),

            Winget(
                id: "epic-games-launcher",
                displayName: "Epic Games Launcher",
                description: "Game launcher and store for Epic Games titles.",
                category: AppInstallCategory.Launcher,
                wingetId: "EpicGames.EpicGamesLauncher",
                installedProgramNames: ["Epic Games Launcher"],
                processNames: ["EpicGamesLauncher"],
                tags: ["Gaming", "Launcher", "Games"],
                sortOrder: 210),

            Winget(
                id: "gog-galaxy",
                displayName: "GOG Galaxy",
                description: "Game launcher and library manager for GOG games.",
                category: AppInstallCategory.Launcher,
                wingetId: "GOG.Galaxy",
                installedProgramNames: ["GOG GALAXY"],
                processNames: ["GalaxyClient"],
                tags: ["Gaming", "Launcher", "Games"],
                sortOrder: 220),

            Winget(
                id: "ea-app",
                displayName: "EA App",
                description: "Electronic Arts game launcher and library app.",
                category: AppInstallCategory.Launcher,
                wingetId: "ElectronicArts.EADesktop",
                installedProgramNames: ["EA app", "EA Desktop"],
                processNames: ["EADesktop", "EALauncher"],
                tags: ["Gaming", "Launcher", "Games"],
                sortOrder: 230),

            Winget(
                id: "ubisoft-connect",
                displayName: "Ubisoft Connect",
                description: "Ubisoft game launcher, store, and account client.",
                category: AppInstallCategory.Launcher,
                wingetId: "Ubisoft.Connect",
                installedProgramNames: ["Ubisoft Connect"],
                processNames: ["upc", "UbisoftConnect"],
                tags: ["Gaming", "Launcher", "Games"],
                sortOrder: 240),

            Winget(
                id: "battle-net",
                displayName: "Battle.net",
                description: "Blizzard game launcher for Battle.net games.",
                category: AppInstallCategory.Launcher,
                wingetId: "Blizzard.BattleNet",
                installedProgramNames: ["Battle.net"],
                processNames: ["Battle.net"],
                tags: ["Gaming", "Launcher", "Games"],
                sortOrder: 250),

            Winget(
                id: "heroic-games-launcher",
                displayName: "Heroic Games Launcher",
                description: "Open-source launcher for Epic, GOG, and Amazon game libraries.",
                category: AppInstallCategory.Launcher,
                wingetId: "HeroicGamesLauncher.HeroicGamesLauncher",
                installedProgramNames: ["Heroic"],
                processNames: ["Heroic"],
                tags: ["Gaming", "Launcher", "Open Source"],
                sortOrder: 260),

            Winget(
                id: "itch",
                displayName: "itch",
                description: "Game launcher and store for indie games.",
                category: AppInstallCategory.Launcher,
                wingetId: "itch.itch",
                installedProgramNames: ["itch"],
                processNames: ["itch"],
                tags: ["Gaming", "Launcher", "Indie"],
                sortOrder: 270),

            Winget(
                id: "prism-launcher",
                displayName: "Prism Launcher",
                description: "Open-source launcher for managing Minecraft instances.",
                category: AppInstallCategory.Gaming,
                wingetId: "PrismLauncher.PrismLauncher",
                installedProgramNames: ["Prism Launcher"],
                processNames: ["prismlauncher"],
                tags: ["Gaming", "Minecraft", "Launcher"],
                sortOrder: 280),

            Winget(
                id: "modrinth-app",
                displayName: "Modrinth App",
                description: "Mod manager and launcher for Minecraft mods and modpacks.",
                category: AppInstallCategory.Gaming,
                wingetId: "Modrinth.ModrinthApp",
                installedProgramNames: ["Modrinth App"],
                processNames: ["Modrinth App"],
                tags: ["Gaming", "Minecraft", "Mods"],
                sortOrder: 290),

            Winget(
                id: "parsec",
                displayName: "Parsec",
                description: "Low-latency remote desktop and game streaming client.",
                category: AppInstallCategory.Gaming,
                wingetId: "Parsec.Parsec",
                installedProgramNames: ["Parsec"],
                processNames: ["parsecd", "parsec"],
                tags: ["Gaming", "Remote Play", "Streaming"],
                sortOrder: 300),

            Winget(
                id: "moonlight",
                displayName: "Moonlight",
                description: "Game streaming client for local and remote game streaming.",
                category: AppInstallCategory.Gaming,
                wingetId: "MoonlightGameStreamingProject.Moonlight",
                installedProgramNames: ["Moonlight"],
                processNames: ["Moonlight"],
                tags: ["Gaming", "Streaming", "Remote Play"],
                sortOrder: 310),

            // ============================================================
            // Utilities
            // ============================================================

            Winget(
                id: "7zip",
                displayName: "7-Zip",
                description: "Free file archiver for ZIP, 7z, and other archive formats.",
                category: AppInstallCategory.Utility,
                wingetId: "7zip.7zip",
                installedProgramNames: ["7-Zip"],
                processNames: ["7zFM", "7zG"],
                tags: ["Archive", "Compression", "Utility"],
                sortOrder: 400),

            Winget(
                id: "winrar",
                displayName: "WinRAR",
                description: "Archive manager for RAR, ZIP, and compressed files.",
                category: AppInstallCategory.Utility,
                wingetId: "RARLab.WinRAR",
                installedProgramNames: ["WinRAR"],
                processNames: ["WinRAR"],
                tags: ["Archive", "Compression", "Utility"],
                sortOrder: 410),

            Winget(
                id: "notepad-plus-plus",
                displayName: "Notepad++",
                description: "Lightweight text and source code editor.",
                category: AppInstallCategory.Utility,
                wingetId: "Notepad++.Notepad++",
                installedProgramNames: ["Notepad++"],
                processNames: ["notepad++"],
                tags: ["Text Editor", "Utility", "Code"],
                sortOrder: 420),

            Winget(
                id: "everything",
                displayName: "Everything",
                description: "Fast file and folder search tool for Windows.",
                category: AppInstallCategory.Utility,
                wingetId: "voidtools.Everything",
                installedProgramNames: ["Everything"],
                processNames: ["Everything"],
                tags: ["Search", "Files", "Utility"],
                sortOrder: 430),

            Winget(
                id: "powertoys",
                displayName: "Microsoft PowerToys",
                description: "Microsoft utility suite for productivity and power-user tools.",
                category: AppInstallCategory.Utility,
                wingetId: "Microsoft.PowerToys",
                installedProgramNames: ["Microsoft PowerToys"],
                processNames: ["PowerToys"],
                tags: ["Microsoft", "Utility", "Productivity"],
                sortOrder: 440),

            Winget(
                id: "sharex",
                displayName: "ShareX",
                description: "Screenshot, screen recording, and sharing utility.",
                category: AppInstallCategory.Utility,
                wingetId: "ShareX.ShareX",
                installedProgramNames: ["ShareX"],
                processNames: ["ShareX"],
                tags: ["Screenshot", "Screen Capture", "Utility"],
                sortOrder: 450),

            Winget(
                id: "greenshot",
                displayName: "Greenshot",
                description: "Lightweight screenshot tool.",
                category: AppInstallCategory.Utility,
                wingetId: "Greenshot.Greenshot",
                installedProgramNames: ["Greenshot"],
                processNames: ["Greenshot"],
                tags: ["Screenshot", "Screen Capture", "Utility"],
                sortOrder: 460),

            Winget(
                id: "rufus",
                displayName: "Rufus",
                description: "Utility for creating bootable USB drives.",
                category: AppInstallCategory.Utility,
                wingetId: "Rufus.Rufus",
                installedProgramNames: ["Rufus"],
                processNames: ["rufus"],
                tags: ["USB", "Bootable", "Utility"],
                sortOrder: 470),

            Winget(
                id: "ventoy",
                displayName: "Ventoy",
                description: "Tool for creating multi-boot USB drives.",
                category: AppInstallCategory.Utility,
                wingetId: "Ventoy.Ventoy",
                installedProgramNames: ["Ventoy"],
                processNames: ["Ventoy2Disk"],
                tags: ["USB", "Bootable", "Utility"],
                sortOrder: 480),

            Winget(
                id: "windirstat",
                displayName: "WinDirStat",
                description: "Disk usage viewer and cleanup helper.",
                category: AppInstallCategory.Utility,
                wingetId: "WinDirStat.WinDirStat",
                installedProgramNames: ["WinDirStat"],
                processNames: ["windirstat"],
                tags: ["Storage", "Disk", "Utility"],
                sortOrder: 490),

            Winget(
                id: "bleachbit",
                displayName: "BleachBit",
                description: "System cleaning utility for temporary files and caches.",
                category: AppInstallCategory.Utility,
                wingetId: "BleachBit.BleachBit",
                installedProgramNames: ["BleachBit"],
                processNames: ["bleachbit"],
                tags: ["Cleaner", "Storage", "Utility"],
                sortOrder: 500),

            Winget(
                id: "winscp",
                displayName: "WinSCP",
                description: "SFTP, FTP, WebDAV, and SCP client for Windows.",
                category: AppInstallCategory.Utility,
                wingetId: "WinSCP.WinSCP",
                installedProgramNames: ["WinSCP"],
                processNames: ["WinSCP"],
                tags: ["FTP", "SFTP", "Utility"],
                sortOrder: 510),

            Winget(
                id: "putty",
                displayName: "PuTTY",
                description: "SSH and Telnet client for Windows.",
                category: AppInstallCategory.Utility,
                wingetId: "PuTTY.PuTTY",
                installedProgramNames: ["PuTTY"],
                processNames: ["putty"],
                tags: ["SSH", "Terminal", "Utility"],
                sortOrder: 520),

            Winget(
                id: "filezilla",
                displayName: "FileZilla",
                description: "FTP, FTPS, and SFTP client.",
                category: AppInstallCategory.Utility,
                wingetId: "FileZilla.FileZilla",
                installedProgramNames: ["FileZilla"],
                processNames: ["filezilla"],
                tags: ["FTP", "SFTP", "Utility"],
                sortOrder: 530),

            Winget(
                id: "bitwarden",
                displayName: "Bitwarden",
                description: "Password manager desktop app.",
                category: AppInstallCategory.Utility,
                wingetId: "Bitwarden.Bitwarden",
                installedProgramNames: ["Bitwarden"],
                processNames: ["Bitwarden"],
                tags: ["Password Manager", "Security", "Utility"],
                sortOrder: 540),

            Winget(
                id: "keepassxc",
                displayName: "KeePassXC",
                description: "Open-source offline password manager.",
                category: AppInstallCategory.Utility,
                wingetId: "KeePassXCTeam.KeePassXC",
                installedProgramNames: ["KeePassXC"],
                processNames: ["KeePassXC"],
                tags: ["Password Manager", "Security", "Utility"],
                sortOrder: 550),

            Winget(
                id: "malwarebytes",
                displayName: "Malwarebytes",
                description: "Anti-malware and security scanning utility.",
                category: AppInstallCategory.Utility,
                wingetId: "Malwarebytes.Malwarebytes",
                installedProgramNames: ["Malwarebytes"],
                processNames: ["Malwarebytes", "mbam"],
                tags: ["Security", "Anti Malware", "Utility"],
                sortOrder: 560),

            Winget(
                id: "sumatra-pdf",
                displayName: "SumatraPDF",
                description: "Lightweight PDF and document reader.",
                category: AppInstallCategory.Utility,
                wingetId: "SumatraPDF.SumatraPDF",
                installedProgramNames: ["SumatraPDF"],
                processNames: ["SumatraPDF"],
                tags: ["PDF", "Reader", "Utility"],
                sortOrder: 570),

            Winget(
                id: "adobe-reader",
                displayName: "Adobe Acrobat Reader",
                description: "PDF reader from Adobe.",
                category: AppInstallCategory.Utility,
                wingetId: "Adobe.Acrobat.Reader.64-bit",
                installedProgramNames: ["Adobe Acrobat", "Adobe Acrobat Reader"],
                processNames: ["AcroRd32", "Acrobat"],
                tags: ["PDF", "Reader", "Utility"],
                sortOrder: 580),

            Winget(
                id: "localsend",
                displayName: "LocalSend",
                description: "Local network file sharing app.",
                category: AppInstallCategory.Utility,
                wingetId: "LocalSend.LocalSend",
                installedProgramNames: ["LocalSend"],
                processNames: ["localsend_app"],
                tags: ["Files", "Sharing", "Utility"],
                sortOrder: 590),

            // ============================================================
            // Media
            // ============================================================

            Winget(
                id: "vlc",
                displayName: "VLC Media Player",
                description: "Popular media player supporting many audio and video formats.",
                category: AppInstallCategory.Media,
                wingetId: "VideoLAN.VLC",
                installedProgramNames: ["VLC media player"],
                processNames: ["vlc"],
                tags: ["Video", "Audio", "Media"],
                sortOrder: 600),

            Winget(
                id: "spotify",
                displayName: "Spotify",
                description: "Music streaming desktop app.",
                category: AppInstallCategory.Media,
                wingetId: "Spotify.Spotify",
                installedProgramNames: ["Spotify"],
                processNames: ["Spotify"],
                tags: ["Music", "Audio", "Media"],
                sortOrder: 610),

            Winget(
                id: "audacity",
                displayName: "Audacity",
                description: "Audio recording and editing app.",
                category: AppInstallCategory.Media,
                wingetId: "Audacity.Audacity",
                installedProgramNames: ["Audacity"],
                processNames: ["audacity"],
                tags: ["Audio", "Editor", "Media"],
                sortOrder: 620),

            Winget(
                id: "handbrake",
                displayName: "HandBrake",
                description: "Video transcoder for converting video files.",
                category: AppInstallCategory.Media,
                wingetId: "HandBrake.HandBrake",
                installedProgramNames: ["HandBrake"],
                processNames: ["HandBrake"],
                tags: ["Video", "Converter", "Media"],
                sortOrder: 630),

            Winget(
                id: "kodi",
                displayName: "Kodi",
                description: "Media centre for organising and playing media.",
                category: AppInstallCategory.Media,
                wingetId: "XBMCFoundation.Kodi",
                installedProgramNames: ["Kodi"],
                processNames: ["kodi"],
                tags: ["Media Centre", "Video", "Media"],
                sortOrder: 640),

            Winget(
                id: "jellyfin-media-player",
                displayName: "Jellyfin Media Player",
                description: "Desktop media player for Jellyfin servers.",
                category: AppInstallCategory.Media,
                wingetId: "Jellyfin.JellyfinMediaPlayer",
                installedProgramNames: ["Jellyfin Media Player"],
                processNames: ["JellyfinMediaPlayer"],
                tags: ["Media", "Video", "Streaming"],
                sortOrder: 650),

            Winget(
                id: "plex",
                displayName: "Plex",
                description: "Media playback and library app for Plex users.",
                category: AppInstallCategory.Media,
                wingetId: "Plex.Plex",
                installedProgramNames: ["Plex"],
                processNames: ["Plex"],
                tags: ["Media", "Video", "Streaming"],
                sortOrder: 660),

            Winget(
                id: "gimp",
                displayName: "GIMP",
                description: "Open-source image editing app.",
                category: AppInstallCategory.Media,
                wingetId: "GIMP.GIMP",
                installedProgramNames: ["GIMP"],
                processNames: ["gimp"],
                tags: ["Image", "Editor", "Media"],
                sortOrder: 670),

            Winget(
                id: "paintdotnet",
                displayName: "paint.net",
                description: "Image editing app for Windows.",
                category: AppInstallCategory.Media,
                wingetId: "dotPDN.PaintDotNet",
                installedProgramNames: ["paint.net"],
                processNames: ["paintdotnet"],
                tags: ["Image", "Editor", "Media"],
                sortOrder: 680),

            Winget(
                id: "inkscape",
                displayName: "Inkscape",
                description: "Vector graphics editor.",
                category: AppInstallCategory.Media,
                wingetId: "Inkscape.Inkscape",
                installedProgramNames: ["Inkscape"],
                processNames: ["inkscape"],
                tags: ["Vector", "Image", "Media"],
                sortOrder: 690),

            Winget(
                id: "blender",
                displayName: "Blender",
                description: "3D modelling, animation, and rendering app.",
                category: AppInstallCategory.Media,
                wingetId: "BlenderFoundation.Blender",
                installedProgramNames: ["Blender"],
                processNames: ["blender"],
                tags: ["3D", "Rendering", "Media"],
                sortOrder: 700),

            Winget(
                id: "shotcut",
                displayName: "Shotcut",
                description: "Open-source video editor.",
                category: AppInstallCategory.Media,
                wingetId: "Meltytech.Shotcut",
                installedProgramNames: ["Shotcut"],
                processNames: ["shotcut"],
                tags: ["Video", "Editor", "Media"],
                sortOrder: 710),

            Winget(
                id: "kdenlive",
                displayName: "Kdenlive",
                description: "Open-source video editing app.",
                category: AppInstallCategory.Media,
                wingetId: "KDE.Kdenlive",
                installedProgramNames: ["Kdenlive"],
                processNames: ["kdenlive"],
                tags: ["Video", "Editor", "Media"],
                sortOrder: 720),

            Winget(
                id: "imageglass",
                displayName: "ImageGlass",
                description: "Lightweight image viewer.",
                category: AppInstallCategory.Media,
                wingetId: "DuongDieuPhap.ImageGlass",
                installedProgramNames: ["ImageGlass"],
                processNames: ["ImageGlass"],
                tags: ["Image", "Viewer", "Media"],
                sortOrder: 730),

            // ============================================================
            // Streaming
            // ============================================================

            Winget(
                id: "obs-studio",
                displayName: "OBS Studio",
                description: "Recording and live streaming software.",
                category: AppInstallCategory.Streaming,
                wingetId: "OBSProject.OBSStudio",
                installedProgramNames: ["OBS Studio"],
                processNames: ["obs64", "obs32"],
                tags: ["Streaming", "Recording", "Video"],
                sortOrder: 800),

            Winget(
                id: "streamlabs",
                displayName: "Streamlabs Desktop",
                description: "Streaming and recording app for creators.",
                category: AppInstallCategory.Streaming,
                wingetId: "Streamlabs.Streamlabs",
                installedProgramNames: ["Streamlabs Desktop"],
                processNames: ["Streamlabs Desktop"],
                tags: ["Streaming", "Recording", "Video"],
                sortOrder: 810),

            Winget(
                id: "elgato-stream-deck",
                displayName: "Elgato Stream Deck",
                description: "Control software for Elgato Stream Deck devices.",
                category: AppInstallCategory.Streaming,
                wingetId: "Elgato.StreamDeck",
                installedProgramNames: ["Elgato Stream Deck", "Stream Deck"],
                processNames: ["StreamDeck"],
                tags: ["Streaming", "Elgato", "Hardware"],
                sortOrder: 820),

            Winget(
                id: "voicemod",
                displayName: "Voicemod",
                description: "Voice changer and soundboard app.",
                category: AppInstallCategory.Streaming,
                wingetId: "Voicemod.Voicemod",
                installedProgramNames: ["Voicemod"],
                processNames: ["VoicemodDesktop"],
                tags: ["Streaming", "Audio", "Voice"],
                sortOrder: 830),

            Winget(
                id: "voicemeeter",
                displayName: "Voicemeeter",
                description: "Virtual audio mixer for routing and mixing audio.",
                category: AppInstallCategory.Streaming,
                wingetId: "VB-Audio.Voicemeeter",
                installedProgramNames: ["Voicemeeter"],
                processNames: ["voicemeeter", "voicemeeterpro"],
                tags: ["Audio", "Streaming", "Mixer"],
                sortOrder: 840),

            // ============================================================
            // Development
            // ============================================================

            Winget(
                id: "visual-studio-code",
                displayName: "Visual Studio Code",
                description: "Lightweight code editor from Microsoft.",
                category: AppInstallCategory.Development,
                wingetId: "Microsoft.VisualStudioCode",
                installedProgramNames: ["Microsoft Visual Studio Code"],
                processNames: ["Code"],
                tags: ["Code", "Editor", "Development"],
                sortOrder: 900),

            Winget(
                id: "visual-studio-community-2022",
                displayName: "Visual Studio Community 2022",
                description: "Full IDE for .NET, C++, desktop, and game development.",
                category: AppInstallCategory.Development,
                wingetId: "Microsoft.VisualStudio.2022.Community",
                installedProgramNames: ["Visual Studio Community 2022"],
                processNames: ["devenv"],
                tags: ["IDE", ".NET", "Development"],
                sortOrder: 910),

            Winget(
                id: "git",
                displayName: "Git",
                description: "Distributed version control system.",
                category: AppInstallCategory.Development,
                wingetId: "Git.Git",
                installedProgramNames: ["Git"],
                processNames: ["git"],
                tags: ["Git", "Version Control", "Development"],
                sortOrder: 920),

            Winget(
                id: "github-desktop",
                displayName: "GitHub Desktop",
                description: "Desktop Git client for GitHub repositories.",
                category: AppInstallCategory.Development,
                wingetId: "GitHub.GitHubDesktop",
                installedProgramNames: ["GitHub Desktop"],
                processNames: ["GitHubDesktop"],
                tags: ["Git", "GitHub", "Development"],
                sortOrder: 930),

            Winget(
                id: "windows-terminal",
                displayName: "Windows Terminal",
                description: "Modern terminal app from Microsoft.",
                category: AppInstallCategory.Development,
                wingetId: "Microsoft.WindowsTerminal",
                installedProgramNames: ["Windows Terminal"],
                processNames: ["WindowsTerminal"],
                tags: ["Terminal", "Console", "Development"],
                sortOrder: 940),

            Winget(
                id: "powershell",
                displayName: "PowerShell",
                description: "Modern cross-platform PowerShell shell.",
                category: AppInstallCategory.Development,
                wingetId: "Microsoft.PowerShell",
                installedProgramNames: ["PowerShell"],
                processNames: ["pwsh"],
                tags: ["Terminal", "Shell", "Development"],
                sortOrder: 950),

            Winget(
                id: "nodejs-lts",
                displayName: "Node.js LTS",
                description: "Long-term support release of Node.js JavaScript runtime.",
                category: AppInstallCategory.Development,
                wingetId: "OpenJS.NodeJS.LTS",
                installedProgramNames: ["Node.js"],
                processNames: ["node"],
                tags: ["Node.js", "JavaScript", "Development"],
                sortOrder: 960),

            Winget(
                id: "python-313",
                displayName: "Python 3.13",
                description: "Python programming language runtime.",
                category: AppInstallCategory.Development,
                wingetId: "Python.Python.3.13",
                installedProgramNames: ["Python 3.13"],
                processNames: ["python"],
                tags: ["Python", "Programming", "Development"],
                sortOrder: 970),

            Winget(
                id: "dotnet-sdk-9",
                displayName: ".NET SDK 9",
                description: "Microsoft .NET SDK for building .NET applications.",
                category: AppInstallCategory.Development,
                wingetId: "Microsoft.DotNet.SDK.9",
                installedProgramNames: ["Microsoft .NET SDK 9"],
                processNames: ["dotnet"],
                tags: [".NET", "SDK", "Development"],
                sortOrder: 980),

            Winget(
                id: "docker-desktop",
                displayName: "Docker Desktop",
                description: "Container development environment for Windows.",
                category: AppInstallCategory.Development,
                wingetId: "Docker.DockerDesktop",
                installedProgramNames: ["Docker Desktop"],
                processNames: ["Docker Desktop"],
                tags: ["Docker", "Containers", "Development"],
                sortOrder: 990,
                requiresAdmin: true,
                requiresRestart: true),

            Winget(
                id: "postman",
                displayName: "Postman",
                description: "API development and testing app.",
                category: AppInstallCategory.Development,
                wingetId: "Postman.Postman",
                installedProgramNames: ["Postman"],
                processNames: ["Postman"],
                tags: ["API", "Testing", "Development"],
                sortOrder: 1000),

            Winget(
                id: "jetbrains-toolbox",
                displayName: "JetBrains Toolbox",
                description: "Manager for JetBrains IDEs and developer tools.",
                category: AppInstallCategory.Development,
                wingetId: "JetBrains.Toolbox",
                installedProgramNames: ["JetBrains Toolbox"],
                processNames: ["jetbrains-toolbox"],
                tags: ["IDE", "JetBrains", "Development"],
                sortOrder: 1010),

            Winget(
                id: "dbeaver",
                displayName: "DBeaver Community",
                description: "Database management tool for developers.",
                category: AppInstallCategory.Development,
                wingetId: "DBeaver.DBeaver.Community",
                installedProgramNames: ["DBeaver"],
                processNames: ["dbeaver"],
                tags: ["Database", "SQL", "Development"],
                sortOrder: 1020),

            Winget(
                id: "db-browser-sqlite",
                displayName: "DB Browser for SQLite",
                description: "SQLite database browser and editor.",
                category: AppInstallCategory.Development,
                wingetId: "DBBrowserForSQLite.DBBrowserForSQLite",
                installedProgramNames: ["DB Browser for SQLite"],
                processNames: ["DB Browser for SQLite"],
                tags: ["SQLite", "Database", "Development"],
                sortOrder: 1030),

            Winget(
                id: "winmerge",
                displayName: "WinMerge",
                description: "File and folder comparison tool.",
                category: AppInstallCategory.Development,
                wingetId: "WinMerge.WinMerge",
                installedProgramNames: ["WinMerge"],
                processNames: ["WinMergeU"],
                tags: ["Diff", "Compare", "Development"],
                sortOrder: 1040),

            Winget(
                id: "fork",
                displayName: "Fork",
                description: "Fast Git client for Windows.",
                category: AppInstallCategory.Development,
                wingetId: "Fork.Fork",
                installedProgramNames: ["Fork"],
                processNames: ["Fork"],
                tags: ["Git", "Version Control", "Development"],
                sortOrder: 1050),

            Winget(
                id: "git-extensions",
                displayName: "Git Extensions",
                description: "Graphical Git client and repository browser.",
                category: AppInstallCategory.Development,
                wingetId: "GitExtensionsTeam.GitExtensions",
                installedProgramNames: ["Git Extensions"],
                processNames: ["GitExtensions"],
                tags: ["Git", "Version Control", "Development"],
                sortOrder: 1060),

            Winget(
                id: "tortoisegit",
                displayName: "TortoiseGit",
                description: "Windows shell interface for Git.",
                category: AppInstallCategory.Development,
                wingetId: "TortoiseGit.TortoiseGit",
                installedProgramNames: ["TortoiseGit"],
                processNames: ["TortoiseGitProc"],
                tags: ["Git", "Shell", "Development"],
                sortOrder: 1070,
                requiresRestart: true),

            Winget(
                id: "tortoisesvn",
                displayName: "TortoiseSVN",
                description: "Windows shell interface for Subversion.",
                category: AppInstallCategory.Development,
                wingetId: "TortoiseSVN.TortoiseSVN",
                installedProgramNames: ["TortoiseSVN"],
                processNames: ["TortoiseProc"],
                tags: ["SVN", "Shell", "Development"],
                sortOrder: 1080,
                requiresRestart: true),

            // ============================================================
            // Hardware
            // ============================================================

            Winget(
                id: "hwinfo",
                displayName: "HWiNFO",
                description: "Hardware monitoring and system information tool.",
                category: AppInstallCategory.Hardware,
                wingetId: "REALiX.HWiNFO",
                installedProgramNames: ["HWiNFO"],
                processNames: ["HWiNFO64", "HWiNFO32"],
                tags: ["Hardware", "Monitoring", "Sensors"],
                sortOrder: 1200),

            Winget(
                id: "cpu-z",
                displayName: "CPU-Z",
                description: "CPU, motherboard, RAM, and system information tool.",
                category: AppInstallCategory.Hardware,
                wingetId: "CPUID.CPU-Z",
                installedProgramNames: ["CPUID CPU-Z", "CPU-Z"],
                processNames: ["cpuz"],
                tags: ["Hardware", "CPU", "System Info"],
                sortOrder: 1210),

            Winget(
                id: "gpu-z",
                displayName: "GPU-Z",
                description: "Graphics card information and monitoring tool.",
                category: AppInstallCategory.Hardware,
                wingetId: "TechPowerUp.GPU-Z",
                installedProgramNames: ["GPU-Z"],
                processNames: ["GPU-Z"],
                tags: ["Hardware", "GPU", "System Info"],
                sortOrder: 1220),

            Winget(
                id: "crystaldiskinfo",
                displayName: "CrystalDiskInfo",
                description: "Drive health and SMART monitoring tool.",
                category: AppInstallCategory.Hardware,
                wingetId: "CrystalDewWorld.CrystalDiskInfo",
                installedProgramNames: ["CrystalDiskInfo"],
                processNames: ["DiskInfo64", "DiskInfo32"],
                tags: ["Hardware", "Storage", "Disk"],
                sortOrder: 1230),

            Winget(
                id: "crystaldiskmark",
                displayName: "CrystalDiskMark",
                description: "Storage benchmark tool.",
                category: AppInstallCategory.Hardware,
                wingetId: "CrystalDewWorld.CrystalDiskMark",
                installedProgramNames: ["CrystalDiskMark"],
                processNames: ["DiskMark64", "DiskMark32"],
                tags: ["Hardware", "Storage", "Benchmark"],
                sortOrder: 1240),

            Winget(
                id: "logitech-ghub",
                displayName: "Logitech G HUB",
                description: "Configuration software for Logitech G devices.",
                category: AppInstallCategory.Hardware,
                wingetId: "Logitech.GHUB",
                installedProgramNames: ["Logitech G HUB"],
                processNames: ["lghub", "lghub_agent"],
                tags: ["Hardware", "Logitech", "Gaming"],
                sortOrder: 1250),

            Winget(
                id: "corsair-icue",
                displayName: "Corsair iCUE",
                description: "Configuration software for Corsair devices and lighting.",
                category: AppInstallCategory.Hardware,
                wingetId: "Corsair.iCUE.5",
                installedProgramNames: ["CORSAIR iCUE"],
                processNames: ["iCUE"],
                tags: ["Hardware", "Corsair", "RGB"],
                sortOrder: 1260),

            Winget(
                id: "razer-synapse",
                displayName: "Razer Synapse",
                description: "Configuration software for Razer devices.",
                category: AppInstallCategory.Hardware,
                wingetId: "Razer.Synapse.3",
                installedProgramNames: ["Razer Synapse"],
                processNames: ["Razer Synapse"],
                tags: ["Hardware", "Razer", "Gaming"],
                sortOrder: 1270),

            Winget(
                id: "intel-driver-support-assistant",
                displayName: "Intel Driver & Support Assistant",
                description: "Intel utility for checking supported Intel drivers and updates.",
                category: AppInstallCategory.Hardware,
                wingetId: "Intel.IntelDriverAndSupportAssistant",
                installedProgramNames: ["Intel Driver & Support Assistant"],
                processNames: ["DSAService", "IntelDSA"],
                tags: ["Hardware", "Intel", "Drivers"],
                sortOrder: 1280,
                requiresAdmin: true),

            Winget(
                id: "nvidia-geforce-experience",
                displayName: "NVIDIA GeForce Experience",
                description: "NVIDIA companion app for supported GeForce GPUs.",
                category: AppInstallCategory.Hardware,
                wingetId: "Nvidia.GeForceExperience",
                installedProgramNames: ["NVIDIA GeForce Experience"],
                processNames: ["NVIDIA GeForce Experience"],
                tags: ["Hardware", "NVIDIA", "GPU"],
                sortOrder: 1290,
                requiresAdmin: true),

            Winget(
                id: "amd-ryzen-master",
                displayName: "AMD Ryzen Master",
                description: "AMD utility for supported Ryzen CPUs.",
                category: AppInstallCategory.Hardware,
                wingetId: "AMD.RyzenMaster",
                installedProgramNames: ["AMD Ryzen Master"],
                processNames: ["AMDRyzenMaster"],
                tags: ["Hardware", "AMD", "CPU"],
                sortOrder: 1300,
                requiresAdmin: true,
                requiresRestart: true),

            Winget(
                id: "msi-afterburner",
                displayName: "MSI Afterburner",
                description: "GPU monitoring and tuning utility.",
                category: AppInstallCategory.Hardware,
                wingetId: "Guru3D.Afterburner",
                installedProgramNames: ["MSI Afterburner"],
                processNames: ["MSIAfterburner"],
                tags: ["Hardware", "GPU", "Monitoring"],
                sortOrder: 1310,
                requiresAdmin: true),

            // ============================================================
            // Productivity
            // ============================================================

            Winget(
                id: "notion",
                displayName: "Notion",
                description: "Notes, documents, workspace, and productivity app.",
                category: AppInstallCategory.Productivity,
                wingetId: "Notion.Notion",
                installedProgramNames: ["Notion"],
                processNames: ["Notion"],
                tags: ["Notes", "Productivity", "Workspace"],
                sortOrder: 1400),

            Winget(
                id: "obsidian",
                displayName: "Obsidian",
                description: "Markdown-based notes and knowledge management app.",
                category: AppInstallCategory.Productivity,
                wingetId: "Obsidian.Obsidian",
                installedProgramNames: ["Obsidian"],
                processNames: ["Obsidian"],
                tags: ["Notes", "Markdown", "Productivity"],
                sortOrder: 1410),

            Winget(
                id: "joplin",
                displayName: "Joplin",
                description: "Open-source notes and to-do app.",
                category: AppInstallCategory.Productivity,
                wingetId: "Joplin.Joplin",
                installedProgramNames: ["Joplin"],
                processNames: ["Joplin"],
                tags: ["Notes", "Tasks", "Productivity"],
                sortOrder: 1420),

            Winget(
                id: "libreoffice",
                displayName: "LibreOffice",
                description: "Open-source office suite for documents, spreadsheets, and presentations.",
                category: AppInstallCategory.Productivity,
                wingetId: "TheDocumentFoundation.LibreOffice",
                installedProgramNames: ["LibreOffice"],
                processNames: ["soffice"],
                tags: ["Office", "Documents", "Productivity"],
                sortOrder: 1430),

            Winget(
                id: "onlyoffice",
                displayName: "ONLYOFFICE Desktop Editors",
                description: "Office suite for documents, spreadsheets, and presentations.",
                category: AppInstallCategory.Productivity,
                wingetId: "ONLYOFFICE.DesktopEditors",
                installedProgramNames: ["ONLYOFFICE Desktop Editors"],
                processNames: ["DesktopEditors"],
                tags: ["Office", "Documents", "Productivity"],
                sortOrder: 1440),

            Winget(
                id: "dropbox",
                displayName: "Dropbox",
                description: "Cloud file sync and storage app.",
                category: AppInstallCategory.Productivity,
                wingetId: "Dropbox.Dropbox",
                installedProgramNames: ["Dropbox"],
                processNames: ["Dropbox"],
                tags: ["Cloud", "Files", "Productivity"],
                sortOrder: 1450),

            Winget(
                id: "google-drive",
                displayName: "Google Drive",
                description: "Google Drive desktop sync app.",
                category: AppInstallCategory.Productivity,
                wingetId: "Google.GoogleDrive",
                installedProgramNames: ["Google Drive"],
                processNames: ["GoogleDriveFS"],
                tags: ["Cloud", "Files", "Productivity"],
                sortOrder: 1460),

            Winget(
                id: "microsoft-onedrive",
                displayName: "Microsoft OneDrive",
                description: "Microsoft cloud file sync app.",
                category: AppInstallCategory.Productivity,
                wingetId: "Microsoft.OneDrive",
                installedProgramNames: ["Microsoft OneDrive", "OneDrive"],
                processNames: ["OneDrive"],
                tags: ["Cloud", "Files", "Microsoft"],
                sortOrder: 1470),

            Winget(
                id: "todoist",
                displayName: "Todoist",
                description: "Task manager and productivity app.",
                category: AppInstallCategory.Productivity,
                wingetId: "Doist.Todoist",
                installedProgramNames: ["Todoist"],
                processNames: ["Todoist"],
                tags: ["Tasks", "Productivity", "To Do"],
                sortOrder: 1480),

            Winget(
                id: "evernote",
                displayName: "Evernote",
                description: "Notes and organisation app.",
                category: AppInstallCategory.Productivity,
                wingetId: "Evernote.Evernote",
                installedProgramNames: ["Evernote"],
                processNames: ["Evernote"],
                tags: ["Notes", "Productivity", "Organisation"],
                sortOrder: 1490),

            Winget(
                id: "drawio",
                displayName: "draw.io Desktop",
                description: "Diagramming app for flowcharts and technical diagrams.",
                category: AppInstallCategory.Productivity,
                wingetId: "JGraph.Draw",
                installedProgramNames: ["draw.io"],
                processNames: ["draw.io"],
                tags: ["Diagrams", "Productivity", "Design"],
                sortOrder: 1500),

            Winget(
                id: "microsoft-office",
                displayName: "Microsoft 365 Apps",
                description: "Microsoft Office desktop apps for Microsoft 365 users.",
                category: AppInstallCategory.Productivity,
                wingetId: "Microsoft.Office",
                installedProgramNames: ["Microsoft 365 Apps", "Microsoft Office"],
                processNames: ["WINWORD", "EXCEL", "POWERPNT"],
                tags: ["Office", "Microsoft", "Productivity"],
                sortOrder: 1510,
                requiresAdmin: true)
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
                AppInstallCategory.Gaming => PackIconKind.ControllerClassic,
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