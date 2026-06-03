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
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Release and Renew IP",
                    Icon = PackIconKind.IpNetwork,
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Clear ARP Cache",
                    Icon = PackIconKind.SilverwareClean,
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Reset Winsock",
                    Icon = PackIconKind.Restart,
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Restart Network Services",
                    Icon = PackIconKind.NetworkStrength4Warning,
                },
                new MultipurposeActionCardViewModel()
                {
                    Title = "Restart Network Adapter",
                    Icon = PackIconKind.RouterNetwork,
                },
             ]);

            return networkTroubleshoot;
        }
    }
}
