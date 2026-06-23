using GameBoost.Application.Selection.Definitions;
using GameBoost.Features.Modules.SystemModules.Cleanup;
using GameBoost.Features.Modules.SystemModules.Cleanup.AppDataOrphan;
using GameBoost.Features.Modules.SystemModules.MicrosoftApps;
using GameBoost.Features.Modules.SystemModules.MSIMode;
using GameBoost.Features.Modules.SystemModules.NetworkTroubleshoot;
using GameBoost.Features.Modules.SystemModules.SystemTroubleshoot;
using MaterialDesignThemes.Wpf;

namespace GameBoost.Application.Selection.Registries
{
    public static class SystemFeatureRegistry
    {
        public static IReadOnlyList<FeatureDefinition> GetFeatures()
        {
            var features = new List<FeatureDefinition>
            {
                Cleanup(),
                NetworkTroubleshoot(),
                MicrosoftAppInstall(),
                SystemTroubleshoot(),
            };

            #region MSI Mode
            var msiModeFeature = MsiModeFeatureFactory.CreateFeature();

            if (msiModeFeature.Actions.Count > 0)
                features.Add(msiModeFeature);
            #endregion

            return features;
        }

        private static FeatureDefinition MicrosoftAppInstall() =>
            MicrosoftAppFeatureFactory.CreateInstallFeature();

        private static FeatureDefinition SystemTroubleshoot()
        {
            return new FeatureDefinition
            {
                Title = "System Troubleshoot",
                Description = "Run Windows system repair and diagnostic commands to help fix corrupted system files, component store issues, and disk problems",
                Icon = PackIconKind.Tools,
                SelectionType = SelectionType.Single,

                Actions =
                [
                    new ActionCardDefinition
                    {
                        Title = "SFC Scan",
                        Icon = PackIconKind.FileSearch,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new SfcScanModule(),
                        InfoToolTip = "Opens an elevated Command Prompt and runs sfc /scannow. This scans and repairs protected Windows system files"
                    },

                    new ActionCardDefinition
                    {
                        Title = "DISM Check Health",
                        Icon = PackIconKind.HeartPulse,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new DismCheckHealthModule(),
                        InfoToolTip = "Runs a quick DISM health check to see whether Windows component store corruption has already been detected"
                    },

                    new ActionCardDefinition
                    {
                        Title = "DISM Scan Health",
                        Icon = PackIconKind.MagnifyScan,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new DismScanHealthModule(),
                        InfoToolTip = "Runs a deeper DISM scan to check the Windows component store for corruption. This can take several minutes"
                    },

                    new ActionCardDefinition
                    {
                        Title = "DISM Restore Health",
                        Icon = PackIconKind.WrenchCog,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new DismRestoreHealthModule(),
                        InfoToolTip = "Runs DISM /RestoreHealth to repair the Windows component store. This may use Windows Update as a repair source"
                    },

                    new ActionCardDefinition
                    {
                        Title = "Windows Component Cleanup",
                        Icon = PackIconKind.PackageVariantClosedRemove,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new DismComponentCleanupModule(),
                        InfoToolTip = "Runs DISM component cleanup to remove superseded Windows component files and reduce component store bloat"
                    },

                    new ActionCardDefinition
                    {
                        Title = "Check Disk Online Scan",
                        Icon = PackIconKind.Harddisk,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new CheckDiskOnlineScanModule(),
                        InfoToolTip = "Runs an online CHKDSK scan on the system drive to check for file-system issues without forcing an immediate restart"
                    },

                    new ActionCardDefinition
                    {
                        Title = "Check Disk Spot Fix",
                        Icon = PackIconKind.HarddiskPlus,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new CheckDiskSpotFixModule(),
                        InfoToolTip = "Runs CHKDSK spot fix on the system drive. This may require exclusive disk access or a restart to complete repairs"
                    }
                ]
            };
        }
        private static FeatureDefinition Cleanup()
        {
            return new FeatureDefinition
            {
                Title = "Disk Cleanup",
                Description = "Clean temporary data and optimise disk usage for improved responsiveness",
                Icon = PackIconKind.Broom,

                Actions =
                [
                    new ActionCardDefinition
                    {
                        Title = "Temporary Files",
                        Icon = PackIconKind.Broom,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new TempFileModule()
                    },

                    new ActionCardDefinition
                    {
                        Title = "Recycle Bin",
                        Icon = PackIconKind.TrashCanOutline,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new RecyclingBinModule()
                    },

                    new ActionCardDefinition
                    {
                        Title = "AppData Orphans",
                        Icon = PackIconKind.FolderRemove,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new AppDataOrphanCleanerModule(),
                    },

                    new ActionCardDefinition
                    {
                        Title = "Windows Disk Clean-up",
                        Icon = PackIconKind.HarddiskRemove,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new WindowsDiskCleanupModule(),
                        InfoToolTip = "Windows Disk Clean-up removes unnecessary Windows files such as temporary files, cache, update leftovers, recycle bin items, and system cleanup files to free up drive space",
                    }
                ]
            };
        }
        private static FeatureDefinition NetworkTroubleshoot()
        {
            return new FeatureDefinition
            {
                Title = "Network Troubleshoot",
                Description = "Run network troubleshooting scripts to resolve connectivity issues and clear outdated configurations",
                Icon = PackIconKind.HelpNetwork,
                SelectionType = SelectionType.Single,

                Actions =
                [
                    new ActionCardDefinition
                    {
                        Title = "Flush DNS",
                        Icon = PackIconKind.Dns,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new FlushDNSModule(),
                        InfoToolTip = "Clears the local DNS cache, which can help fix website loading issues, stale DNS records, or connection problems after DNS/network changes"
                    },

                    new ActionCardDefinition
                    {
                        Title = "Release and Renew IP",
                        Icon = PackIconKind.IpNetwork,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new ReleaseAndRenewIPModule(),
                        InfoToolTip = "Releases the current IP address and requests a new one from the router/DHCP server, which can help fix local network connection issues or IP conflicts"
                    },

                    new ActionCardDefinition
                    {
                        Title = "Clear ARP Cache",
                        Icon = PackIconKind.SilverwareClean,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new ClearArpCacheModule(),
                        InfoToolTip = "Clears stored IP-to-MAC address mappings, which can help fix local network communication issues caused by outdated or incorrect ARP entries"
                    },

                    new ActionCardDefinition
                    {
                        Title = "Reset Winsock",
                        Icon = PackIconKind.Restart,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new ResetWinsockModule(),
                        InfoToolTip = "Resets the Windows network socket catalog, which can help fix broken internet connectivity caused by corrupted network settings or faulty networking software"
                    },

                    new ActionCardDefinition
                    {
                        Title = "Restart Network Services",
                        Icon = PackIconKind.NetworkStrength4Warning,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new RestartNetworkServicesModule(),
                        InfoToolTip = "Restarts key Windows networking services, which can help recover network functionality without needing a full PC restart"
                    },

                    new ActionCardDefinition
                    {
                        Title = "Restart Network Adapter",
                        Icon = PackIconKind.RouterNetwork,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new RestartNetworkAdapterModule(),
                        InfoToolTip = "Disables and re-enables the selected network adapter, which can help fix temporary Wi-Fi or Ethernet connection issues"
                    }
                ]
            };
        }
    }
}