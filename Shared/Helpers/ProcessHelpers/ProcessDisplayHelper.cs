using GameBoost.Application.FileLookup;
using MaterialDesignThemes.Wpf;

namespace GameBoost.Shared.Helpers.ProcessHelpers
{
    public static class ProcessDisplayHelper
    {
        public static string GetTitle(FileLockInfo fileLockInfo)
        {
            if (!string.IsNullOrWhiteSpace(fileLockInfo.ApplicationName))
                return fileLockInfo.ApplicationName;

            if (IsService(fileLockInfo))
                return GetFriendlyServiceTitle(fileLockInfo.ServiceName);

            if (!string.IsNullOrWhiteSpace(fileLockInfo.ProcessName))
                return GetFriendlyProcessTitle(fileLockInfo.ProcessName);

            return "Unknown process";
        }

        public static PackIconKind GetIcon(FileLockInfo fileLockInfo)
        {
            string processName = Normalize(fileLockInfo.ProcessName);
            string serviceName = Normalize(fileLockInfo.ServiceName);
            string appType = Normalize(fileLockInfo.ApplicationType);

            if (IsMicrosoft(fileLockInfo))
                return PackIconKind.Microsoft;

            if (IsCritical(fileLockInfo))
                return PackIconKind.AlertCircle;

            if (IsSecurityProcess(processName, serviceName))
                return PackIconKind.ShieldCheck;

            if (IsExplorer(fileLockInfo))
                return PackIconKind.FolderOpen;

            if (IsService(fileLockInfo))
                return PackIconKind.Cog;

            if (IsSearchOrIndexing(processName, serviceName))
                return PackIconKind.Magnify;

            if (IsPrintService(processName, serviceName))
                return PackIconKind.Printer;

            if (IsInstallerOrUpdater(processName, serviceName))
                return PackIconKind.Update;

            if (IsConsoleProcess(processName, appType))
                return PackIconKind.Console;

            if (IsSystemProcess(processName))
                return PackIconKind.DesktopTower;

            return PackIconKind.Application;
        }


        public static string GetStatus(FileLockInfo fileLockInfo)
        {
            string processName = Normalize(fileLockInfo.ProcessName);
            string serviceName = Normalize(fileLockInfo.ServiceName);

            if (IsCritical(fileLockInfo))
                return "Protected system process";

            if (IsSecurityProcess(processName, serviceName))
                return "Security service";

            if (IsInstallerOrUpdater(processName, serviceName))
                return "Installer or updater";

            if (IsExplorer(fileLockInfo))
                return "Windows shell";

            if (IsService(fileLockInfo))
                return "Windows service";

            if (fileLockInfo.Restartable)
                return "Can request close";

            return "Review before closing";
        }

        public static string GetSubtitle(FileLockInfo fileLockInfo)
        {
            string processName = string.IsNullOrWhiteSpace(fileLockInfo.ProcessName)
                ? "Unknown process"
                : fileLockInfo.ProcessName;

            return $"{processName} · PID {fileLockInfo.ProcessId}";
        }

        public static string GetInfoToolTip(FileLockInfo fileLockInfo)
        {
            return
                $"Process: {GetSafeValue(fileLockInfo.ProcessName)}\n" +
                $"PID: {fileLockInfo.ProcessId}\n" +
                $"Service: {GetSafeValue(fileLockInfo.ServiceName)}\n" +
                $"Type: {GetSafeValue(fileLockInfo.ApplicationType)}\n" +
                $"Restartable: {(fileLockInfo.Restartable ? "Yes" : "No")}";
        }
        private static bool IsMicrosoft(FileLockInfo fileLockInfo)
        {
            return Normalize(fileLockInfo.ApplicationName).Contains("microsoft");
        }

        private static bool IsCritical(FileLockInfo fileLockInfo)
        {
            string appType = Normalize(fileLockInfo.ApplicationType);
            string processName = Normalize(fileLockInfo.ProcessName);

            return appType.Contains("critical") ||
                   processName is "system" or "registry" or "smss.exe" or "csrss.exe" or "wininit.exe" or "services.exe" or "lsass.exe";
        }

        private static bool IsExplorer(FileLockInfo fileLockInfo)
        {
            string processName = Normalize(fileLockInfo.ProcessName);
            string appType = Normalize(fileLockInfo.ApplicationType);

            return processName == "explorer.exe" ||
                   appType.Contains("explorer");
        }

        private static bool IsService(FileLockInfo fileLockInfo)
        {
            string serviceName = Normalize(fileLockInfo.ServiceName);
            string appType = Normalize(fileLockInfo.ApplicationType);
            string processName = Normalize(fileLockInfo.ProcessName);

            return !string.IsNullOrWhiteSpace(serviceName) ||
                   appType.Contains("service") ||
                   processName == "svchost.exe";
        }

        private static bool IsSecurityProcess(string processName, string serviceName)
        {
            return processName is "msmpeng.exe" or "securityhealthservice.exe" or "smartscreen.exe" ||
                   serviceName is "windefend" or "securityhealthservice" or "sense";
        }

        private static bool IsSearchOrIndexing(string processName, string serviceName)
        {
            return processName is "searchindexer.exe" or "searchhost.exe" or "searchprotocolhost.exe" ||
                   serviceName is "wsearch";
        }

        private static bool IsPrintService(string processName, string serviceName)
        {
            return processName is "spoolsv.exe" ||
                   serviceName is "spooler";
        }

        private static bool IsInstallerOrUpdater(string processName, string serviceName)
        {
            return processName is "msiexec.exe" or "trustedinstaller.exe" or "tiworker.exe" or "wuauclt.exe" ||
                   serviceName is "trustedinstaller" or "wuauserv" or "bits";
        }

        private static bool IsConsoleProcess(string processName, string appType)
        {
            return appType.Contains("console") ||
                   processName is "cmd.exe" or "powershell.exe" or "pwsh.exe" or "conhost.exe" or "windowsterminal.exe";
        }

        private static bool IsSystemProcess(string processName)
        {
            return processName is "svchost.exe" or "rundll32.exe" or "dllhost.exe" or "runtimebroker.exe";
        }

        private static string GetFriendlyProcessTitle(string processName)
        {
            string normalized = Normalize(processName);

            return normalized switch
            {
                "explorer.exe" => "Windows Explorer",
                "svchost.exe" => "Windows Service Host",
                "msmpeng.exe" => "Microsoft Defender Antivirus",
                "searchindexer.exe" => "Windows Search Indexer",
                "searchhost.exe" => "Windows Search",
                "spoolsv.exe" => "Print Spooler",
                "trustedinstaller.exe" => "Windows Modules Installer",
                "tiworker.exe" => "Windows Modules Installer Worker",
                "msiexec.exe" => "Windows Installer",
                "runtimebroker.exe" => "Runtime Broker",
                "dllhost.exe" => "COM Surrogate",
                "rundll32.exe" => "Windows Host Process",
                "cmd.exe" => "Command Prompt",
                "powershell.exe" => "Windows PowerShell",
                "pwsh.exe" => "PowerShell",
                "conhost.exe" => "Console Window Host",
                _ => processName
            };
        }

        private static string GetFriendlyServiceTitle(string serviceName)
        {
            string normalized = Normalize(serviceName);

            return normalized switch
            {
                "windefend" => "Microsoft Defender Antivirus Service",
                "wsearch" => "Windows Search Service",
                "spooler" => "Print Spooler Service",
                "trustedinstaller" => "Windows Modules Installer Service",
                "wuauserv" => "Windows Update Service",
                "bits" => "Background Intelligent Transfer Service",
                "eventlog" => "Windows Event Log Service",
                "schedule" => "Task Scheduler Service",
                "cryptsvc" => "Cryptographic Services",
                _ => serviceName
            };
        }

        private static string GetSafeValue(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? "None" : value;
        }

        private static string Normalize(string? value)
        {
            return value?.Trim().ToLowerInvariant() ?? string.Empty;
        }
    }
}