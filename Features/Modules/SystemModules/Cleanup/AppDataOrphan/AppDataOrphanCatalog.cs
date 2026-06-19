namespace GameBoost.Features.Modules.SystemModules.Cleanup.AppDataOrphan
{
    public static class AppDataOrphanCatalog
    {
        public static IReadOnlyList<AppDataOrphanDefinition> Rules { get; } =
        [
            #region Game Launchers / Stores

            Rule(
                displayName: "Steam",
                folderNames:
                [
                    "Steam",
                    "Valve",
                    "Valve Corporation"
                ],
                installedProgramNames:
                [
                    "Steam"
                ],
                processNames:
                [
                    "steam",
                    "steamwebhelper",
                    "steamservice"
                ],
                highRisk: true),

            Rule(
                displayName: "Epic Games Launcher",
                folderNames:
                [
                    "Epic Games",
                    "EpicGamesLauncher",
                    "EpicWebHelper",
                    "UnrealEngineLauncher"
                ],
                installedProgramNames:
                [
                    "Epic Games Launcher"
                ],
                processNames:
                [
                    "EpicGamesLauncher",
                    "EpicWebHelper"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "EA App",
                folderNames:
                [
                    "EA",
                    "EADesktop",
                    "EALaunchHelper",
                    "EAConnect_microsoft",
                    "Electronic Arts",
                    "Origin",
                    "Link2EA"
                ],
                installedProgramNames:
                [
                    "EA app",
                    "EA Desktop",
                    "Origin"
                ],
                processNames:
                [
                    "EADesktop",
                    "EABackgroundService",
                    "EALaunchHelper",
                    "Origin"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "Battle.net",
                folderNames:
                [
                    "Battle.net",
                    "Blizzard Entertainment"
                ],
                installedProgramNames:
                [
                    "Battle.net"
                ],
                processNames:
                [
                    "Battle.net",
                    "Agent"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "Ubisoft Connect",
                folderNames:
                [
                    "Ubisoft",
                    "Ubisoft Connect"
                ],
                installedProgramNames:
                [
                    "Ubisoft Connect",
                    "Uplay"
                ],
                processNames:
                [
                    "UbisoftConnect",
                    "upc"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "GOG Galaxy",
                folderNames:
                [
                    "GOG.com",
                    "GOG Galaxy",
                    "GalaxyClient"
                ],
                installedProgramNames:
                [
                    "GOG Galaxy"
                ],
                processNames:
                [
                    "GalaxyClient",
                    "GalaxyClient Helper"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "Riot Games",
                folderNames:
                [
                    "Riot Games",
                    "riot-client-ux",
                    "VALORANT",
                    "League of Legends"
                ],
                installedProgramNames:
                [
                    "Riot Client",
                    "VALORANT",
                    "League of Legends"
                ],
                processNames:
                [
                    "RiotClientServices",
                    "RiotClientUx",
                    "RiotClientUxRender",
                    "VALORANT",
                    "LeagueClient"
                ],
                minimumAgeDays: 60,
                highRisk: true),

            Rule(
                displayName: "Xbox App",
                folderNames:
                [
                    "Xbox",
                    "XboxPcApp",
                    "GamingServices"
                ],
                installedProgramNames:
                [
                    "Xbox",
                    "Gaming Services"
                ],
                processNames:
                [
                    "XboxPcApp",
                    "GamingServices",
                    "GamingServicesNet"
                ],
                minimumAgeDays: 180,
                highRisk: true),

            #endregion

            #region Game / Anti-Cheat Ecosystems

            Rule(
                displayName: "Battlefield",
                folderNames:
                [
                    "Battlefield",
                    "Battlefield 6",
                    "Battlefield V",
                    "Battlefield 2042",
                    "BattlefieldGameData.CH1-qol.Win32",
                    "BattlefieldGameData.CH1-release.Win32",
                    "BattlefieldGameData.kin-live.Win32",
                    "BattlefieldGameData.kin-release.Win32"
                ],
                installedProgramNames:
                [
                    "Battlefield",
                    "Battlefield 6",
                    "Battlefield 2042",
                    "Battlefield V"
                ],
                processNames:
                [
                    "bf6",
                    "BF2042",
                    "bfv"
                ],
                minimumAgeDays: 60,
                highRisk: true),

            Rule(
                displayName: "Activision / Call of Duty",
                folderNames:
                [
                    "Activision",
                    "Call of Duty",
                    "CallOfDuty"
                ],
                installedProgramNames:
                [
                    "Call of Duty",
                    "Warzone",
                    "Modern Warfare",
                    "Black Ops"
                ],
                processNames:
                [
                    "cod",
                    "cod22",
                    "cod23",
                    "bootstrapper"
                ],
                minimumAgeDays: 60,
                highRisk: true),

            Rule(
                displayName: "Battlestate Games",
                folderNames:
                [
                    "Battlestate Games",
                    "EscapeFromTarkov",
                    "EscapeStore"
                ],
                installedProgramNames:
                [
                    "Battlestate Games",
                    "Escape from Tarkov",
                    "Tarkov"
                ],
                processNames:
                [
                    "EscapeFromTarkov",
                    "BsgLauncher"
                ],
                minimumAgeDays: 60,
                highRisk: true),

            Rule(
                displayName: "Easy Anti-Cheat",
                folderNames:
                [
                    "EasyAntiCheat",
                    "EasyAntiCheat_EOS"
                ],
                installedProgramNames:
                [
                    "Easy Anti-Cheat"
                ],
                processNames:
                [
                    "EasyAntiCheat",
                    "EasyAntiCheat_EOS"
                ],
                minimumAgeDays: 180,
                deleteWhenEmptyOnly: true),

            Rule(
                displayName: "EA AntiCheat",
                folderNames:
                [
                    "EAAntiCheat",
                    "EAAntiCheat.Installer.Tool"
                ],
                installedProgramNames:
                [
                    "EA AntiCheat"
                ],
                processNames:
                [
                    "EAAntiCheat.GameServiceLauncher",
                    "EAAntiCheat.Installer.Tool"
                ],
                minimumAgeDays: 180,
                deleteWhenEmptyOnly: true),

            Rule(
                displayName: "PunkBuster",
                folderNames:
                [
                    "PunkBuster",
                    "PB"
                ],
                installedProgramNames:
                [
                    "PunkBuster"
                ],
                processNames:
                [
                    "PnkBstrA",
                    "PnkBstrB"
                ],
                minimumAgeDays: 60,
                deleteWhenEmptyOnly: true),

            Rule(
                displayName: "Anybrain SDK",
                folderNames:
                [
                    "AnybrainSDK"
                ],
                installedProgramNames:
                [
                    "Anybrain"
                ],
                processNames:
                [
                    "Anybrain"
                ],
                minimumAgeDays: 60,
                deleteWhenEmptyOnly: true),

            Rule(
                displayName: "Vivox",
                folderNames:
                [
                    "Vivox"
                ],
                installedProgramNames:
                [
                    "Vivox"
                ],
                processNames:
                [
                    "Vivox"
                ],
                minimumAgeDays: 60,
                highRisk: true),

            Rule(
                displayName: "GameAnalytics",
                folderNames:
                [
                    "GameAnalytics"
                ],
                installedProgramNames:
                [
                    "GameAnalytics"
                ],
                processNames:
                [
                    "GameAnalytics"
                ],
                minimumAgeDays: 60,
                deleteWhenEmptyOnly: true),

            Rule(
                displayName: "mod.io",
                folderNames:
                [
                    "mod.io",
                    "modio"
                ],
                installedProgramNames:
                [
                    "mod.io"
                ],
                processNames:
                [
                    "modio"
                ],
                minimumAgeDays: 60,
                highRisk: true),

            #endregion

            #region Game Save / Game Data Folders

            GameFolder("Baldur's Gate 3 / Larian Studios",
                ["Larian Studios", "BG3ScriptExtender"],
                ["Baldur's Gate 3"],
                ["bg3", "bg3_dx11"]),

            GameFolder("Dead by Daylight",
                ["DeadByDaylight"],
                ["Dead by Daylight"],
                ["DeadByDaylight-Win64-Shipping"]),

            GameFolder("Dead Island",
                ["DeadIsland"],
                ["Dead Island"],
                ["DeadIsland"]),

            GameFolder("Ready or Not",
                ["ReadyOrNot"],
                ["Ready or Not"],
                ["ReadyOrNot-Win64-Shipping"]),

            GameFolder("The Isle",
                ["TheIsle"],
                ["The Isle"],
                ["TheIsle"]),

            GameFolder("Satisfactory",
                ["FactoryGame"],
                ["Satisfactory"],
                ["FactoryGame-Win64-Shipping"]),

            GameFolder("Cyberpunk / CD Projekt Red",
                ["CD Projekt Red", "REDEngine"],
                ["Cyberpunk 2077", "CD Projekt Red"],
                ["Cyberpunk2077"]),

            GameFolder("Rockstar Games",
                ["Rockstar Games"],
                ["Rockstar Games Launcher", "Grand Theft Auto", "Red Dead Redemption"],
                ["RockstarService", "Launcher", "GTA5", "RDR2"]),

            GameFolder("Marvel Rivals",
                ["MarvelRivals_Launcher"],
                ["Marvel Rivals"],
                ["MarvelRivals_Launcher", "MarvelRivals"]),

            GameFolder("Netease Games",
                ["Netease"],
                ["NetEase", "Netease"],
                ["Netease"]),

            GameFolder("Meta Quest / Oculus",
                ["Oculus", "Meta Quest Link", "Meta Quest Remote Desktop", "MetaXR", "openvr"],
                ["Oculus", "Meta Quest Link", "Meta Quest Remote Desktop", "SteamVR"],
                ["OculusClient", "OVRServer_x64", "MetaQuestRemoteDesktop"]),

            GameFolder("Star Stable Online",
                ["Star Stable Online", "starstableonline-updater"],
                ["Star Stable Online"],
                ["StarStableOnline"]),

            GameFolder("Command & Conquer",
                ["Command & Conquer 3 Kane's Wrath", "Command & Conquer 3 Kanes Wrath"],
                ["Command & Conquer"],
                ["CNC3", "cnc3ep1"]),

            GameFolder("PopCap Games",
                ["PopCap Games"],
                ["PopCap", "Plants vs. Zombies", "Bejeweled"],
                ["PlantsVsZombies", "Bejeweled"]),

            GameFolder("Split Fiction",
                ["SplitFiction"],
                ["Split Fiction"],
                ["SplitFiction"]),

            GameFolder("Tokyo Xtreme Racer",
                ["TokyoXtremeRacer"],
                ["Tokyo Xtreme Racer"],
                ["TokyoXtremeRacer"]),

            GameFolder("Revenge of the Savage Planet",
                ["RevengeOfTheSavagePlanet"],
                ["Revenge of the Savage Planet"],
                ["RevengeOfTheSavagePlanet"]),

            GameFolder("Escape the Backrooms",
                ["EscapeTheBackrooms"],
                ["Escape the Backrooms"],
                ["EscapeTheBackrooms"]),

            GameFolder("Insomniac Games",
                ["Insomniac Games"],
                ["Insomniac"],
                ["Spider-Man", "MilesMorales", "Ratchet"]),

            GameFolder("Goldberg / Steam Emulator Saves",
                ["Goldberg SteamEmu Saves", "GSE Saves", "SmartSteamEmu"],
                ["SmartSteamEmu", "Goldberg"],
                ["SmartSteamEmu"],
                minimumAgeDays: 60),

            #endregion

            #region Development Tools / Engines

            Rule(
                displayName: "Unity",
                folderNames:
                [
                    "Unity",
                    "UnityHub",
                    "Unity Technologies",
                    "unityhub-updater"
                ],
                installedProgramNames:
                [
                    "Unity Hub",
                    "Unity Editor",
                    "Unity 6",
                    "Unity 2023",
                    "Unity 2022",
                    "Unity 2021",
                    "Unity Technologies"
                ],
                processNames:
                [
                    "Unity",
                    "Unity Hub",
                    "UnityHub"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "Unreal Engine",
                folderNames:
                [
                    "UnrealEngine",
                    "Unreal Engine"
                ],
                installedProgramNames:
                [
                    "Unreal Engine",
                    "Epic Games Launcher"
                ],
                processNames:
                [
                    "UnrealEditor",
                    "UE4Editor",
                    "UE5Editor",
                    "EpicGamesLauncher"
                ],
                minimumAgeDays: 60,
                highRisk: true),

            Rule(
                displayName: "Godot",
                folderNames:
                [
                    "Godot"
                ],
                installedProgramNames:
                [
                    "Godot"
                ],
                processNames:
                [
                    "Godot"
                ],
                minimumAgeDays: 60,
                highRisk: true),

            Rule(
                displayName: "Visual Studio / Setup",
                folderNames:
                [
                    "Visual Studio Setup",
                    "Microsoft FxCop",
                    "Microsoft SDKs",
                    "RefSrcSymbols",
                    "SymbolSourceSymbols",
                    "SourceServer"
                ],
                installedProgramNames:
                [
                    "Visual Studio",
                    "Microsoft Visual Studio",
                    ".NET SDK",
                    "Windows SDK"
                ],
                processNames:
                [
                    "devenv",
                    "VSIXInstaller",
                    "ServiceHub",
                    "MSBuild"
                ],
                minimumAgeDays: 180,
                highRisk: true),

            Rule(
                displayName: "VS Code",
                folderNames:
                [
                    "Code"
                ],
                installedProgramNames:
                [
                    "Microsoft Visual Studio Code",
                    "Visual Studio Code"
                ],
                processNames:
                [
                    "Code"
                ],
                minimumAgeDays: 180,
                highRisk: true),

            Rule(
                displayName: "GitHub Desktop",
                folderNames:
                [
                    "GitHubDesktop",
                    "GitHub Desktop"
                ],
                installedProgramNames:
                [
                    "GitHub Desktop"
                ],
                processNames:
                [
                    "GitHubDesktop"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "GitHub Copilot",
                folderNames:
                [
                    "github-copilot"
                ],
                installedProgramNames:
                [
                    "GitHub Copilot"
                ],
                processNames:
                [
                    "Code",
                    "devenv"
                ],
                minimumAgeDays: 60,
                highRisk: true),

            Rule(
                displayName: "JetBrains",
                folderNames:
                [
                    "JetBrains"
                ],
                installedProgramNames:
                [
                    "JetBrains",
                    "Rider",
                    "WebStorm",
                    "PyCharm",
                    "IntelliJ IDEA",
                    "ReSharper"
                ],
                processNames:
                [
                    "rider64",
                    "webstorm64",
                    "pycharm64",
                    "idea64",
                    "datagrip64"
                ],
                minimumAgeDays: 60,
                highRisk: true),

            Rule(
                displayName: "Postman",
                folderNames:
                [
                    "Postman"
                ],
                installedProgramNames:
                [
                    "Postman"
                ],
                processNames:
                [
                    "Postman"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "NuGet",
                folderNames:
                [
                    "NuGet"
                ],
                installedProgramNames:
                [
                    "NuGet",
                    "Visual Studio",
                    ".NET SDK"
                ],
                processNames:
                [
                    "devenv",
                    "dotnet"
                ],
                minimumAgeDays: 180,
                highRisk: true),

            Rule(
                displayName: "Node / npm",
                folderNames:
                [
                    "npm-cache",
                    "SquirrelTemp"
                ],
                installedProgramNames:
                [
                    "Node.js",
                    "npm"
                ],
                processNames:
                [
                    "node",
                    "npm"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "Python / pip",
                folderNames:
                [
                    "pip",
                    "conda",
                    "conda-anaconda-tos",
                    "numba",
                    "uv"
                ],
                installedProgramNames:
                [
                    "Python",
                    "Anaconda",
                    "Miniconda",
                    "uv"
                ],
                processNames:
                [
                    "python",
                    "pythonw",
                    "conda",
                    "uv"
                ],
                minimumAgeDays: 180,
                highRisk: true),

            Rule(
                displayName: "Playwright",
                folderNames:
                [
                    "ms-playwright",
                    "ms-playwright-go"
                ],
                installedProgramNames:
                [
                    "Playwright"
                ],
                processNames:
                [
                    "node",
                    "python",
                    "playwright"
                ],
                minimumAgeDays: 180),

            Rule(
                displayName: "Azure Functions Tools",
                folderNames:
                [
                    "AzureFunctionsTools"
                ],
                installedProgramNames:
                [
                    "Azure Functions Core Tools",
                    "Visual Studio"
                ],
                processNames:
                [
                    "func",
                    "devenv",
                    "Code"
                ],
                minimumAgeDays: 365,
                highRisk: true),

            Rule(
                displayName: "Uno Platform",
                folderNames:
                [
                    "Uno Platform"
                ],
                installedProgramNames:
                [
                    "Uno Platform"
                ],
                processNames:
                [
                    "devenv",
                    "Code"
                ],
                minimumAgeDays: 365,
                highRisk: true),

            Rule(
                displayName: "ComfyUI / Comfy Desktop",
                folderNames:
                [
                    "Comfy Desktop",
                    "comfyui-desktop-2-updater"
                ],
                installedProgramNames:
                [
                    "ComfyUI",
                    "Comfy Desktop"
                ],
                processNames:
                [
                    "ComfyUI",
                    "Comfy Desktop"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "Ultralytics",
                folderNames:
                [
                    "Ultralytics"
                ],
                installedProgramNames:
                [
                    "Ultralytics"
                ],
                processNames:
                [
                    "python",
                    "yolo"
                ],
                minimumAgeDays: 365,
                highRisk: true),

            Rule(
                displayName: "VMware",
                folderNames:
                [
                    "VMware"
                ],
                installedProgramNames:
                [
                    "VMware",
                    "VMware Workstation",
                    "VMware Player"
                ],
                processNames:
                [
                    "vmware",
                    "vmware-vmx"
                ],
                minimumAgeDays: 365,
                highRisk: true),

            Rule(
                displayName: "Hex-Rays / IDA",
                folderNames:
                [
                    "Hex-Rays"
                ],
                installedProgramNames:
                [
                    "IDA",
                    "IDA Pro",
                    "Hex-Rays"
                ],
                processNames:
                [
                    "ida",
                    "ida64"
                ],
                minimumAgeDays: 365,
                highRisk: true),

            #endregion

            #region Creative / Productivity Apps

            Rule(
                displayName: "Adobe",
                folderNames:
                [
                    "Adobe",
                    "AdobeCommon",
                    "com.adobe.dunamis"
                ],
                installedProgramNames:
                [
                    "Adobe",
                    "Adobe Photoshop",
                    "Adobe Premiere",
                    "Adobe After Effects",
                    "Adobe Creative Cloud",
                    "Creative Cloud"
                ],
                processNames:
                [
                    "Creative Cloud",
                    "CCXProcess",
                    "CoreSync",
                    "Adobe Desktop Service",
                    "Photoshop",
                    "Adobe Premiere Pro"
                ],
                minimumAgeDays: 180,
                highRisk: true),

            Rule(
                displayName: "CapCut",
                folderNames:
                [
                    "CapCut"
                ],
                installedProgramNames:
                [
                    "CapCut"
                ],
                processNames:
                [
                    "CapCut"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "Audacity",
                folderNames:
                [
                    "audacity"
                ],
                installedProgramNames:
                [
                    "Audacity"
                ],
                processNames:
                [
                    "audacity"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "MuseSampler",
                folderNames:
                [
                    "MuseSampler"
                ],
                installedProgramNames:
                [
                    "MuseSampler",
                    "MuseScore"
                ],
                processNames:
                [
                    "MuseSampler",
                    "MuseScore"
                ],
                minimumAgeDays: 180),

            Rule(
                displayName: "WinRAR",
                folderNames:
                [
                    "WinRAR"
                ],
                installedProgramNames:
                [
                    "WinRAR"
                ],
                processNames:
                [
                    "WinRAR"
                ],
                minimumAgeDays: 180,
                highRisk: true),

            Rule(
                displayName: "Rufus",
                folderNames:
                [
                    "Rufus"
                ],
                installedProgramNames:
                [
                    "Rufus"
                ],
                processNames:
                [
                    "rufus"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "O&O Software",
                folderNames:
                [
                    "OO Software",
                    "O&O Software"
                ],
                installedProgramNames:
                [
                    "O&O",
                    "O&O Software"
                ],
                processNames:
                [
                    "OODefrag",
                    "OOSU10"
                ],
                minimumAgeDays: 60,
                highRisk: true),

            #endregion

            #region Communication / Media Apps

            Rule(
                displayName: "Discord",
                folderNames:
                [
                    "Discord",
                    "discord"
                ],
                installedProgramNames:
                [
                    "Discord"
                ],
                processNames:
                [
                    "Discord"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "Spotify",
                folderNames:
                [
                    "Spotify",
                    "spicetify"
                ],
                installedProgramNames:
                [
                    "Spotify"
                ],
                processNames:
                [
                    "Spotify"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "TeamViewer",
                folderNames:
                [
                    "TeamViewer"
                ],
                installedProgramNames:
                [
                    "TeamViewer"
                ],
                processNames:
                [
                    "TeamViewer"
                ],
                minimumAgeDays: 60),

            #endregion

            #region Peripheral / Driver Utility Apps

            Rule(
                displayName: "Logitech G HUB",
                folderNames:
                [
                    "G HUB",
                    "LGHUB",
                    "LGHUB_BKP"
                ],
                installedProgramNames:
                [
                    "Logitech G HUB",
                    "G HUB"
                ],
                processNames:
                [
                    "lghub",
                    "lghub_agent",
                    "lghub_updater"
                ],
                minimumAgeDays: 60,
                highRisk: true),

            Rule(
                displayName: "DS4Windows",
                folderNames:
                [
                    "DS4Windows"
                ],
                installedProgramNames:
                [
                    "DS4Windows"
                ],
                processNames:
                [
                    "DS4Windows"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "Samsung Magician",
                folderNames:
                [
                    "Samsung Magician"
                ],
                installedProgramNames:
                [
                    "Samsung Magician"
                ],
                processNames:
                [
                    "SamsungMagician"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "YUNZII Driver",
                folderNames:
                [
                    "YUNZII Driver",
                    "yunzii driver-updater"
                ],
                installedProgramNames:
                [
                    "YUNZII Driver",
                    "YUNZII"
                ],
                processNames:
                [
                    "YUNZII Driver",
                    "yunzii"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "HP Utilities",
                folderNames:
                [
                    "HP"
                ],
                installedProgramNames:
                [
                    "HP",
                    "HP Support Assistant",
                    "HP Smart"
                ],
                processNames:
                [
                    "HPSupportAssistant",
                    "HPSystemEventUtilityHost"
                ],
                minimumAgeDays: 60),

            #endregion

            #region AI / Coding Assistants / Editors

            Rule(
                displayName: "Codeium",
                folderNames:
                [
                    ".codeium"
                ],
                installedProgramNames:
                [
                    "Codeium"
                ],
                processNames:
                [
                    "Code",
                    "devenv"
                ],
                minimumAgeDays: 60),

            Rule(
                displayName: "Fitten Code",
                folderNames:
                [
                    ".fittencode"
                ],
                installedProgramNames:
                [
                    "Fitten Code",
                    "Fitten"
                ],
                processNames:
                [
                    "Code",
                    "devenv"
                ],
                minimumAgeDays: 60),

            #endregion

            #region Installers / Updaters / Empty-Only Leftovers

            EmptyOnly(
                displayName: "Downloaded Installations",
                folderNames:
                [
                    "Downloaded Installations"
                ],
                minimumAgeDays: 30),

            EmptyOnly(
                displayName: "Deployment",
                folderNames:
                [
                    "Deployment"
                ],
                minimumAgeDays: 30),

            EmptyOnly(
                displayName: "Elevated Diagnostics",
                folderNames:
                [
                    "ElevatedDiagnostics"
                ],
                minimumAgeDays: 30),

            EmptyOnly(
                displayName: "CefSharp Cache",
                folderNames:
                [
                    "CefSharpCache",
                    "CEF"
                ],
                minimumAgeDays: 30),

            EmptyOnly(
                displayName: "Generic Cache Folder",
                folderNames:
                [
                    "cache"
                ],
                minimumAgeDays: 30),

            EmptyOnly(
                displayName: "Plastic SCM Legacy Folder",
                folderNames:
                [
                    "plastic4"
                ],
                minimumAgeDays: 180),

            EmptyOnly(
                displayName: "Toast Notification Compatibility Folder",
                folderNames:
                [
                    "ToastNotificationManagerCompat"
                ],
                minimumAgeDays: 180),

            EmptyOnly(
                displayName: "Placeholder Tile Logo Folder",
                folderNames:
                [
                    "PlaceholderTileLogoFolder"
                ],
                minimumAgeDays: 180),

            #endregion

            #region App / SDK Telemetry Leftovers

            Rule(
                displayName: "AppsFlyer",
                folderNames:
                [
                    "appsflyer"
                ],
                installedProgramNames:
                [
                    "AppsFlyer"
                ],
                processNames:
                [
                    "appsflyer"
                ],
                minimumAgeDays: 180,
                deleteWhenEmptyOnly: true),

            Rule(
                displayName: "Sentry",
                folderNames:
                [
                    "Sentry"
                ],
                installedProgramNames:
                [
                    "Sentry"
                ],
                processNames:
                [
                    "Sentry"
                ],
                minimumAgeDays: 180,
                deleteWhenEmptyOnly: true),

            Rule(
                displayName: "NgConsentManager",
                folderNames:
                [
                    "NgConsentManager"
                ],
                installedProgramNames:
                [
                    "NgConsentManager"
                ],
                processNames:
                [
                    "NgConsentManager"
                ],
                minimumAgeDays: 180,
                deleteWhenEmptyOnly: true),

            Rule(
                displayName: "UniSDK",
                folderNames:
                [
                    "UniSDK",
                    "UniSDK_FirstOpen",
                    "UniCompactView"
                ],
                installedProgramNames:
                [
                    "UniSDK"
                ],
                processNames:
                [
                    "UniSDK"
                ],
                minimumAgeDays: 180,
                deleteWhenEmptyOnly: true),

            #endregion
        ];

        #region Helpers

        private static AppDataOrphanDefinition GameFolder(
            string displayName,
            IReadOnlyList<string> folderNames,
            IReadOnlyList<string> installedProgramNames,
            IReadOnlyList<string> processNames,
            int minimumAgeDays = 60)
        {
            return Rule(
                displayName,
                folderNames,
                installedProgramNames,
                processNames,
                minimumAgeDays,
                highRisk: true);
        }

        private static AppDataOrphanDefinition EmptyOnly(
            string displayName,
            IReadOnlyList<string> folderNames,
            int minimumAgeDays)
        {
            return Rule(
                displayName,
                folderNames,
                installedProgramNames: [],
                processNames: [],
                minimumAgeDays,
                highRisk: true,
                deleteWhenEmptyOnly: true);
        }

        private static AppDataOrphanDefinition Rule(
            string displayName,
            IReadOnlyList<string> folderNames,
            IReadOnlyList<string> installedProgramNames,
            IReadOnlyList<string> processNames,
            int minimumAgeDays = 180,
            bool highRisk = false,
            bool deleteWhenEmptyOnly = false)
        {
            return new AppDataOrphanDefinition
            {
                DisplayName = displayName,
                FolderNames = folderNames,
                InstalledProgramNames = installedProgramNames,
                ProcessNames = processNames,
                MinimumAge = TimeSpan.FromDays(minimumAgeDays),
                HighRisk = highRisk,
                DeleteWhenEmptyOnly = deleteWhenEmptyOnly
            };
        }

        #endregion
    }
}