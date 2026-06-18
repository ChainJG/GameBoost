using GameBoost.Infrastructure.Services;
using MaterialDesignThemes.Wpf;

namespace GameBoost.Features.Modules.WindowsModules.Services.Catalog
{
    public static class WindowsServiceCatalog
    {
        public static IReadOnlyList<ServiceEditInfo> Services { get; } =
        [
            // Telemetry / tracking
            Service("DiagTrack", "Connected User Experiences and Telemetry", "Windows telemetry and diagnostic data service", WindowsServiceStartupMode.Disabled, "Recommended disabled for privacy-focused gaming systems", PackIconKind.ChartTimelineVariant),
            Service("dmwappushservice", "WAP Push Message Routing", "Push message routing service used by Windows telemetry components", WindowsServiceStartupMode.Disabled, "Usually unnecessary on gaming desktops", PackIconKind.MessageProcessingOutline),
            Service("lfsvc", "Geolocation Service", "Provides location services to Windows and apps", WindowsServiceStartupMode.Disabled, "Disable if you do not use location-based apps", PackIconKind.MapMarkerOff),
            Service("WerSvc", "Windows Error Reporting", "Collects and sends application crash reports", WindowsServiceStartupMode.Manual, "Manual keeps crash reporting available without forcing it always on", PackIconKind.AlertCircleOutline),

            // Remote / demo services
            Service("RemoteRegistry", "Remote Registry", "Allows remote users to modify registry settings", WindowsServiceStartupMode.Disabled, "Usually unnecessary and better disabled for home gaming PCs", PackIconKind.RemoteDesktop),
            Service("RetailDemo", "Retail Demo Service", "Used for store demo devices", WindowsServiceStartupMode.Disabled, "Not needed on normal gaming PCs", PackIconKind.StoreOff),
            Service("RemoteAccess", "Routing and Remote Access", "Provides routing/VPN server functionality", WindowsServiceStartupMode.Disabled, "Disable if the PC is not being used as a router or VPN server", PackIconKind.RouterNetwork),
            Service("shpamsvc", "Shared PC Account Manager", "Used for shared PC account scenarios", WindowsServiceStartupMode.Disabled, "Usually unnecessary on personal gaming PCs", PackIconKind.AccountMultipleRemove),
            Service("SEMgrSvc", "Payments and NFC/SE Manager", "Supports secure element payments and related features", WindowsServiceStartupMode.Disabled, "Usually unnecessary on desktop gaming PCs", PackIconKind.CreditCardOff),
            Service("PhoneSvc", "Phone Service", "Supports phone integration features", WindowsServiceStartupMode.Disabled, "Disable if you do not use phone-link features", PackIconKind.CellphoneOff),

            // Hyper-V / virtualisation
            Service("HvHost", "Hyper-V Host Compute Service", "Supports Hyper-V host compute features", WindowsServiceStartupMode.Disabled, "Disable only if you do not use Hyper-V, WSL2, Windows Sandbox, or virtual machines", PackIconKind.ServerOff, highRisk: true),
            Service("vmickvpexchange", "Hyper-V Data Exchange Service", "Hyper-V guest integration service", WindowsServiceStartupMode.Disabled, "Disable only if you do not use Hyper-V virtual machines", PackIconKind.ServerOff, highRisk: true),
            Service("vmicguestinterface", "Hyper-V Guest Service Interface", "Hyper-V guest integration service", WindowsServiceStartupMode.Disabled, "Disable only if you do not use Hyper-V virtual machines", PackIconKind.ServerOff, highRisk: true),
            Service("vmicshutdown", "Hyper-V Guest Shutdown Service", "Hyper-V guest shutdown integration", WindowsServiceStartupMode.Disabled, "Disable only if you do not use Hyper-V virtual machines", PackIconKind.ServerOff, highRisk: true),
            Service("vmicheartbeat", "Hyper-V Heartbeat Service", "Hyper-V guest health integration", WindowsServiceStartupMode.Disabled, "Disable only if you do not use Hyper-V virtual machines", PackIconKind.ServerOff, highRisk: true),
            Service("vmicvmsession", "Hyper-V PowerShell Direct Service", "Hyper-V guest session integration", WindowsServiceStartupMode.Disabled, "Disable only if you do not use Hyper-V virtual machines", PackIconKind.ServerOff, highRisk: true),
            Service("vmicrdv", "Hyper-V Remote Desktop Virtualization Service", "Hyper-V remote desktop integration", WindowsServiceStartupMode.Disabled, "Disable only if you do not use Hyper-V virtual machines", PackIconKind.ServerOff, highRisk: true),
            Service("vmictimesync", "Hyper-V Time Synchronization Service", "Hyper-V guest time sync integration", WindowsServiceStartupMode.Disabled, "Disable only if you do not use Hyper-V virtual machines", PackIconKind.ServerOff, highRisk: true),
            Service("vmicvss", "Hyper-V Volume Shadow Copy Requestor", "Hyper-V guest backup integration", WindowsServiceStartupMode.Disabled, "Disable only if you do not use Hyper-V virtual machines", PackIconKind.ServerOff, highRisk: true),

            // Manual, not disabled
            Service("bthserv", "Bluetooth Support Service", "Supports Bluetooth devices", WindowsServiceStartupMode.Manual, "Manual is safer than disabled because controllers, headsets, and peripherals may need Bluetooth", PackIconKind.Bluetooth),
            Service("BTAGService", "Bluetooth Audio Gateway Service", "Supports Bluetooth hands-free audio", WindowsServiceStartupMode.Manual, "Manual keeps Bluetooth audio available when needed", PackIconKind.BluetoothAudio),
            Service("Spooler", "Print Spooler", "Handles print jobs", WindowsServiceStartupMode.Manual, "Manual is safer than disabled unless the user never prints", PackIconKind.Printer),
            Service("QWAVE", "Quality Windows Audio Video Experience", "Supports QoS for audio/video streaming", WindowsServiceStartupMode.Manual, "Manual keeps multimedia compatibility without forcing it always on", PackIconKind.Waveform),
            Service("WSearch", "Windows Search", "Indexes files for faster search", WindowsServiceStartupMode.Manual, "Manual reduces indexing activity while keeping search available", PackIconKind.FileSearchOutline),
            Service("W32Time", "Windows Time", "Keeps system time synchronized", WindowsServiceStartupMode.Manual, "Manual is safer than disabled because time sync affects sign-ins, certificates, and online services", PackIconKind.Clock),
            Service("LanmanWorkstation", "Workstation", "Supports SMB network shares and network file access", WindowsServiceStartupMode.Manual, "Do not disable if using NAS, shared folders, or network drives", PackIconKind.Lan),
            Service("ssh-agent", "OpenSSH Authentication Agent", "Stores SSH private keys for developer workflows", WindowsServiceStartupMode.Manual, "Manual is safest unless the user never uses SSH", PackIconKind.ConsoleNetwork),
            Service("GoogleChromeElevationService", "Google Chrome Elevation Service", "Used by Chrome update/install tasks", WindowsServiceStartupMode.Manual, "Manual keeps Chrome maintenance working when needed", PackIconKind.GoogleChrome),
            Service("gupdate", "Google Update Service", "Google updater service", WindowsServiceStartupMode.Manual, "Manual avoids always-on updater behaviour without fully breaking updates", PackIconKind.Google),
            Service("edgeupdate", "Microsoft Edge Update Service", "Microsoft Edge updater service", WindowsServiceStartupMode.Manual, "Manual keeps Edge updates available when needed", PackIconKind.MicrosoftEdge),
            Service("edgeupdatem", "Microsoft Edge Update Service Machine", "Microsoft Edge updater service", WindowsServiceStartupMode.Manual, "Manual keeps Edge updates available when needed", PackIconKind.MicrosoftEdge),
            Service("DPS", "Diagnostic Policy Service", "Enables Windows diagnostic detection, troubleshooting, and problem resolution", WindowsServiceStartupMode.Manual, "Manual is recommended because this service can help diagnose network and Windows issues without needing to stay aggressively always-on", PackIconKind.Stethoscope, highRisk: true),
            Service("WdiSystemHost", "Diagnostic System Host", "Hosts Windows diagnostic components used by troubleshooting services", WindowsServiceStartupMode.Manual, "Manual is recommended because disabling this can reduce Windows troubleshooting functionality", PackIconKind.Tools,highRisk: true),
            Service("DusmSvc", "Data Usage", "Tracks network data usage for Windows and apps", WindowsServiceStartupMode.Manual, "Manual is recommended for most gaming desktops because it reduces always-on background behaviour while keeping data usage features available if needed", PackIconKind.ChartLine),
            Service("RmSvc", "Radio Management Service", "Manages radio and airplane-mode related device state", WindowsServiceStartupMode.Manual, "Manual is safer than Disabled, especially on laptops or systems with wireless/radio hardware", PackIconKind.AccessPointNetwork),
            Service("TeamViewer", "TeamViewer", "Allows TeamViewer remote access and background connectivity", WindowsServiceStartupMode.Manual, "Manual is recommended if you use TeamViewer remote access, because it keeps the service available when needed", PackIconKind.RemoteDesktop),
            Service("wuauserv", "Windows Update", "Enables detection, download, and installation of Windows updates", WindowsServiceStartupMode.Manual, "Manual is recommended instead of Disabled because Windows Update is important for security updates, Microsoft Store apps, drivers, and system servicing", PackIconKind.Update, true),



            // Keep automatic / avoid aggressive disable
            Service("SysMain", "SysMain", "Windows memory and app preloading service", WindowsServiceStartupMode.Automatic, "Keep automatic unless there are clear disk or RAM usage issues", PackIconKind.Memory, highRisk: true),

            // Extra optional
            Service("MapsBroker", "Downloaded Maps Manager", "Manages offline downloaded maps", WindowsServiceStartupMode.Disabled, "Usually unnecessary on gaming desktops", PackIconKind.MapMarkerOff),
            Service("WMPNetworkSvc", "Windows Media Player Network Sharing", "Shares media libraries over the network", WindowsServiceStartupMode.Disabled, "Usually unnecessary unless using media library sharing", PackIconKind.Multimedia),
            Service("PcaSvc", "Program Compatibility Assistant", "Detects compatibility issues for older apps", WindowsServiceStartupMode.Manual, "Manual is safer because some older games/apps may benefit from compatibility assistance", PackIconKind.ApplicationCog),
            Service("SharedAccess", "Internet Connection Sharing", "Provides internet sharing and mobile hotspot support", WindowsServiceStartupMode.Manual, "Manual is safer unless the user never uses hotspot or ICS", PackIconKind.WifiCog),

            // Xbox / Game Pass optional
            Service("XblAuthManager", "Xbox Live Auth Manager", "Xbox Live sign-in support", WindowsServiceStartupMode.Manual, "Manual is safest for users who may use Xbox, Game Pass, or Microsoft Store games", PackIconKind.MicrosoftXbox),
            Service("XblGameSave", "Xbox Live Game Save", "Xbox cloud save support", WindowsServiceStartupMode.Manual, "Manual is safest for Game Pass and Xbox cloud saves", PackIconKind.ContentSave),
            Service("XboxGipSvc", "Xbox Accessory Management", "Supports Xbox accessories and controllers", WindowsServiceStartupMode.Manual, "Manual is safer because controllers may need this service", PackIconKind.Controller),
            Service("XboxNetApiSvc", "Xbox Live Networking Service", "Xbox networking support", WindowsServiceStartupMode.Manual, "Manual is safest for Game Pass and Xbox network features", PackIconKind.MicrosoftXbox),
        ];

        private static ServiceEditInfo Service(
            string serviceName,
            string displayName,
            string description,
            WindowsServiceStartupMode recommended,
            string reason,
            PackIconKind icon,
            bool highRisk = false)
        {
            return new ServiceEditInfo
            {
                Name = serviceName,
                DisplayName = displayName,
                Description = description,
                RecommendedStartupMode = recommended,
                RecommendationReason = reason,
                Icon = icon,
                HighRisk = highRisk,
                Admin = true,
                SystemReboot = true
            };
        }
    }
}