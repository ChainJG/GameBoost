using GameBoost.Application.Selection.Definitions;
using GameBoost.Features.Modules.SystemModules.Cleanup;
using GameBoost.Features.Modules.SystemModules.NetworkTroubleshoot;
using MaterialDesignThemes.Wpf;

namespace GameBoost.Application.Selection.Registries
{
    public static class SystemFeatureRegistry
    {
        public static IReadOnlyList<FeatureDefinition> GetFeatures()
        {
            return
            [
                Cleanup(),
                NetworkTroubleshoot()
            ];
        }

        private static FeatureDefinition Cleanup()
        {
            return new FeatureDefinition
            {
                Title = "Disk Cleanup",
                Description = "Clean temporary data and optimise disk usage for improved responsiveness.",
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
                    }
                ]
            };
        }

        private static FeatureDefinition NetworkTroubleshoot()
        {
            return new FeatureDefinition
            {
                Title = "Network Troubleshoot",
                Description = "Run network troubleshooting scripts to resolve connectivity issues and clear outdated configurations.",
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
                        InfoToolTip = "Clears the local DNS cache, which can help fix website loading issues, stale DNS records, or connection problems after DNS/network changes."
                    },

                    new ActionCardDefinition
                    {
                        Title = "Release and Renew IP",
                        Icon = PackIconKind.IpNetwork,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new ReleaseAndRenewIPModule(),
                        InfoToolTip = "Releases the current IP address and requests a new one from the router/DHCP server, which can help fix local network connection issues or IP conflicts."
                    },

                    new ActionCardDefinition
                    {
                        Title = "Clear ARP Cache",
                        Icon = PackIconKind.SilverwareClean,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new ClearArpCacheModule(),
                        InfoToolTip = "Clears stored IP-to-MAC address mappings, which can help fix local network communication issues caused by outdated or incorrect ARP entries."
                    },

                    new ActionCardDefinition
                    {
                        Title = "Reset Winsock",
                        Icon = PackIconKind.Restart,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new ResetWinsockModule(),
                        InfoToolTip = "Resets the Windows network socket catalog, which can help fix broken internet connectivity caused by corrupted network settings or faulty networking software."
                    },

                    new ActionCardDefinition
                    {
                        Title = "Restart Network Services",
                        Icon = PackIconKind.NetworkStrength4Warning,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new RestartNetworkServicesModule(),
                        InfoToolTip = "Restarts key Windows networking services, which can help recover network functionality without needing a full PC restart."
                    },

                    new ActionCardDefinition
                    {
                        Title = "Restart Network Adapter",
                        Icon = PackIconKind.RouterNetwork,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new RestartNetworkAdapterModule(),
                        InfoToolTip = "Disables and re-enables the selected network adapter, which can help fix temporary Wi-Fi or Ethernet connection issues."
                    }
                ]
            };
        }
    }
}