using GameBoost.Features.Modules.System.NetworkTroubleshoot;
using GameBoost.MVVM.ViewModels.Shared.Selection;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions;
using MaterialDesignThemes.Wpf;

namespace GameBoost.MVVM.ViewModels
{
    public class SystemViewModel : SelectionViewModel
    {

        public SystemViewModel(string pageTitle)
        {
            PageTitle = pageTitle;

            FeatureCards =
            [
                NetworkTroubleshoot(),
            ];
        }

        private static SelectionFeatureViewModel NetworkTroubleshoot()
        {
            var networkTroubleshoot = new SelectionFeatureViewModel
            {
                Title = "Network Troubleshoot",
                Description = "Running network troubleshooting scripts is essential for resolving connectivity issues, clearing outdated configurations, and improving network speed",
                Icon = PackIconKind.HelpNetwork,
                SelectionType = SelectionType.Single,
            };
            networkTroubleshoot.AddActions(
            [
                new MultipurposeActionCardViewModel()
                {
                    Title = "Flush DNS",
                    Icon = PackIconKind.Dns,
                    Module = new FlushDNSModule(),
                    InfoToolTip = "Clears the local DNS cache, which can help fix website loading issues, stale DNS records, or connection problems after DNS/network changes",
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Release and Renew IP",
                    Icon = PackIconKind.IpNetwork,
                    Module = new ReleaseAndRenewIPModule(),
                    InfoToolTip = "Releases the current IP address and requests a new one from the router/DHCP server, which can help fix local network connection issues or IP conflicts",
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Clear ARP Cache",
                    Icon = PackIconKind.SilverwareClean,
                    Module = new ClearArpCacheModule(),
                    InfoToolTip = "Clears stored IP-to-MAC address mappings, which can help fix local network communication issues caused by outdated or incorrect ARP entries",
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Reset Winsock",
                    Icon = PackIconKind.Restart,
                    Module = new ResetWinsockModule(),
                    InfoToolTip = "Resets the Windows network socket catalog, which can help fix broken internet connectivity caused by corrupted network settings or faulty networking software",
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Restart Network Services",
                    Icon = PackIconKind.NetworkStrength4Warning,
                    Module = new RestartNetworkServicesModule(),
                    InfoToolTip = "Restarts key Windows networking services, which can help recover network functionality without needing a full PC restart",
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Restart Network Adapter",
                    Icon = PackIconKind.RouterNetwork,
                    Module = new RestartNetworkAdapterModule(),
                    InfoToolTip = "Disables and re-enables the selected network adapter, which can help fix temporary Wi-Fi or Ethernet connection issues",
                },
             ]);

            return networkTroubleshoot;
        }
    }
}
