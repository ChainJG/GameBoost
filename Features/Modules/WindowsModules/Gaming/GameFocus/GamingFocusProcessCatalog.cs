namespace GameBoost.Features.Modules.WindowsModules.Gaming.GameFocus
{
    public static class GamingFocusProcessCatalog
    {

        // True = GameBoost will first try to close the app normally
        // like pressing the X button on the app window
        // This is safer because it lets the app shut down cleanly
        //TryGracefulClose = true,

        // False = GameBoost is NOT allowed to force kill this process
        // This helps avoid crashes, broken sessions, or data loss
        // If Process does not close normally, GameBoost will skip it instead of killing it
        //AllowForceKill = false,

        // True = this process is included in the normal Game Focus scan automatically
        // If Process is running, GameBoost will detect it and try to close it safely
        //EnabledByDefault = true

        public static IReadOnlyList<GamingFocusProcessDefinition> Processes { get; } =
        [
            #region Communication / Voice Chat

            CloseByDefault(
                processName: "Discord",
                displayName: "Discord",
                reason: "Can use CPU, GPU acceleration, memory, voice, network, overlays, and notifications in the background"),

            KillHelperByDefault(
                processName: "Teams",
                displayName: "Microsoft Teams",
                reason: "Can use memory, CPU, network, and notifications in the background"),

            KillHelperByDefault(
                processName: "ms-teams",
                displayName: "Microsoft Teams",
                reason: "Can use memory, CPU, network, and notifications in the background"),

            KillHelperByDefault(
                processName: "Slack",
                displayName: "Slack",
                reason: "Can use memory, network activity, and desktop notifications while gaming"),

            KillHelperByDefault(
                processName: "Zoom",
                displayName: "Zoom",
                reason: "Can keep background meeting, update, audio, and notification components active"),

            KillHelperByDefault(
                processName: "Webex",
                displayName: "Cisco Webex",
                reason: "Can run background meeting and communication components"),

            KillHelperByDefault(
                processName: "Skype",
                displayName: "Skype",
                reason: "Can use memory, network, and notifications in the background"),

            KillHelperByDefault(
                processName: "Telegram",
                displayName: "Telegram",
                reason: "Can use network activity and notifications in the background"),

            KillHelperByDefault(
                processName: "WhatsApp",
                displayName: "WhatsApp",
                reason: "Can use memory, network activity, and notifications in the background"),

            CloseByDefault(
                processName: "Signal",
                displayName: "Signal",
                reason: "Can use network activity and notifications in the background"),

            #endregion

            #region Cloud Sync

            KillHelperByDefault(
                processName: "OneDrive",
                displayName: "OneDrive",
                reason: "Can sync files and use disk, CPU, and network activity during gameplay"),

            KillHelperByDefault(
                processName: "Dropbox",
                displayName: "Dropbox",
                reason: "Can sync files and use disk, CPU, and network activity during gameplay"),

            CloseByDefault(
                processName: "GoogleDriveFS",
                displayName: "Google Drive",
                reason: "Can sync files and use disk, CPU, and network activity during gameplay"),

            CloseByDefault(
                processName: "iCloudDrive",
                displayName: "iCloud Drive",
                reason: "Can sync files and photos in the background"),

            CloseByDefault(
                processName: "iCloudServices",
                displayName: "iCloud Services",
                reason: "Can keep Apple cloud sync services active in the background"),

            KillHelperByDefault(
                processName: "OneDriveStandaloneUpdater",
                displayName: "OneDrive Updater",
                reason: "Background updater helper that is safe to close before gaming"),

            #endregion

            #region Browser Crash / Update Helpers

            KillHelperByDefault(
                processName: "GoogleCrashHandler",
                displayName: "Google Crash Handler",
                reason: "Background crash-reporting helper that is not needed during gameplay"),

            KillHelperByDefault(
                processName: "GoogleCrashHandler64",
                displayName: "Google Crash Handler 64-bit",
                reason: "Background crash-reporting helper that is not needed during gameplay"),

            KillHelperByDefault(
                processName: "GoogleUpdate",
                displayName: "Google Update",
                reason: "Background update helper that can be closed before gaming"),

            KillHelperByDefault(
                processName: "MicrosoftEdgeUpdate",
                displayName: "Microsoft Edge Update",
                reason: "Background update helper that can be closed before gaming"),

            KillHelperByDefault(
                processName: "update_notifier",
                displayName: "Browser Update Notifier",
                reason: "Background update notifier that is not needed during gameplay"),

            #endregion

            #region Browsers

            OptionalClose(
                processName: "chrome",
                displayName: "Google Chrome",
                reason: "Can use large amounts of memory, CPU, GPU acceleration, and network activity"),

            KillHelperByDefault(
                processName: "msedge",
                displayName: "Microsoft Edge",
                reason: "Can use large amounts of memory, CPU, GPU acceleration, and network activity"),

            KillHelperByDefault(
                processName: "firefox",
                displayName: "Mozilla Firefox",
                reason: "Can use memory, CPU, GPU acceleration, and network activity"),

            KillHelperByDefault(
                processName: "brave",
                displayName: "Brave Browser",
                reason: "Can use memory, CPU, GPU acceleration, and network activity"),

            KillHelperByDefault(
                processName: "opera",
                displayName: "Opera",
                reason: "Can use memory, CPU, GPU acceleration, and network activity"),

            KillHelperByDefault(
                processName: "opera_gx",
                displayName: "Opera GX",
                reason: "Can use memory, CPU, GPU acceleration, and background browser features"),

            KillHelperByDefault(
                processName: "vivaldi",
                displayName: "Vivaldi",
                reason: "Can use memory, CPU, GPU acceleration, and network activity"),

            KillHelperByDefault(
                processName: "Arc",
                displayName: "Arc Browser",
                reason: "Can use memory, CPU, GPU acceleration, and network activity"),

            #endregion

            #region Game Launchers

            CloseByDefault(
                processName: "steam",
                displayName: "Steam",
                reason: "Can use memory, web views, downloads, friends, chat, cloud sync, and overlay features"),

            OptionalClose(
                processName: "steamwebhelper",
                displayName: "Steam Web Helper",
                reason: "Steam helper processes can use extra memory and CPU, but closing them may affect Steam features"),

            CloseByDefault(
                processName: "EpicGamesLauncher",
                displayName: "Epic Games Launcher",
                reason: "Can use memory, web views, downloads, updates, and launcher services"),

            OptionalClose(
                processName: "EpicWebHelper",
                displayName: "Epic Web Helper",
                reason: "Epic launcher helper process that can use memory and CPU"),

            CloseByDefault(
                processName: "Battle.net",
                displayName: "Battle.net",
                reason: "Can use memory, web views, updates, and launcher services"),

            OptionalClose(
                processName: "Agent",
                displayName: "Battle.net Agent",
                reason: "Battle.net update and launcher helper process"),

            CloseByDefault(
                processName: "EADesktop",
                displayName: "EA App",
                reason: "Can use memory, web views, updates, and background launcher services"),

            OptionalClose(
                processName: "EABackgroundService",
                displayName: "EA Background Service",
                reason: "EA background helper service used by the EA App"),

            CloseByDefault(
                processName: "Origin",
                displayName: "Origin",
                reason: "Can use memory, web views, updates, and background launcher services"),

            CloseByDefault(
                processName: "UbisoftConnect",
                displayName: "Ubisoft Connect",
                reason: "Can use memory, updates, overlays, and launcher services"),

            OptionalClose(
                processName: "upc",
                displayName: "Ubisoft Connect Helper",
                reason: "Ubisoft Connect helper process"),

            OptionalClose(
                processName: "RiotClientServices",
                displayName: "Riot Client",
                reason: "Can use memory, launcher services, and background network activity"),

            OptionalClose(
                processName: "RiotClientUx",
                displayName: "Riot Client UX",
                reason: "Riot launcher user-interface process"),

            OptionalClose(
                processName: "RiotClientUxRender",
                displayName: "Riot Client Renderer",
                reason: "Riot launcher renderer process"),

            CloseByDefault(
                processName: "GalaxyClient",
                displayName: "GOG Galaxy",
                reason: "Can use memory, cloud sync, updates, and launcher services"),

            CloseByDefault(
                processName: "GalaxyClient Helper",
                displayName: "GOG Galaxy Helper",
                reason: "GOG Galaxy helper process"),

            CloseByDefault(
                processName: "MinecraftLauncher",
                displayName: "Minecraft Launcher",
                reason: "Can remain open in the background after launching a game"),

            CloseByDefault(
                processName: "XboxPcApp",
                displayName: "Xbox App",
                reason: "Can use memory, notifications, Game Pass services, and background activity"),

            #endregion

            #region Recording / Overlay / Capture Apps

            OptionalClose(
                processName: "obs64",
                displayName: "OBS Studio",
                reason: "Can use CPU, GPU, capture hooks, encoders, and memory"),

            OptionalClose(
                processName: "obs32",
                displayName: "OBS Studio 32-bit",
                reason: "Can use CPU, GPU, capture hooks, encoders, and memory"),

            OptionalClose(
                processName: "Streamlabs Desktop",
                displayName: "Streamlabs Desktop",
                reason: "Can use CPU, GPU, capture hooks, encoders, and memory"),

            OptionalClose(
                processName: "NVIDIA Share",
                displayName: "NVIDIA Share",
                reason: "Can use overlay, recording, instant replay, and GPU resources"),

            OptionalClose(
                processName: "NVIDIA Web Helper",
                displayName: "NVIDIA Web Helper",
                reason: "NVIDIA helper process that can use memory in the background"),

            CloseByDefault(
                processName: "RadeonSoftware",
                displayName: "AMD Software",
                reason: "Can use overlay, recording, metrics, and background features"),

            OptionalClose(
                processName: "AMDRSServ",
                displayName: "AMD Radeon Settings Service",
                reason: "Can support overlay, recording, and Radeon background features"),

            CloseByDefault(
                processName: "Medal",
                displayName: "Medal",
                reason: "Can record gameplay, use encoders, overlays, and background uploads"),

            CloseByDefault(
                processName: "Overwolf",
                displayName: "Overwolf",
                reason: "Can use overlays, app hooks, browser views, CPU, memory, and GPU acceleration"),

            OptionalClose(
                processName: "Outplayed",
                displayName: "Outplayed",
                reason: "Can record gameplay and use CPU/GPU encoding resources"),

            OptionalClose(
                processName: "GameBar",
                displayName: "Xbox Game Bar",
                reason: "Can use overlay, capture, widgets, and background recording features"),

            OptionalClose(
                processName: "GameBarFTServer",
                displayName: "Xbox Game Bar Full Trust Server",
                reason: "Xbox Game Bar helper process"),

            OptionalClose(
                processName: "XboxGameBarWidgets",
                displayName: "Xbox Game Bar Widgets",
                reason: "Xbox Game Bar widget process"),

            OptionalClose(
                processName: "Action!",
                displayName: "Action Recorder",
                reason: "Can record gameplay and use CPU/GPU encoding resources"),

            OptionalClose(
                processName: "Bandicam",
                displayName: "Bandicam",
                reason: "Can record gameplay and use CPU/GPU encoding resources"),

            #endregion

            #region Music / Media Apps 

            CloseByDefault(
                processName: "Spotify",
                displayName: "Spotify",
                reason: "Can use memory, network activity, notifications, and background playback features"),

            CloseByDefault(
                processName: "AppleMusic",
                displayName: "Apple Music",
                reason: "Can use memory, network activity, and background playback features"),

            CloseByDefault(
                processName: "iTunes",
                displayName: "iTunes",
                reason: "Can use background Apple services and media library activity"),

            CloseByDefault(
                processName: "Music.UI",
                displayName: "Windows Media Player",
                reason: "Can keep media playback and library components active"),

            CloseByDefault(
                processName: "vlc",
                displayName: "VLC Media Player",
                reason: "Can keep media playback and memory active"),

            CloseByDefault(
                processName: "Plex",
                displayName: "Plex",
                reason: "Can use media server/library background activity"),

            #endregion

            #region RGB / Peripheral Software 

            CloseByDefault(
                processName: "lghub",
                displayName: "Logitech G HUB",
                reason: "Can use memory, device polling, lighting, profiles, and background device services"),

            CloseByDefault(
                processName: "lghub_agent",
                displayName: "Logitech G HUB Agent",
                reason: "Logitech G HUB background agent"),

            CloseByDefault(
                processName: "lghub_updater",
                displayName: "Logitech G HUB Updater",
                reason: "Logitech G HUB updater helper"),

            CloseByDefault(
                processName: "iCUE",
                displayName: "Corsair iCUE",
                reason: "Can use memory, lighting effects, sensors, device polling, and background services"),

            CloseByDefault(
                processName: "iCUEDevicePluginHost",
                displayName: "Corsair iCUE Plugin Host",
                reason: "Corsair iCUE plugin helper process"),

            CloseByDefault(
                processName: "Razer Synapse",
                displayName: "Razer Synapse",
                reason: "Can use memory, lighting effects, profiles, and background device services"),

            CloseByDefault(
                processName: "Razer Central",
                displayName: "Razer Central",
                reason: "Razer background launcher and account process"),

            OptionalClose(
                processName: "RazerAppEngine",
                displayName: "Razer App Engine",
                reason: "Razer background app engine process"),

            CloseByDefault(
                processName: "ArmouryCrate",
                displayName: "ASUS Armoury Crate",
                reason: "Can use memory, lighting, hardware polling, updates, and background services"),

            CloseByDefault(
                processName: "ArmouryCrate.UserSessionHelper",
                displayName: "ASUS Armoury Crate Session Helper",
                reason: "ASUS Armoury Crate user-session helper process"),

            CloseByDefault(
                processName: "AuraCreator",
                displayName: "ASUS Aura Creator",
                reason: "Can use memory and lighting-control background features"),

            CloseByDefault(
                processName: "LightingService",
                displayName: "ASUS Lighting Service",
                reason: "ASUS lighting background service"),

            CloseByDefault(
                processName: "MSI Center",
                displayName: "MSI Center",
                reason: "Can use memory, hardware monitoring, lighting, updates, and background services"),

            CloseByDefault(
                processName: "MSI.CentralServer",
                displayName: "MSI Center Server",
                reason: "MSI Center background server process"),

            CloseByDefault(
                processName: "NZXT CAM",
                displayName: "NZXT CAM",
                reason: "Can use memory, hardware polling, overlay, lighting, and monitoring features"),

            CloseByDefault(
                processName: "SignalRgb",
                displayName: "SignalRGB",
                reason: "Can use memory, lighting effects, and device polling in the background"),

            CloseByDefault(
                processName: "OpenRGB",
                displayName: "OpenRGB",
                reason: "Can use lighting and hardware-device polling in the background"),

            #endregion

            #region Hardware Monitoring / Tuning Tools 

            KillHelperByDefault(
                processName: "HWiNFO64",
                displayName: "HWiNFO64",
                reason: "Can poll sensors and use background monitoring resources"),

            KillHelperByDefault(
                processName: "HWiNFO32",
                displayName: "HWiNFO32",
                reason: "Can poll sensors and use background monitoring resources"),

            KillHelperByDefault(
                processName: "HWMonitor",
                displayName: "HWMonitor",
                reason: "Can poll sensors and use background monitoring resources"),

            CloseByDefault(
                processName: "MSIAfterburner",
                displayName: "MSI Afterburner",
                reason: "Can poll sensors, run overlays, and manage GPU tuning profiles"),

            OptionalClose(
                processName: "RTSS",
                displayName: "RivaTuner Statistics Server",
                reason: "Can hook into games for overlays, frame limiting, and monitoring"),

            OptionalClose(
                processName: "FanControl",
                displayName: "FanControl",
                reason: "Can poll sensors and control fan curves in the background"),

            OptionalClose(
                processName: "AIDA64",
                displayName: "AIDA64",
                reason: "Can poll sensors and perform background hardware monitoring"),

            KillHelperByDefault(
                processName: "CPU-Z",
                displayName: "CPU-Z",
                reason: "Can remain open and use hardware polling resources"),

            KillHelperByDefault(
                processName: "GPU-Z",
                displayName: "GPU-Z",
                reason: "Can remain open and use hardware polling resources"),

            #endregion

            #region Vendor / OEM Apps 

            OptionalClose(
                processName: "LenovoVantage",
                displayName: "Lenovo Vantage",
                reason: "Can use background device, update, telemetry, and hardware-management features"),

            OptionalClose(
                processName: "DellSupportAssist",
                displayName: "Dell SupportAssist",
                reason: "Can use background diagnostics, updates, and support services"),

            OptionalClose(
                processName: "HP Support Assistant",
                displayName: "HP Support Assistant",
                reason: "Can use background diagnostics, updates, and support features"),

            OptionalClose(
                processName: "MyASUS",
                displayName: "MyASUS",
                reason: "Can use background device support, updates, and hardware-management features"),

            OptionalClose(
                processName: "AcerCareCenter",
                displayName: "Acer Care Center",
                reason: "Can use background diagnostics, updates, and support features"),

            OptionalClose(
                processName: "KillerControlCenter",
                displayName: "Killer Control Center",
                reason: "Can run network prioritisation and background UI components"),

            OptionalClose(
                processName: "IntelGraphicsCommandCenter",
                displayName: "Intel Graphics Command Center",
                reason: "Can keep graphics control UI/background components active"),

            #endregion

            #region Creative / Adobe Helpers 

            KillHelperByDefault(
                processName: "Creative Cloud",
                displayName: "Adobe Creative Cloud",
                reason: "Can use memory, sync, updates, libraries, and background Adobe services"),

            KillHelperByDefault(
                processName: "CCXProcess",
                displayName: "Adobe CCXProcess",
                reason: "Adobe Creative Cloud helper process"),

            KillHelperByDefault(
                processName: "CoreSync",
                displayName: "Adobe CoreSync",
                reason: "Adobe cloud sync helper that can use disk and network activity"),

            KillHelperByDefault(
                processName: "Adobe Desktop Service",
                displayName: "Adobe Desktop Service",
                reason: "Adobe Creative Cloud background service"),

            KillHelperByDefault(
                processName: "AdobeGCClient",
                displayName: "Adobe Genuine Client",
                reason: "Adobe background validation helper process"),

            #endregion

            #region Office / Productivity Apps 

            OptionalClose(
                processName: "OUTLOOK",
                displayName: "Microsoft Outlook",
                reason: "Can use memory, network, notifications, indexing, and sync activity"),

            OptionalClose(
                processName: "WINWORD",
                displayName: "Microsoft Word",
                reason: "Can use memory and background document services. Close only if work is saved"),

            OptionalClose(
                processName: "EXCEL",
                displayName: "Microsoft Excel",
                reason: "Can use memory and background document services. Close only if work is saved"),

            OptionalClose(
                processName: "POWERPNT",
                displayName: "Microsoft PowerPoint",
                reason: "Can use memory and background document services. Close only if work is saved"),

            OptionalClose(
                processName: "ONENOTE",
                displayName: "Microsoft OneNote",
                reason: "Can sync notes and use background network activity"),

            OptionalClose(
                processName: "Notion",
                displayName: "Notion",
                reason: "Can use memory, network, web views, and notifications"),

            OptionalClose(
                processName: "Obsidian",
                displayName: "Obsidian",
                reason: "Can use memory, plugins, sync, and background activity"),

            #endregion

            #region Remote Access

            KillHelperByDefault(
                processName: "TeamViewer",
                displayName: "TeamViewer",
                reason: "Can keep remote-access connectivity running in the background"),

            KillHelperByDefault(
                processName: "AnyDesk",
                displayName: "AnyDesk",
                reason: "Can keep remote-access connectivity running in the background"),

            KillHelperByDefault(
                processName: "RustDesk",
                displayName: "RustDesk",
                reason: "Can keep remote-access connectivity running in the background"),

            KillHelperByDefault(
                processName: "parsecd",
                displayName: "Parsec",
                reason: "Can keep remote streaming and remote-control services active"),

            KillHelperByDefault(
                processName: "Moonlight",
                displayName: "Moonlight",
                reason: "Can keep game-streaming components active"),

            #endregion

            #region VPN 
            KillHelperByDefault(
                processName: "openvpnserv2",
                displayName: "OpenVPN",
                reason: ""),

            KillHelperByDefault(
                processName: "OpenVPN Service",
                displayName: "OpenVPN Service",
                reason: ""),

            #endregion

            #region Misc Optional Background Apps

            OptionalClose(
                processName: "PowerToys",
                displayName: "Microsoft PowerToys",
                reason: "Can keep utility hooks, keyboard managers, window tools, and background modules running"),

            OptionalClose(
                processName: "Everything",
                displayName: "Everything Search",
                reason: "Can keep file indexing/search helper activity running"),

            OptionalClose(
                processName: "ShareX",
                displayName: "ShareX",
                reason: "Can run hotkeys, capture hooks, clipboard monitoring, and background upload helpers"),

            OptionalClose(
                processName: "Lightshot",
                displayName: "Lightshot",
                reason: "Can run screenshot hooks and background capture features"),

            OptionalClose(
                processName: "Greenshot",
                displayName: "Greenshot",
                reason: "Can run screenshot hooks and background capture features"),

            OptionalClose(
                processName: "Wallpaper64",
                displayName: "Wallpaper Engine",
                reason: "Can use GPU, video decoding, and background animation resources"),

            OptionalClose(
                processName: "wallpaper32",
                displayName: "Wallpaper Engine 32-bit",
                reason: "Can use GPU, video decoding, and background animation resources"),

            OptionalClose(
                processName: "Rainmeter",
                displayName: "Rainmeter",
                reason: "Can use desktop widgets, skins, sensor polling, and background UI activity"),

            #endregion

        ];

        #region Helpers

        private static GamingFocusProcessDefinition CloseByDefault(
            string processName,
            string displayName,
            string reason)
        {
            return new GamingFocusProcessDefinition
            {
                ProcessName = processName,
                DisplayName = displayName,
                Reason = reason,
                TryGracefulClose = true,
                AllowForceKill = false,
                EnabledByDefault = true
            };
        }

        private static GamingFocusProcessDefinition KillHelperByDefault(
            string processName,
            string displayName,
            string reason)
        {
            return new GamingFocusProcessDefinition
            {
                ProcessName = processName,
                DisplayName = displayName,
                Reason = reason,
                TryGracefulClose = false,
                AllowForceKill = true,
                EnabledByDefault = true
            };
        }

        private static GamingFocusProcessDefinition OptionalClose(
            string processName,
            string displayName,
            string reason)
        {
            return new GamingFocusProcessDefinition
            {
                ProcessName = processName,
                DisplayName = displayName,
                Reason = reason,
                TryGracefulClose = true,
                AllowForceKill = false,
                EnabledByDefault = false
            };
        }

        private static GamingFocusProcessDefinition OptionalKill(
            string processName,
            string displayName,
            string reason)
        {
            return new GamingFocusProcessDefinition
            {
                ProcessName = processName,
                DisplayName = displayName,
                Reason = reason,
                TryGracefulClose = false,
                AllowForceKill = true,
                EnabledByDefault = false
            };
        }

        #endregion
    }
}